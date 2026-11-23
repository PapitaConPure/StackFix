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
using System;

namespace StackFix.Items.Impl {
	internal sealed class LuminousShot: ItemFix {
		private const int VANILLA_CHARGES_MAX_BASE = 5;
		private const int VANILLA_CHARGES_MAX_STACK = 1;
		private const float VANILLA_DAMAGE_BASE_PERCENT = 175f;
		private const float VANILLA_DAMAGE_STACK_PERCENT = 50f;
		private const float VANILLA_DAMAGE_BASE = VANILLA_DAMAGE_BASE_PERCENT * 0.01f;
		private const float VANILLA_DAMAGE_STACK = VANILLA_DAMAGE_STACK_PERCENT * 0.01f;
		private const float VANILLA_COOLDOWN_REDUCTION_BASE_PERCENT = 20f;
		private const float VANILLA_COOLDOWN_REDUCTION_BASE = VANILLA_COOLDOWN_REDUCTION_BASE_PERCENT * 0.01f;

		private ConfigEntry<int> chargesMaxBase;
		private ConfigEntry<int> chargesMaxStack;
		private ConfigEntry<float> damageBase;
		private ConfigEntry<float> damageStack;
		private ConfigEntry<float> cooldownReductionBase;
		private ConfigEntry<float> cooldownReductionStack;
		private ConfigEntry<ModSettings.LuminousShotCooldownReductionStacking> cooldownReductionStackType;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled                 = this.RegisterSetting(plugin, ModSettings.LuminousShotFixEnabled);
			this.chargesMaxBase             = this.RegisterSetting(plugin, ModSettings.LuminousShotChargesMaxBase);
			this.chargesMaxStack            = this.RegisterSetting(plugin, ModSettings.LuminousShotChargesMaxStack);
			this.damageBase                 = this.RegisterSetting(plugin, ModSettings.LuminousShotDamageBase);
			this.damageStack                = this.RegisterSetting(plugin, ModSettings.LuminousShotDamageStack);
			this.cooldownReductionBase      = this.RegisterSetting(plugin, ModSettings.LuminousShotCooldownReductionBase);
			this.cooldownReductionStack     = this.RegisterSetting(plugin, ModSettings.LuminousShotCooldownReductionStack);
			this.cooldownReductionStackType = this.RegisterSetting(plugin, ModSettings.LuminousShotCooldownReductionStackType);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompat.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompat.AddSliderOption(this.chargesMaxBase);
			RiskOfOptionsCompat.AddSliderOption(this.chargesMaxStack, new(Prefix: "+"));
			RiskOfOptionsCompat.AddSliderOption(this.damageBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompat.AddSliderOption(this.damageStack, new(FormatString: "+{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownReductionBase, new(Suffix: "%"));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownReductionStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompat.AddChoiceOption(this.cooldownReductionStackType);
		}

		public override void RegisterLookingGlassStats() {
			int chargesMaxBase() => this.Enabled ? this.chargesMaxBase.Value : VANILLA_CHARGES_MAX_BASE;
			int chargesMaxStack() => this.Enabled ? this.chargesMaxStack.Value : VANILLA_CHARGES_MAX_STACK;

			float damageBase() => this.Enabled ? this.damageBase.Value * 0.01f : VANILLA_DAMAGE_BASE;
			float damageStack() => this.Enabled ? this.damageStack.Value * 0.01f : VANILLA_DAMAGE_STACK;

			float cooldownBase() => this.Enabled ? this.cooldownReductionBase.Value * 0.01f : VANILLA_COOLDOWN_REDUCTION_BASE;
			float cooldownStack() => this.Enabled ? this.cooldownReductionStack.Value * 0.01f : 0f;
			ModSettings.LuminousShotCooldownReductionStacking cooldownStackType()
				=> this.Enabled ? this.cooldownReductionStackType.Value : ModSettings.LuminousShotCooldownReductionStacking.Linear;

			LookingGlassCompat.AddItem(DLC2Content.Items.IncreasePrimaryDamage, [
				new("Max Charges",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.Number,
					itemCount => ItemStacking.Linear(chargesMaxBase(), chargesMaxStack(), itemCount)),
				new("Total Damage",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.Percentage,
					itemCount => ItemStacking.Linear(damageBase(), damageStack(), itemCount)),
				new("Secondary Cooldown Reduction",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Percentage,
					itemCount => 1f - cooldownStackType() switch {
						ModSettings.LuminousShotCooldownReductionStacking.Linear
							=> 1f - ItemStacking.Linear(cooldownBase(), cooldownStack(), itemCount),

						ModSettings.LuminousShotCooldownReductionStacking.Reciprocal
							=> ItemStacking.Reciprocal(cooldownBase(), cooldownStack(), itemCount),

						ModSettings.LuminousShotCooldownReductionStacking.Exponential
							=> ItemStacking.Exponential(1f - cooldownBase(), 1f - cooldownStack(), itemCount),

						_ => 1f - cooldownBase(),
					}),
				new("Proc Coefficient",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.ProcCoeff,
					itemCount => 1f),
			]);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			int chargesMaxBase                  = enabled ? this.chargesMaxBase.Value : VANILLA_CHARGES_MAX_BASE;
			int chargesMaxStack                 = enabled ? this.chargesMaxStack.Value : VANILLA_CHARGES_MAX_STACK;
			float damageBasePercent             = enabled ? this.damageBase.Value : VANILLA_DAMAGE_BASE_PERCENT;
			float damageStackPercent            = enabled ? this.damageStack.Value : VANILLA_DAMAGE_STACK_PERCENT;
			float cooldownBasePercent           = enabled ? this.cooldownReductionBase.Value : VANILLA_COOLDOWN_REDUCTION_BASE_PERCENT;
			float cooldownStackPercent          = enabled ? this.cooldownReductionStack.Value : 0f;
			ItemStacking.Type cooldownStackType = enabled ? ItemStacking.FromEnum(this.cooldownReductionStackType.Value) : ItemStacking.Type.Linear;

			string description =
				$"Activating {ItD.Utility("Secondary skill")} stores up to {ItD.Utility($"{chargesMaxBase} charges")}{ItD.Stack(chargesMaxStack)}. " +
				$"Requires {ItD.Utility("3 charges")} for your {ItD.Utility("Primary skill")} to fire lightning strikes, dealing " +
				$"{ItD.Damage($"{damageBasePercent}% TOTAL damage")}{ItD.Stack(damageStackPercent, "+{0:0.##}%")} each. " +
				$"{ItD.Utility($"Reduces Secondary skill cooldown by {cooldownBasePercent}%")}{ItD.Stack(cooldownStackPercent, cooldownStackType, "+{0:0.##;0.##}%")}.";

			LanguageAPI.AddOverlay("ITEM_INCREASEPRIMARYDAMAGE_DESC", description);

			if(ItemQualitiesCompat.Installed)
				ItemQualitiesCompat.ApplyQualityDescriptions("INCREASEPRIMARYDAMAGE", description);
		}

		protected override void ApplyHooks() {
			IL.RoR2.CharacterBody.AddIncreasePrimaryDamageStack += this.CharacterBody_AddIncreasePrimaryDamageStack;
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += this.GlobalEventManager_ProcessHitEnemy;
			IL.RoR2.CharacterBody.RecalculateStats += this.CharacterBody_RecalculateStats;
		}

		private void CharacterBody_AddIncreasePrimaryDamageStack(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdsfld(typeof(DLC2Content.Items), nameof(DLC2Content.Items.IncreasePrimaryDamage)),
				instr => instr.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCountEffective))))
				throw new ILSeekException(il, "stack max buff count", nameof(CharacterBody.AddIncreasePrimaryDamageStack));

			Instruction itemCountInstr = cursor.Prev;

			if(!cursor.TryGotoPrev(MoveType.After,
				instr => instr.MatchLdcI4(VANILLA_CHARGES_MAX_BASE - 1)))
				throw new ILSeekException(il, "base max buff count", nameof(CharacterBody.AddIncreasePrimaryDamageStack));

			cursor.EmitDelegate(this.NullifyBaseMaxCharges);

			cursor.Goto(itemCountInstr, MoveType.After);
			cursor.EmitDelegate(this.ModifyMaxCharges);
		}

		private int NullifyBaseMaxCharges(int baseMaxCharges)
			=> this.Enabled ? 0 : baseMaxCharges;

		private int ModifyMaxCharges(int itemCount) {
			if(!this.Enabled)
				return itemCount;

			return ItemStacking.Linear(this.chargesMaxBase.Value, this.chargesMaxStack.Value, itemCount);
		}

		private void GlobalEventManager_ProcessHitEnemy(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdsfld(typeof(DLC2Content.Items), nameof(DLC2Content.Items.IncreasePrimaryDamage)),
				instr => instr.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCountEffective))))
				throw new ILSeekException(il, "item count", nameof(GlobalEventManager.ProcessHitEnemy));

			Instruction itemCountInstr = cursor.Prev;

			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdcR4(VANILLA_DAMAGE_BASE)))
				throw new ILSeekException(il, "base damage", nameof(GlobalEventManager.ProcessHitEnemy));

			Instruction baseDamageInstr = cursor.Prev;

			cursor.Goto(itemCountInstr, MoveType.After);
			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdcR4(VANILLA_DAMAGE_STACK)))
				throw new ILSeekException(il, "stack damage", nameof(GlobalEventManager.ProcessHitEnemy));

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

		private void CharacterBody_RecalculateStats(ILContext il) {
			ILCursor cursor = new(il);

			int itemCountEffectiveLocalIndex = default;
			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdsfld(typeof(DLC2Content.Items), nameof(DLC2Content.Items.IncreasePrimaryDamage)),
				instr => instr.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCountEffective)),
				instr => instr.MatchStloc(out itemCountEffectiveLocalIndex)))
				throw new ILSeekException(il, "item count", nameof(CharacterBody.RecalculateStats));

			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdloc(itemCountEffectiveLocalIndex),
				instr => instr.MatchLdcI4(0)))
				throw new ILSeekException(il, "cooldown reduction logical expression", nameof(CharacterBody.RecalculateStats));

			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdcR4(1f - VANILLA_COOLDOWN_REDUCTION_BASE)))
				throw new ILSeekException(il, "base cooldown reduction factor", nameof(CharacterBody.RecalculateStats));

			cursor.Emit(OpCodes.Ldloc, itemCountEffectiveLocalIndex);
			cursor.EmitDelegate(this.ModifyCooldownFactor);
		}

		private float ModifyCooldownFactor(float vanillaFactor, int itemCount) {
			if(!this.Enabled)
				return vanillaFactor;

			float initial = this.cooldownReductionBase.Value * 0.01f;
			float perStack = this.cooldownReductionStack.Value * 0.01f;

			float factor = this.cooldownReductionStackType.Value switch {
				ModSettings.LuminousShotCooldownReductionStacking.Linear
					=> 1f - ItemStacking.Linear(initial, perStack, itemCount),

				ModSettings.LuminousShotCooldownReductionStacking.Reciprocal
					=> ItemStacking.Reciprocal(initial, perStack, itemCount),

				ModSettings.LuminousShotCooldownReductionStacking.Exponential
					=> ItemStacking.Exponential(1f - initial, 1f - perStack, itemCount),

				_ => 1f - initial,
			};

			return MathF.Max(0.1f, factor);
		}
	}
}
