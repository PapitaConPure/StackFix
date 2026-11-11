using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API;
using RoR2;
using StackFix.Compat;
using StackFix.Exceptions;
using StackFix.Settings;
using StackFix.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace StackFix.Items.Impl {
	internal class PrayerBeads: ItemFix, IDisposable {
		private const float VANILLA_STATS_BASE = 0.2f;
		private const float VANILLA_STATS_STACK = 0.05f;
		private const float VANILLA_STATS_BASE_PERCENT = VANILLA_STATS_BASE * 100f;
		private const float VANILLA_STATS_STACK_PERCENT = VANILLA_STATS_STACK * 100f;

		private ConfigEntry<float> statsBase;
		private ConfigEntry<float> statsStack;

		private const string THAT_FUCKING_METHOD_I_HATE = "<RecalculateStats>g__CalculateAppliedBeadStats";
		private ILHook calculateAppliedBeadStatsHook;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled = this.RegisterSetting(plugin, ModSettings.PrayerBeadsFixEnabled);
			this.statsBase  = this.RegisterSetting(plugin, ModSettings.PrayerBeadsStatsBase);
			this.statsStack = this.RegisterSetting(plugin, ModSettings.PrayerBeadsStatsStack);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompat.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompat.AddSliderOption(this.statsBase, new(Suffix: "%"));
			RiskOfOptionsCompat.AddSliderOption(this.statsStack, new(Prefix: "+", Suffix: "%"));
		}

		public override void RegisterLookingGlassStats() {
			float statsBasePercent() => this.Enabled ? this.statsBase.Value * 0.01f : VANILLA_STATS_BASE;
			float statsStackPercent() => this.Enabled ? this.statsStack.Value * 0.01f : VANILLA_STATS_STACK;

			LookingGlassCompat.AddItem(DLC2Content.Items.ExtraStatsOnLevelUp, [
				new("Stored Stats",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Percentage,
					itemCount => ItemStacking.Linear(statsBasePercent(), statsStackPercent(), itemCount)),
			]);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float statsBasePercent           = enabled ? this.statsBase.Value : VANILLA_STATS_BASE_PERCENT;
			float statsStackPercent          = enabled ? this.statsStack.Value : VANILLA_STATS_STACK_PERCENT;

			LanguageAPI.AddOverlay("ITEM_EXTRASTATSONLEVELUP_DESC",
				$"Grows by gaining experience, storing {ItD.Utility($"{statsBasePercent}%")}{ItD.Stack(statsStackPercent, "+{0:0.##}%")} " +
				$"bonus stats ({ItD.Healing("health")}, {ItD.Healing("regeneration")}, and {ItD.Damage("damage")}). " +
				$"On removal, gain all stored stats.");
		}

		protected override void ApplyHooks() {
			this.InstallCalculateAppliedBeadStatsHook();
		}

		private void InstallCalculateAppliedBeadStatsHook() {
			MethodInfo method = typeof(CharacterBody)
				.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
				.SingleOrDefault(method =>
					method.Name.StartsWith($"{THAT_FUCKING_METHOD_I_HATE}|", StringComparison.Ordinal))
				?? throw new MissingMethodException(typeof(CharacterBody).FullName, THAT_FUCKING_METHOD_I_HATE);

			this.calculateAppliedBeadStatsHook = new ILHook(method, this.CharacterBody_RecalculateStats_CalculateAppliedBeadStats);
		}

		private void CharacterBody_RecalculateStats_CalculateAppliedBeadStats(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_STATS_BASE)))
				throw new ILSeekException(il, "vanilla initial bonus stats", $"CharacterBody.{THAT_FUCKING_METHOD_I_HATE}");

			Instruction baseStatsInstr = cursor.Prev;

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_STATS_STACK)))
				throw new ILSeekException(il, "vanilla bonus stats per stack", $"CharacterBody.{THAT_FUCKING_METHOD_I_HATE}");

			cursor.EmitDelegate(this.ModifyStackStats);

			cursor.Goto(baseStatsInstr, MoveType.After);
			cursor.EmitDelegate(this.ModifyBaseStats);
		}

		private float ModifyBaseStats(float baseStats) {
			if(!this.Enabled)
				return baseStats;

			return this.statsBase.Value * 0.01f;
		}

		private float ModifyStackStats(float stackStats) {
			if(!this.Enabled)
				return stackStats;

			return this.statsStack.Value * 0.01f;
		}

		public void Dispose() {
			this.calculateAppliedBeadStatsHook?.Dispose();
			this.calculateAppliedBeadStatsHook = null;
		}
	}
}
