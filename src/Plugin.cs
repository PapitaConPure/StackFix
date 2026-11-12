using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using StackFix.Compat;
using StackFix.Exceptions;
using StackFix.Items;
using StackFix.Items.Impl;
using StackFix.Settings;
using StackFix.Utils;
using System;

namespace StackFix {
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	[BepInDependency(RiskOfOptionsCompat.GUID, BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency(LookingGlassCompat.GUID, BepInDependency.DependencyFlags.SoftDependency)]
	public sealed class Plugin: BaseUnityPlugin {
		internal static ConfigEntry<bool> Enabled;
		internal static ConfigEntry<bool> Megumin;

		private readonly IItemFix[] ItemFixes = [
			//White
			new BolsteringLantern(),
			new EclipseLite(),
			new WarpedEcho(),
			//Green
			new ChanceDoll(),
			new FaradaySpur(),
			new PrayerBeads(),
			//Red
			new NetworkedSuffering(),
		];

		public void Awake() {
			this.RegisterSettings();
			this.TryApplyItemFixes();
			this.TryRegisterRiskOfOptions();
			this.TryRegisterLookingGlass();

#if DEBUG
			this.Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} loaded.");
#endif
		}

		public void OnDestroy() {
			foreach(IItemFix itemFix in this.ItemFixes)
				if(itemFix is IDisposable toBeDisposed)
					toBeDisposed.Dispose();
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
				} catch(MissingMethodException ex) {
					this.Logger.LogError(ex);
					this.Logger.LogWarning($"Item fix '{itemFix}' was skipped because of a missing IL method.");
				}
		}

		private void TryRegisterRiskOfOptions() {
			if(!RiskOfOptionsCompat.Installed)
				return;

			try {
				RiskOfOptionsCompat.SetModDescription(
					"Balances the initial/additional stack behavior of a few items. Configurable.");
				RiskOfOptionsCompat.SetModIcon(ModIcon.Load());

				RiskOfOptionsCompat.AddCheckboxOption(Enabled);

				foreach(IItemFix itemFix in this.ItemFixes)
					if(itemFix.Applied)
						itemFix.RegisterRiskOfOptionsSettings();

				RiskOfOptionsCompat.AddCheckboxOption(Megumin);
			} catch(Exception ex) {
				this.Logger.LogWarning($"Could not register Risk of Options settings: {ex}");
			}
		}

		private void TryRegisterLookingGlass() {
			if(!LookingGlassCompat.Installed)
				return;

#if DEBUG
			this.Logger.LogDebug("LookingGlass is installed and support will be initialized when items catalog is ready.");
#endif

			ItemCatalog.availability.CallWhenAvailable(() => {
				try {
#if DEBUG
					this.Logger.LogDebug("Trying to initialize LookingGlass support...");
#endif

					foreach(IItemFix itemFix in this.ItemFixes)
						if(itemFix.Applied)
							itemFix.RegisterLookingGlassStats();
				} catch(Exception ex) {
					this.Logger.LogWarning($"Could not initialize LookingGlass support: {ex}");
				}
			});
		}
	}
}
