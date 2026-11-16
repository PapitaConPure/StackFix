using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using StackFix.Compat;
using StackFix.Exceptions;
using StackFix.Settings;
using StackFix.Utils;

namespace StackFix.Items.Impl {
	internal class ChronicExpansion: ItemFix {
		private const float VANILLA_DAMAGE_BASE = 0.035f;
		private const float VANILLA_DAMAGE_STACK = 0.01f;
		private const int VANILLA_COMBO_MAX_BASE = 10;
		private const int VANILLA_COMBO_MAX_STACK = 5;
		private const float VANILLA_DURATION_BASE = 7;

		private ConfigEntry<float> damageBase;
		private ConfigEntry<float> damageStack;
		private ConfigEntry<int> comboMaxBase;
		private ConfigEntry<int> comboMaxStack;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled  = this.RegisterSetting(plugin, ModSettings.ChronicExpansionFixEnabled);
			this.damageBase  = this.RegisterSetting(plugin, ModSettings.ChronicExpansionDamageBase);
			this.damageStack = this.RegisterSetting(plugin, ModSettings.ChronicExpansionDamageStack);
			this.comboMaxBase  = this.RegisterSetting(plugin, ModSettings.ChronicExpansionComboMaxBase);
			this.comboMaxStack = this.RegisterSetting(plugin, ModSettings.ChronicExpansionComboMaxStack);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompat.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompat.AddSliderOption(this.damageBase, new(FormatString: "{0:0.##}%"));
			RiskOfOptionsCompat.AddSliderOption(this.damageStack, new(FormatString: "+{0:0.##}%"));
			RiskOfOptionsCompat.AddSliderOption(this.comboMaxBase);
			RiskOfOptionsCompat.AddSliderOption(this.comboMaxStack, new(Prefix: "+"));
		}

		public override void RegisterLookingGlassStats() {
			float damageBase() => this.Enabled ? this.damageBase.Value * 0.01f : VANILLA_DAMAGE_BASE;
			float damageStack() => this.Enabled ? this.damageStack.Value * 0.01f : VANILLA_DAMAGE_STACK;

			int comboMaxBase() => this.Enabled ? this.comboMaxBase.Value : VANILLA_COMBO_MAX_BASE;
			int comboMaxStack() => this.Enabled ? this.comboMaxStack.Value : VANILLA_COMBO_MAX_STACK;

			LookingGlassCompat.AddItem(DLC2Content.Items.IncreaseDamageOnMultiKill, [
				new("Damage per Kill",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.Percentage,
					itemCount => ItemStacking.Linear(damageBase(), damageStack(), itemCount)),
				new("Max Damage Increase",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.Percentage,
					itemCount => ItemStacking.Linear(damageBase(), damageStack(), itemCount)
							   * ItemStacking.Linear(comboMaxBase(), comboMaxStack(), itemCount)),
				new("Max Combo",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.Number,
					itemCount => ItemStacking.Linear(comboMaxBase(), comboMaxStack(), itemCount)),
			]);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float damageBasePercent  = enabled ? this.damageBase.Value : VANILLA_DAMAGE_BASE * 100f;
			float damageStackPercent = enabled ? this.damageStack.Value : VANILLA_DAMAGE_STACK * 100f;
			int comboMaxBase         = enabled ? this.comboMaxBase.Value : VANILLA_COMBO_MAX_BASE;
			int comboMaxStack        = enabled ? this.comboMaxStack.Value : VANILLA_COMBO_MAX_STACK;
			float durationBase       = VANILLA_DURATION_BASE;

			LanguageAPI.AddOverlay("ITEM_INCREASEDAMAGEONMULTIKILL_DESC",
				$"Killing an enemy increases your damage by {ItD.Damage($"{damageBasePercent}%")}{ItD.Stack(damageStackPercent, "+{0:0.##}%")}, " +
				$"up to {ItD.Utility(comboMaxBase)}{ItD.Stack(comboMaxStack)}, for {ItD.Utility($"{durationBase}s")}. " +
				$"Dealing damage refreshes the timer.");
		}

		protected override void ApplyHooks() {
			IL.RoR2.CharacterBody.GetIncreasedDamageFromMultiKillBuffsForCharacter += this.CharacterBody_GetIncreasedDamageFromMultiKillBuffsForCharacter;
			IL.RoR2.CharacterBody.GetMaxIncreasedDamageMultiKillBuffsForCharacter += this.CharacterBody_GetMaxIncreasedDamageMultiKillBuffsForCharacter;
		}

		private void CharacterBody_GetIncreasedDamageFromMultiKillBuffsForCharacter(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_DAMAGE_BASE)))
				throw new ILSeekException(il, "base damage", nameof(CharacterBody.GetIncreasedDamageFromMultiKillBuffsForCharacter));

			Instruction baseDamageInstr = cursor.Prev;

			cursor.Index = 0;
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_DAMAGE_STACK)))
				throw new ILSeekException(il, "stack damage", nameof(CharacterBody.GetIncreasedDamageFromMultiKillBuffsForCharacter));

			cursor.EmitDelegate(this.ModifyStackDamage);

			cursor.Goto(baseDamageInstr, MoveType.After);
			cursor.EmitDelegate(this.ModifyBaseDamage);
		}

		private float ModifyBaseDamage(float baseDamage) {
			if(!this.Enabled)
				return baseDamage;

			return this.damageBase.Value * 0.01f;
		}

		private float ModifyStackDamage(float stackDamage) {
			if(!this.Enabled)
				return stackDamage;

			return this.damageStack.Value * 0.01f;
		}

		private void CharacterBody_GetMaxIncreasedDamageMultiKillBuffsForCharacter(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_COMBO_MAX_BASE)))
				throw new ILSeekException(il, "base max combo", nameof(CharacterBody.GetMaxIncreasedDamageMultiKillBuffsForCharacter));

			Instruction baseComboInstr = cursor.Prev;

			cursor.Index = 0;
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_COMBO_MAX_STACK)))
				throw new ILSeekException(il, "stack max combo", nameof(CharacterBody.GetMaxIncreasedDamageMultiKillBuffsForCharacter));

			cursor.EmitDelegate(this.ModifyStackMaxCombo);

			cursor.Goto(baseComboInstr, MoveType.After);
			cursor.EmitDelegate(this.ModifyBaseMaxCombo);
		}

		private float ModifyBaseMaxCombo(float baseMaxCombo) {
			if(!this.Enabled)
				return baseMaxCombo;

			return this.comboMaxBase.Value;
		}

		private float ModifyStackMaxCombo(float stackMaxCombo) {
			if(!this.Enabled)
				return stackMaxCombo;

			return this.comboMaxStack.Value;
		}
	}
}
