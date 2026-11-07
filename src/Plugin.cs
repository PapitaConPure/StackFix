using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using StackFix.Items;
using StackFix.Settings;
using StackFix.Utils;
using System;

namespace StackFix {
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
	public sealed class Plugin: BaseUnityPlugin {
		internal static ConfigEntry<bool> Enabled;

		internal static ConfigEntry<bool> EclipseLiteFixEnabled;
		internal static ConfigEntry<float> EclipseLiteBarrierBase;
		internal static ConfigEntry<float> EclipseLiteBarrierStack;
		internal static ConfigEntry<ItemStacking.Type> EclipseLiteBarrierStackType;

		internal static ConfigEntry<bool> FaradaySpurFixEnabled;
		internal static ConfigEntry<float> FaradaySpurMoveSpeedBase;
		internal static ConfigEntry<float> FaradaySpurMoveSpeedStack;
		internal static ConfigEntry<float> FaradaySpurJumpHeightBase;
		internal static ConfigEntry<float> FaradaySpurJumpHeightStack;
		internal static ConfigEntry<float> FaradaySpurDamageBase;
		internal static ConfigEntry<float> FaradaySpurDamageStack;

		internal static ConfigEntry<bool> NetworkedSufferingFixEnabled;
		internal static ConfigEntry<int> NetworkedSufferingSpreadBase;
		internal static ConfigEntry<int> NetworkedSufferingSpreadStack;
		internal static ConfigEntry<float> NetworkedSufferingDamageBase;
		internal static ConfigEntry<float> NetworkedSufferingDamageStack;



		internal static ConfigEntry<bool> Megumin;

		private readonly IItemFix[] ItemFixes = [
			new EclipseLite(),
			new FaradaySpur(),
			new NetworkedSuffering(),
		];

		public void Awake() {
			this.RegisterSettings();
			this.TryRegisterRiskOfOptions();
			this.ApplyItemFixes();

#if DEBUG
			this.Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} loaded.");
#endif
		}

		private void RegisterSettings() {
			Enabled                      = ModSettings.Enabled.RegisterFor(this);

			EclipseLiteFixEnabled       = ModSettings.EclipseLiteFixEnabled.RegisterFor(this);
			EclipseLiteBarrierBase      = ModSettings.EclipseLiteBarrierBase.RegisterFor(this);
			EclipseLiteBarrierStack     = ModSettings.EclipseLiteBarrierStack.RegisterFor(this);
			EclipseLiteBarrierStackType = ModSettings.EclipseLiteBarrierStackType.RegisterFor(this);

			FaradaySpurFixEnabled      = ModSettings.FaradaySpurFixEnabled.RegisterFor(this);
			FaradaySpurMoveSpeedBase   = ModSettings.FaradaySpurMoveSpeedBase.RegisterFor(this);
			FaradaySpurMoveSpeedStack  = ModSettings.FaradaySpurMoveSpeedStack.RegisterFor(this);
			FaradaySpurJumpHeightBase  = ModSettings.FaradaySpurJumpHeightBase.RegisterFor(this);
			FaradaySpurJumpHeightStack = ModSettings.FaradaySpurJumpHeightStack.RegisterFor(this);
			FaradaySpurDamageBase      = ModSettings.FaradaySpurDamageBase.RegisterFor(this);
			FaradaySpurDamageStack     = ModSettings.FaradaySpurDamageStack.RegisterFor(this);

			NetworkedSufferingFixEnabled  = ModSettings.NetworkedSufferingFixEnabled.RegisterFor(this);
			NetworkedSufferingSpreadBase  = ModSettings.NetworkedSufferingSpreadBase.RegisterFor(this);
			NetworkedSufferingSpreadStack = ModSettings.NetworkedSufferingSpreadStack.RegisterFor(this);
			NetworkedSufferingDamageBase  = ModSettings.NetworkedSufferingDamageBase.RegisterFor(this);
			NetworkedSufferingDamageStack = ModSettings.NetworkedSufferingDamageStack.RegisterFor(this);

			Megumin = ModSettings.Megumin.RegisterFor(this);
		}

		private void TryRegisterRiskOfOptions() {
			if(!RiskOfOptionsCompatManager.Enabled)
				return;

			try {
				RiskOfOptionsCompatManager.SetModDescription(
					"Balances the initial/additional stack behavior of a few items. Configurable.");
				RiskOfOptionsCompatManager.SetModIcon(ModIcon.Load());

				RiskOfOptionsCompatManager.AddCheckboxOption(Enabled);

				RiskOfOptionsCompatManager.AddCheckboxOption(EclipseLiteFixEnabled);
				RiskOfOptionsCompatManager.AddSliderOption(EclipseLiteBarrierBase, new(FormatString: "{0:0.##}%"));
				RiskOfOptionsCompatManager.AddSliderOption(EclipseLiteBarrierStack, new(FormatString: "{0:0.##}%"));
				RiskOfOptionsCompatManager.AddChoiceOption(EclipseLiteBarrierStackType);

				RiskOfOptionsCompatManager.AddCheckboxOption(FaradaySpurFixEnabled);
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurMoveSpeedBase, new(FormatString: "{0:0.##}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurMoveSpeedStack, new(FormatString: "{0:0.##}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurJumpHeightBase, new(FormatString: "{0:0.##}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurJumpHeightStack, new(FormatString: "{0:0.##}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurDamageBase, new(FormatString: "{0:0.##}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurDamageStack, new(FormatString: "{0:0.##}%", Step: 0.5f));

				RiskOfOptionsCompatManager.AddCheckboxOption(NetworkedSufferingFixEnabled);
				RiskOfOptionsCompatManager.AddSliderOption(NetworkedSufferingSpreadBase);
				RiskOfOptionsCompatManager.AddSliderOption(NetworkedSufferingSpreadStack);
				RiskOfOptionsCompatManager.AddSliderOption(NetworkedSufferingDamageBase, new(FormatString: "{0:0.##}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(NetworkedSufferingDamageStack, new(FormatString: "{0:0.##}%"));

				RiskOfOptionsCompatManager.AddCheckboxOption(Megumin);
			} catch(Exception exception) {
				this.Logger.LogWarning($"Could not register Risk of Options settings: {exception}");
			}
		}

		private void ApplyItemFixes() {
			foreach(IItemFix itemFix in this.ItemFixes)
				itemFix.ApplyFix();
		}
	}
}
