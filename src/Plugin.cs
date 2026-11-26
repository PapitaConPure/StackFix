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
using System.Diagnostics;

namespace StackFix {
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	[BepInDependency(RiskOfOptionsCompat.GUID, BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency(LookingGlassCompat.GUID, BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency(ItemQualitiesCompat.GUID, BepInDependency.DependencyFlags.SoftDependency)]
	public sealed class Plugin: BaseUnityPlugin {
		internal static ConfigEntry<bool> Enabled;
		internal static ConfigEntry<bool> ShowStackingType;
		internal static ConfigEntry<bool> Megumin;

		private static Plugin instance;
		public static Plugin Instance => instance;

		internal static BepInEx.Logging.ManualLogSource Logging => Instance.Logger;

		private readonly IItemFix[] ItemFixes = [
			//White
			new BolsteringLantern(),
			new ChronicExpansion(),
			new EclipseLite(),
			new WarpedEcho(),
			//Green
			new BoxOfDynamite(),
			new BreachingFin(),
			new ChanceDoll(),
			new FaradaySpur(),
			new LuminousShot(),
			new PrayerBeads(),
			//Red
			new NetworkedSuffering(),
			new SonorousWhispers(),
		];

		public void Awake() {
			SingletonHelper.Assign(ref instance, this);

			Stopwatch stopwatch = Stopwatch.StartNew();

			this.MeasureAction("Register Settings", this.RegisterSettings);
			this.MeasureAction("Apply Item Fixes", this.TryApplyItemFixes);
			this.ListenToExtraEvents();

			this.TryRegisterRiskOfOptions();
			this.TryRegisterLookingGlass();

			stopwatch.Stop();
			this.Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} loaded in {stopwatch.ElapsedMilliseconds}ms.");
		}

		public void OnDestroy() {
			foreach(IItemFix itemFix in this.ItemFixes)
				if(itemFix is IDisposable toBeDisposed)
					toBeDisposed.Dispose();

			SingletonHelper.Unassign(ref instance, this);
		}

		private void RegisterSettings() {
			Enabled = ModSettings.Enabled.RegisterFor(this);
			ShowStackingType = ModSettings.ShowStackingType.RegisterFor(this);

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

#if DEBUG
			this.Logger.LogDebug("Risk Of Options is installed. Trying to initialize support...");
#endif

			try {
				RiskOfOptionsCompat.SetModDescription(
					"Balances the initial effect and stacking of a few items to reduce upfront power spikes and incentivize stacking.");
				RiskOfOptionsCompat.SetModIcon(ModIcon.Load());

				RiskOfOptionsCompat.AddCheckboxOption(Enabled);
				RiskOfOptionsCompat.AddCheckboxOption(ShowStackingType);

				foreach(IItemFix itemFix in this.ItemFixes)
					if(itemFix.Applied)
						itemFix.RegisterRiskOfOptionsSettings();

				RiskOfOptionsCompat.AddCheckboxOption(Megumin);

#if DEBUG
				this.Logger.LogDebug("Risk Of Options support initialized!");
#endif
			} catch(Exception ex) {
				this.Logger.LogWarning($"Could not register Risk Of Options settings: {ex}");
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

#if DEBUG
					this.Logger.LogDebug("LookingGlass support initialized!");
#endif
				} catch(Exception ex) {
					this.Logger.LogWarning($"Could not initialize LookingGlass support: {ex}");
				}
			});
		}

		private void ListenToExtraEvents() {
			Enabled.SettingChanged += this.Enabled_SettingChanged;
		}

		private void Enabled_SettingChanged(object sender, EventArgs e) {
			if(Run.instance)
				PickupDropTable.RegenerateAll(Run.instance);
		}

		private void MeasureAction(string name, Action action) {
#if DEBUG
			Stopwatch actionTime = Stopwatch.StartNew();
			action();
			actionTime.Stop();

			this.Logger.LogInfo($"Completed action: '{name}', in {actionTime.ElapsedMilliseconds}ms.");
#else
			action();
#endif
		}
	}
}
