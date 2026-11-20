using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using StackFix.Compat;
using StackFix.Settings;
using StackFix.Utils;

namespace StackFix.Items.Impl {
	internal sealed class BoxOfDynamite: ItemFix {
		private const float VANILLA_DAMAGE_BASE = 2.4f;
		private const float VANILLA_DAMAGE_STACK = 0.85f;
		private const float VANILLA_DAMAGE_BASE_PERCENT = VANILLA_DAMAGE_BASE * 100f;
		private const float VANILLA_DAMAGE_STACK_PERCENT = VANILLA_DAMAGE_STACK * 100f;
		private const float VANILLA_COOLDOWN_BASE = 10f;

		private ConfigEntry<float> damageBase;
		private ConfigEntry<float> damageStack;
		private ConfigEntry<float> cooldownBase;
		private ConfigEntry<float> cooldownStack;
		private ConfigEntry<ModSettings.BoxOfDynamiteCooldownStacking> cooldownStackType;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled        = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteFixEnabled);
			this.damageBase        = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteDamageBase);
			this.damageStack       = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteDamageStack);
			this.cooldownBase      = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteCooldownBase);
			this.cooldownStack     = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteCooldownStack);
			this.cooldownStackType = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteCooldownStackType);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompat.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompat.AddSliderOption(this.damageBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompat.AddSliderOption(this.damageStack, new(FormatString: "+{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownBase, new(FormatString: "{0:0.#}s", Step: 0.1f));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompat.AddChoiceOption(this.cooldownStackType);
		}

		public override void RegisterLookingGlassStats() {
			float damageBase() => this.Enabled ? this.damageBase.Value * 0.01f : VANILLA_DAMAGE_BASE;
			float damageStack() => this.Enabled ? this.damageStack.Value * 0.01f : VANILLA_DAMAGE_STACK;

			float cooldownBase() => this.Enabled ? this.cooldownBase.Value : VANILLA_COOLDOWN_BASE;
			float cooldownStack() => this.Enabled ? this.cooldownStack.Value * 0.01f : 0f;
			ModSettings.BoxOfDynamiteCooldownStacking cooldownStackType()
				=> this.Enabled ? this.cooldownStackType.Value : ModSettings.BoxOfDynamiteCooldownStacking.Linear;

			LookingGlassCompat.AddItem(DLC3Content.Items.DronesDropDynamite, [
				new("Damage",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.Percentage,
					itemCount => ItemStacking.Linear(damageBase(), damageStack(), itemCount)),
				new("Cooldown",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Seconds,
					itemCount => cooldownBase() * cooldownStackType() switch {
						ModSettings.BoxOfDynamiteCooldownStacking.Linear
							=> ItemStacking.Linear(1f, -cooldownStack(), itemCount),

						ModSettings.BoxOfDynamiteCooldownStacking.Reciprocal
							=> ItemStacking.Reciprocal(0f, cooldownStack(), itemCount),

						ModSettings.BoxOfDynamiteCooldownStacking.Exponential
							=> ItemStacking.Exponential(1f - cooldownStack(), itemCount - 1),

						_ => 1f,
					}),
			]);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float damageBase                    = enabled ? this.damageBase.Value : VANILLA_DAMAGE_BASE_PERCENT;
			float damageStack                   = enabled ? this.damageStack.Value : VANILLA_DAMAGE_STACK_PERCENT;
			float cooldownBase                  = enabled ? this.cooldownBase.Value : VANILLA_COOLDOWN_BASE;
			float cooldownStack                 = enabled ? this.cooldownStack.Value : 0f;
			ItemStacking.Type cooldownStackType = enabled ? ItemStacking.FromEnum(this.cooldownStackType.Value) : ItemStacking.Type.Linear;

			LanguageAPI.AddOverlay("ITEM_DRONESDROPDYNAMITE_DESC",
				$"Gain {ItD.Damage("Lt.Droneboy")}. While in combat, your drones drop sticks of dynamite that detonate for " +
				$"{ItD.Damage($"{damageBase}% damage{ItD.Stack(damageStack, "+{0:0.#}%")}")}, stunning enemies. " +
				$"Recharges after {ItD.Utility(cooldownBase)} seconds{ItD.Stack(cooldownStack, cooldownStackType, "{0:-0.##;-0.##}%")}.");

			ItemQualitiesCompat.ApplyQualityDescriptions("DRONESDROPDYNAMITE", []);
		}

		protected override void ApplyHooks() {
		}
	}
}
