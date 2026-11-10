using BepInEx;
using BepInEx.Configuration;
using StackFix.Compat;
using StackFix.Settings;
using System;

namespace StackFix.Items.Impl {
	internal class ChanceDoll: ItemFix {
		private const float VANILLA_CHANCE_BASE = 0.4f;
		private const float VANILLA_CHANCE_STACK = 0.1f;

		private ConfigEntry<float> chanceBase;
		private ConfigEntry<float> chanceStack;
		private ConfigEntry<ModSettings.EclipseLiteBarrierStacking> chanceStackType;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled      = this.RegisterSetting(plugin, ModSettings.EclipseLiteFixEnabled);
			this.chanceBase      = this.RegisterSetting(plugin, ModSettings.EclipseLiteBarrierBase);
			this.chanceStack     = this.RegisterSetting(plugin, ModSettings.EclipseLiteBarrierStack);
			this.chanceStackType = this.RegisterSetting(plugin, ModSettings.EclipseLiteBarrierStackType);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompatManager.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompatManager.AddSliderOption(this.chanceBase, new(Suffix: "%"));
			RiskOfOptionsCompatManager.AddSliderOption(this.chanceStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompatManager.AddChoiceOption(this.chanceStackType);
		}

		protected override void ApplyDescription() {
			throw new NotImplementedException();
		}

		protected override void ApplyHooks() {
			throw new NotImplementedException();
		}
	}
}
