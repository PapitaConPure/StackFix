using BepInEx;
using BepInEx.Configuration;
using StackFix.Compat;
using StackFix.Settings;

namespace StackFix.Items.Impl {
	internal sealed class BoxOfDynamite: ItemFix {
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
		}

		protected override void ApplyDescription() {
		}

		protected override void ApplyHooks() {
		}
	}
}
