using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using StackFix.Compat;
using StackFix.Settings;
using StackFix.Utils;
using System;

namespace StackFix.Items.Impl {
	internal class BreachingFin: ItemFix {
		private const int VANILLA_AIRBORNE_LAUNCHES_BASE = 2;
		private const int VANILLA_AIRBORNE_LAUNCHES_STACK = 1;
		private const float VANILLA_DAMAGE_BASE = 0.1f;
		private const float VANILLA_DAMAGE_BASE_PERCENT = VANILLA_DAMAGE_BASE * 100f;
		private const float VANILLA_COOLDOWN_BASE = 15f;

		private ConfigEntry<int> airLaunchesBase;
		private ConfigEntry<int> airLaunchesStack;
		private ConfigEntry<float> damageBase;
		private ConfigEntry<float> damageStack;
		private ConfigEntry<float> cooldownBase;
		private ConfigEntry<float> cooldownStack;
		private ConfigEntry<ModSettings.BreachingFinCooldownStacking> cooldownStackType;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled = ModSettings.BreachingFinFixEnabled.RegisterFor(plugin);
			this.airLaunchesBase = ModSettings.BreachingFinAirLaunchesBase.RegisterFor(plugin);
			this.airLaunchesStack = ModSettings.BreachingFinAirLaunchesStack.RegisterFor(plugin);
			this.damageBase = ModSettings.BreachingFinDamageBase.RegisterFor(plugin);
			this.damageStack = ModSettings.BreachingFinDamageStack.RegisterFor(plugin);
			this.cooldownBase = ModSettings.BreachingFinCooldownBase.RegisterFor(plugin);
			this.cooldownStack = ModSettings.BreachingFinCooldownStack.RegisterFor(plugin);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompat.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompat.AddSliderOption(this.airLaunchesBase);
			RiskOfOptionsCompat.AddSliderOption(this.airLaunchesStack);
			RiskOfOptionsCompat.AddSliderOption(this.damageBase, new(Suffix: "%"));
			RiskOfOptionsCompat.AddSliderOption(this.damageStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownBase, new(FormatString: "{0:0.#}s", Step: 0.1f));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompat.AddChoiceOption(this.cooldownStackType);
		}

		public override void RegisterLookingGlassStats() {
			int airLaunchesBase() => this.Enabled ? this.airLaunchesBase.Value : VANILLA_AIRBORNE_LAUNCHES_BASE;
			int airLaunchesStack() => this.Enabled ? this.airLaunchesStack.Value : VANILLA_AIRBORNE_LAUNCHES_STACK;

			float damageBase() => this.Enabled ? this.damageBase.Value * 0.01f : VANILLA_DAMAGE_BASE;
			float damageStack() => this.Enabled ? this.damageStack.Value * 0.01f : 0f;

			float cooldownBase() => this.Enabled ? this.cooldownBase.Value : VANILLA_COOLDOWN_BASE;
			float cooldownStack() => this.Enabled ? this.cooldownStack.Value * 0.01f : 0f;
			ModSettings.BreachingFinCooldownStacking cooldownStackType()
				=> this.Enabled ? this.cooldownStackType.Value : ModSettings.BreachingFinCooldownStacking.Linear;

			LookingGlassCompat.AddItem(DLC2Content.Items.KnockBackHitEnemies, [
				new("Airborne launches",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Number,
					itemCount => ItemStacking.Linear(airLaunchesBase(), airLaunchesStack(), itemCount)),
				new("Damage",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.Percentage,
					itemCount => ItemStacking.Linear(damageBase(), damageStack(), itemCount)),
				new("Cooldown",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Seconds,
					itemCount => cooldownBase() * cooldownStackType() switch {
						ModSettings.BreachingFinCooldownStacking.Linear
							=> ItemStacking.Linear(1f, -cooldownStack(), itemCount),

						ModSettings.BreachingFinCooldownStacking.Reciprocal
							=> ItemStacking.Reciprocal(0f, cooldownStack(), itemCount),

						ModSettings.BreachingFinCooldownStacking.Exponential
							=> ItemStacking.Exponential(1f - cooldownStack(), itemCount - 1),

						_ => 1f,
					}),
			]);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			int airLaunchesBase = enabled ? this.airLaunchesBase.Value : VANILLA_AIRBORNE_LAUNCHES_BASE;
			int airLaunchesStack = enabled ? this.airLaunchesStack.Value : VANILLA_AIRBORNE_LAUNCHES_STACK;
			float damageBase = enabled ? this.damageBase.Value : VANILLA_DAMAGE_BASE_PERCENT;
			float damageStack = enabled ? this.damageStack.Value : 0f;
			float cooldownBase = enabled ? this.cooldownBase.Value : VANILLA_DAMAGE_BASE_PERCENT;
			float cooldownStack = enabled ? this.cooldownStack.Value : 0f;
			ItemStacking.Type cooldownStackType = enabled ? ItemStacking.FromEnum(this.cooldownStackType.Value) : ItemStacking.Type.Linear;

			string description =
				$"Grounded enemies hit with any {ItD.Utility("skill")} are {ItD.Utility("launched")} and {ItD.Utility("stunned<")}. " +
				$"Enemies hit while airborne are {ItD.Utility("launched")} again up to {ItD.Utility($"{airLaunchesBase} times")}" +
				$"{ItD.Stack(airLaunchesStack)}. Launched enemies take increased damage by {ItD.Damage($"{damageBase}%")}{ItD.Stack(damageStack)} " +
				$"from all sources per launch. Recharges after {ItD.Utility($"{cooldownBase}s")}{ItD.Stack(cooldownStack, cooldownStackType, "{0:-0.##;-0.##}%")}.";

			LanguageAPI.AddOverlay("ITEM_KNOCKBACKHITENEMIES_DESC", description);

			if(ItemQualitiesCompat.Installed)
				ItemQualitiesCompat.ApplyQualityDescriptions("KNOCKBACKHITENEMIES", description);
		}

		protected override void ApplyHooks() {
			//TODO: hook and impl
		}

	}
}
