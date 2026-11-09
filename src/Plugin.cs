using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using StackFix.Exceptions;
using StackFix.Items;
using StackFix.Settings;
using StackFix.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StackFix {
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
	public sealed class Plugin: BaseUnityPlugin {
		internal static ConfigEntry<bool> Enabled;
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
			this.TryApplyItemFixes();
			this.TryRegisterRiskOfOptions();

#if DEBUG
			this.Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} loaded.");
#endif
		}

		private void RegisterSettings() {
			Enabled                      = ModSettings.Enabled.RegisterFor(this);

			foreach(IItemFix itemFix in this.ItemFixes)
				itemFix.RegisterSettings(this);

			Megumin = ModSettings.Megumin.RegisterFor(this);
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

		private void TryRegisterRiskOfOptions() {
			if(!RiskOfOptionsCompatManager.Enabled)
				return;

			try {
				RiskOfOptionsCompatManager.SetModDescription(
					"Balances the initial/additional stack behavior of a few items. Configurable.");
				RiskOfOptionsCompatManager.SetModIcon(ModIcon.Load());

				RiskOfOptionsCompatManager.AddCheckboxOption(Enabled);

				foreach(IItemFix itemFix in this.ItemFixes)
					if(itemFix.Applied)
						itemFix.RegisterRiskOfOptionsSettings();

				RiskOfOptionsCompatManager.AddCheckboxOption(Megumin);
			} catch(Exception exception) {
				this.Logger.LogWarning($"Could not register Risk of Options settings: {exception}");
			}
		}
	}
}
