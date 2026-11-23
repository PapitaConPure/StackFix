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
	internal class LuminousShot: ItemFix {
		private const int VANILLA_CHARGES_MAX_BASE = 5;
		private const int VANILLA_CHARGES_MAX_STACK = 1;
		private const float VANILLA_DAMAGE_BASE_PERCENT = 40f;
		private const float VANILLA_DAMAGE_STACK_PERCENT = 10f;
		private const float VANILLA_DAMAGE_BASE = VANILLA_DAMAGE_BASE_PERCENT * 0.01f;
		private const float VANILLA_DAMAGE_STACK = VANILLA_DAMAGE_STACK_PERCENT * 0.01f;
		private const float VANILLA_COOLDOWN_REDUCTION_BASE_PERCENT = 20f;
		private const float VANILLA_COOLDOWN_REDUCTION_BASE = VANILLA_COOLDOWN_REDUCTION_BASE_PERCENT * 0.01f;

		private ConfigEntry<int> chargesMaxBase;
		private ConfigEntry<int> chargesMaxStack;
		private ConfigEntry<float> damageBase;
		private ConfigEntry<float> damageStack;
		private ConfigEntry<float> cooldownBase;
		private ConfigEntry<float> cooldownStack;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled      = this.RegisterSetting(plugin, ModSettings.LuminousShotFixEnabled);
			this.chargesMaxBase  = this.RegisterSetting(plugin, ModSettings.LuminousShotChargesMaxBase);
			this.chargesMaxStack = this.RegisterSetting(plugin, ModSettings.LuminousShotChargesMaxStack);
			this.damageBase      = this.RegisterSetting(plugin, ModSettings.LuminousShotDamageBase);
			this.damageStack     = this.RegisterSetting(plugin, ModSettings.LuminousShotDamageStack);
			this.cooldownBase    = this.RegisterSetting(plugin, ModSettings.LuminousShotCooldownReductionBase);
			this.cooldownStack   = this.RegisterSetting(plugin, ModSettings.LuminousShotCooldownReductionStack);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompat.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompat.AddSliderOption(this.damageBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompat.AddSliderOption(this.damageStack, new(FormatString: "+{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompat.AddSliderOption(this.chargesMaxBase);
			RiskOfOptionsCompat.AddSliderOption(this.chargesMaxStack, new(Prefix: "+"));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownBase, new(Suffix: "%"));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownStack, new(Prefix: "+", Suffix: "%"));
		}

		public override void RegisterLookingGlassStats() {
			int chargesMaxBase() => this.Enabled ? this.chargesMaxBase.Value : VANILLA_CHARGES_MAX_BASE;
			int chargesMaxStack() => this.Enabled ? this.chargesMaxStack.Value : VANILLA_CHARGES_MAX_STACK;

			float damageBase() => this.Enabled ? this.damageBase.Value * 0.01f : VANILLA_DAMAGE_BASE;
			float damageStack() => this.Enabled ? this.damageStack.Value * 0.01f : VANILLA_DAMAGE_STACK;

			float cooldownBase() => this.Enabled ? this.cooldownBase.Value * 0.01f : VANILLA_COOLDOWN_REDUCTION_BASE;
			float cooldownStack() => this.Enabled ? this.cooldownStack.Value * 0.01f : 0f;

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
					itemCount => ItemStacking.Linear(cooldownBase(), cooldownStack(), itemCount)),
				new("Proc Coefficient",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.ProcCoeff,
					itemCount => 1f),
			]);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			int chargesMaxBase         = enabled ? this.chargesMaxBase.Value : VANILLA_CHARGES_MAX_BASE;
			int chargesMaxStack        = enabled ? this.chargesMaxStack.Value : VANILLA_CHARGES_MAX_STACK;
			float damageBasePercent    = enabled ? this.damageBase.Value : VANILLA_DAMAGE_BASE_PERCENT;
			float damageStackPercent   = enabled ? this.damageStack.Value : VANILLA_DAMAGE_STACK_PERCENT;
			float cooldownBasePercent  = enabled ? this.cooldownBase.Value : VANILLA_COOLDOWN_REDUCTION_BASE_PERCENT;
			float cooldownStackPercent = enabled ? this.cooldownStack.Value : 0f;

			string description =
				$"Activating {ItD.Utility("Secondary skill")} stores up to {ItD.Utility($"{chargesMaxBase} charges")}{ItD.Stack(chargesMaxStack)}. " +
				$"Requires {ItD.Utility("3 charges")} for your {ItD.Utility("Primary skill")} to fire lightning strikes, dealing " +
				$"{ItD.Damage($"{damageBasePercent}% TOTAL damage")}{ItD.Stack(damageStackPercent, "+{0:0.##}%")} each. " +
				$"{ItD.Utility($"Reduces Secondary skill cooldown by {cooldownBasePercent}%")}{ItD.Stack(cooldownStackPercent, "+{0:0.##;0.##}%")}.";

			LanguageAPI.AddOverlay("ITEM_INCREASEPRIMARYDAMAGE_DESC", description);

			if(ItemQualitiesCompat.Installed)
				ItemQualitiesCompat.ApplyQualityDescriptions("INCREASEPRIMARYDAMAGE", description);
		}

		protected override void ApplyHooks() {
			//TODO: hook things and implement tweaks
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
	}
}
