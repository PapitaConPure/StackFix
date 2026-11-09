using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using StackFix.Exceptions;
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

		internal static ConfigEntry<bool> BolsteringLanternFixEnabled;
		internal static ConfigEntry<float> BolsteringLanternAttackSpeedBase;
		internal static ConfigEntry<float> BolsteringLanternAttackSpeedStack;
		internal static ConfigEntry<int> BolsteringLanternCharacterCountMaxBase;
		internal static ConfigEntry<int> BolsteringLanternCharacterCountMaxStack;

		internal static ConfigEntry<bool> EclipseLiteFixEnabled;
		internal static ConfigEntry<float> EclipseLiteBarrierBase;
		internal static ConfigEntry<float> EclipseLiteBarrierStack;
		internal static ConfigEntry<ModSettings.EclipseLiteBarrierStacking> EclipseLiteBarrierStackType;

		internal static ConfigEntry<bool> WarpedEchoFixEnabled;
		internal static ConfigEntry<float> WarpedEchoDamageReductionBase;
		internal static ConfigEntry<float> WarpedEchoDamageReductionStack;
		internal static ConfigEntry<ModSettings.WarpedEchoDamageReductionStacking> WarpedEchoDamageReductionStackType;
		internal static ConfigEntry<int> WarpedEchoDamageSpreadBase;
		internal static ConfigEntry<int> WarpedEchoDamageSpreadStack;
		internal static ConfigEntry<float> WarpedEchoCooldownBase;
		internal static ConfigEntry<float> WarpedEchoCooldownStack;
		internal static ConfigEntry<ModSettings.WarpedEchoCooldownStacking> WarpedEchoCooldownStackType;

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
			new BolsteringLantern(),
			new EclipseLite(),
			new WarpedEcho(),
			new FaradaySpur(),
			new NetworkedSuffering(),
		];

		public void Awake() {
			this.RegisterSettings();
			this.TryRegisterRiskOfOptions();
			this.TryApplyItemFixes();

#if DEBUG
			this.Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} loaded.");
#endif
		}

		private void RegisterSettings() {
			Enabled                      = ModSettings.Enabled.RegisterFor(this);

			BolsteringLanternFixEnabled             = ModSettings.BolsteringLanternFixEnabled.RegisterFor(this);
			BolsteringLanternAttackSpeedBase        = ModSettings.BolsteringLanternAttackSpeedBase.RegisterFor(this);
			BolsteringLanternAttackSpeedStack       = ModSettings.BolsteringLanternAttackSpeedStack.RegisterFor(this);
			BolsteringLanternCharacterCountMaxBase  = ModSettings.BolsteringLanternCharacterCountMaxBase.RegisterFor(this);
			BolsteringLanternCharacterCountMaxStack = ModSettings.BolsteringLanternCharacterCountMaxStack.RegisterFor(this);

			EclipseLiteFixEnabled       = ModSettings.EclipseLiteFixEnabled.RegisterFor(this);
			EclipseLiteBarrierBase      = ModSettings.EclipseLiteBarrierBase.RegisterFor(this);
			EclipseLiteBarrierStack     = ModSettings.EclipseLiteBarrierStack.RegisterFor(this);
			EclipseLiteBarrierStackType = ModSettings.EclipseLiteBarrierStackType.RegisterFor(this);

			WarpedEchoFixEnabled               = ModSettings.WarpedEchoFixEnabled.RegisterFor(this);
			WarpedEchoDamageReductionBase      = ModSettings.WarpedEchoDamageReductionBase.RegisterFor(this);
			WarpedEchoDamageReductionStack     = ModSettings.WarpedEchoDamageReductionStack.RegisterFor(this);
			WarpedEchoDamageReductionStackType = ModSettings.WarpedEchoDamageReductionStackType.RegisterFor(this);
			WarpedEchoDamageSpreadBase         = ModSettings.WarpedEchoDamageSpreadBase.RegisterFor(this);
			WarpedEchoDamageSpreadStack        = ModSettings.WarpedEchoDamageSpreadStack.RegisterFor(this);
			WarpedEchoCooldownBase             = ModSettings.WarpedEchoCooldownBase.RegisterFor(this);
			WarpedEchoCooldownStack            = ModSettings.WarpedEchoCooldownStack.RegisterFor(this);
			WarpedEchoCooldownStackType        = ModSettings.WarpedEchoCooldownStackType.RegisterFor(this);

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

				RiskOfOptionsCompatManager.AddCheckboxOption(BolsteringLanternFixEnabled);
				RiskOfOptionsCompatManager.AddSliderOption(BolsteringLanternAttackSpeedBase, new(Suffix: "%"));
				RiskOfOptionsCompatManager.AddSliderOption(BolsteringLanternAttackSpeedStack, new(FormatString: "+{0:0.##}%"));
				RiskOfOptionsCompatManager.AddSliderOption(BolsteringLanternCharacterCountMaxBase);
				RiskOfOptionsCompatManager.AddSliderOption(BolsteringLanternCharacterCountMaxStack, new(Prefix: "+"));

				RiskOfOptionsCompatManager.AddCheckboxOption(EclipseLiteFixEnabled);
				RiskOfOptionsCompatManager.AddSliderOption(EclipseLiteBarrierBase, new(Suffix: "%"));
				RiskOfOptionsCompatManager.AddSliderOption(EclipseLiteBarrierStack, new(Prefix: "+", Suffix: "%"));
				RiskOfOptionsCompatManager.AddChoiceOption(EclipseLiteBarrierStackType);

				RiskOfOptionsCompatManager.AddCheckboxOption(WarpedEchoFixEnabled);
				RiskOfOptionsCompatManager.AddSliderOption(WarpedEchoDamageReductionBase, new(Suffix: "%"));
				RiskOfOptionsCompatManager.AddSliderOption(WarpedEchoDamageReductionStack, new(Prefix: "+", Suffix: "%"));
				RiskOfOptionsCompatManager.AddChoiceOption(WarpedEchoDamageReductionStackType);
				RiskOfOptionsCompatManager.AddSliderOption(WarpedEchoDamageSpreadBase);
				RiskOfOptionsCompatManager.AddSliderOption(WarpedEchoDamageSpreadStack, new(Prefix: "+"));
				RiskOfOptionsCompatManager.AddSliderOption(WarpedEchoCooldownBase, new(FormatString: "{0:0.#}s", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(WarpedEchoCooldownStack, new(Prefix: "+", Suffix: "%"));
				RiskOfOptionsCompatManager.AddChoiceOption(WarpedEchoCooldownStackType);

				RiskOfOptionsCompatManager.AddCheckboxOption(FaradaySpurFixEnabled);
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurMoveSpeedBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurMoveSpeedStack, new(FormatString: "+{0:0.#}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurJumpHeightBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurJumpHeightStack, new(FormatString: "+{0:0.#}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurDamageBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(FaradaySpurDamageStack, new(FormatString: "+{0:0.#}%", Step: 0.5f));

				RiskOfOptionsCompatManager.AddCheckboxOption(NetworkedSufferingFixEnabled);
				RiskOfOptionsCompatManager.AddSliderOption(NetworkedSufferingSpreadBase);
				RiskOfOptionsCompatManager.AddSliderOption(NetworkedSufferingSpreadStack, new(Prefix: "+"));
				RiskOfOptionsCompatManager.AddSliderOption(NetworkedSufferingDamageBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
				RiskOfOptionsCompatManager.AddSliderOption(NetworkedSufferingDamageStack, new(Prefix: "+", Suffix: "%"));

				RiskOfOptionsCompatManager.AddCheckboxOption(Megumin);
			} catch(Exception exception) {
				this.Logger.LogWarning($"Could not register Risk of Options settings: {exception}");
			}
		}

		private void TryApplyItemFixes() {
			foreach(IItemFix itemFix in this.ItemFixes)
				try {
					itemFix.ApplyFix();
				} catch(ILSeekException ex) {
					this.Logger.LogError(ex);
					this.Logger.LogWarning($"Item fix '{itemFix}' was skipped because of an unresolved IL mismatch.");
				}
		}
	}
}
