using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API;
using RoR2;
using RoR2.Items;
using StackFix.Compat;
using StackFix.Exceptions;
using StackFix.Settings;
using StackFix.Utils;
using System;
using System.Reflection;

namespace StackFix.Items.Impl {
	internal sealed class NetworkedSuffering: ItemFix, IDisposable {
		private const int VANILLA_VIRUS_SPREAD_COUNT_BASE = 4;
		private const int VANILLA_VIRUS_SPREAD_COUNT_STACK = 2;
		private const float VANILLA_VIRUS_DAMAGE_BASE = 0.45f;

		private ConfigEntry<int> spreadBase;
		private ConfigEntry<int> spreadStack;
		private ConfigEntry<float> damageBase;
		private ConfigEntry<float> damageStack;

		private ILHook currentMaxHook;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled  = this.RegisterSetting(plugin, ModSettings.NetworkedSufferingFixEnabled);
			this.spreadBase  = this.RegisterSetting(plugin, ModSettings.NetworkedSufferingSpreadBase);
			this.spreadStack = this.RegisterSetting(plugin, ModSettings.NetworkedSufferingSpreadStack);
			this.damageBase  = this.RegisterSetting(plugin, ModSettings.NetworkedSufferingDamageBase);
			this.damageStack = this.RegisterSetting(plugin, ModSettings.NetworkedSufferingDamageStack);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompatManager.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompatManager.AddSliderOption(this.spreadBase);
			RiskOfOptionsCompatManager.AddSliderOption(this.spreadStack, new(Prefix: "+"));
			RiskOfOptionsCompatManager.AddSliderOption(this.damageBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompatManager.AddSliderOption(this.damageStack, new(Prefix: "+", Suffix: "%"));
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			int maxVirusCountBase  = enabled ? this.spreadBase.Value : VANILLA_VIRUS_SPREAD_COUNT_BASE;
			int maxVirusCountStack = enabled ? this.spreadStack.Value : VANILLA_VIRUS_SPREAD_COUNT_STACK;
			float damageBase       = enabled ? this.damageBase.Value : VANILLA_VIRUS_DAMAGE_BASE * 100f;
			float damageStack      = enabled ? this.damageStack.Value : 0f;

			LanguageAPI.AddOverlay("ITEM_SHAREDSUFFERING_DESC", new ItD(
				$"Infect up to {ItD.Utility($"{maxVirusCountBase}")}{ItD.Stack(maxVirusCountStack)} enemies with a computer virus that ",
				$"duplicates {ItD.Damage($"{damageBase}%{ItD.Stack(damageStack, "+{0:0}%")} of all damage taken")} and sends it to a global ",
				$"damage pool. Every 3 seconds, ALL infected enemies suffer {ItD.Damage("100% pooled damage")}."));
		}

		protected override void ApplyHooks() {
			this.InstallCurrentMaxHook();
			IL.RoR2.SharedSufferingManager.SendToDamagePool += this.SharedSufferingManager_SendToDamagePool;
		}

		private void InstallCurrentMaxHook() {
			PropertyInfo property = typeof(SharedSufferingItemBehaviour).GetProperty(
				"currentMax",
				BindingFlags.Instance | BindingFlags.NonPublic);

			MethodInfo getter = (property?.GetGetMethod(nonPublic: true))
				?? throw new MissingMethodException(typeof(SharedSufferingItemBehaviour).FullName, "get_currentMax");

			this.currentMaxHook = new ILHook(getter, this.CurrentMax);
		}

		private void CurrentMax(ILContext il) {
			il.Body.Instructions.Clear();

			ILCursor cursor = new(il);

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.OverwriteCurrentMax);
			cursor.Emit(OpCodes.Ret);
		}

		private int OverwriteCurrentMax(SharedSufferingItemBehaviour self) {
			bool enabled = this.Enabled;
			int initial = enabled ? this.spreadBase.Value : VANILLA_VIRUS_SPREAD_COUNT_BASE;
			int perStack = enabled ? this.spreadStack.Value : VANILLA_VIRUS_SPREAD_COUNT_STACK;

			return ItemStacking.Linear(initial, perStack, self.stack);
		}

		private void SharedSufferingManager_SendToDamagePool(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(
				MoveType.Before,
				x => x.MatchLdarg(0),
				x => x.MatchLdfld<SharedSufferingManager>("_damageSentToPool"),
				x => x.MatchMul()))
				throw new ILSeekException(il, "damageSentToPool", nameof(SharedSufferingManager.SendToDamagePool));

			cursor.RemoveRange(3);

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.Emit(OpCodes.Ldarg_1);

			cursor.EmitDelegate(this.ModifyDamageMultiplier);
			cursor.Emit(OpCodes.Mul);
		}

		private float ModifyDamageMultiplier(SharedSufferingManager manager, DamageReport report) {
			if(!this.Enabled)
				return manager._damageSentToPool;

			CharacterBody victimBody = report.victimBody;

			if(!victimBody)
				return manager._damageSentToPool;

			SharedSufferingBuffController buffController = victimBody.GetComponent<SharedSufferingBuffController>();

			CharacterBody attackerBody = buffController ? buffController.attacker : null;

			int itemCount = attackerBody?.inventory?.GetItemCountEffective(DLC3Content.Items.SharedSuffering) ?? 0;

			if(itemCount <= 0)
				return manager._damageSentToPool;

			float initial = this.damageBase.Value;
			float perStack = this.damageStack.Value;
			float totalPercent = ItemStacking.Linear(initial, perStack, itemCount);

			return totalPercent * 0.01f;
		}

		public void Dispose() {
			this.currentMaxHook?.Dispose();
			this.currentMaxHook = null;
		}
	}
}
