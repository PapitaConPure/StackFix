using BepInEx;
using BepInEx.Configuration;
using ConfigEntryExt;
using System;
using System.Collections.Generic;

namespace StackFix.Items {
	internal abstract class ItemFix: IItemFix {
		protected ConfigEntry<bool> fixEnabled;
		private readonly List<Action> extraFixes = [];

		public bool Enabled => Plugin.Enabled.Value && this.fixEnabled.Value;

		public bool Applied { get; private set; }

		private readonly List<IConfigEntryChangeObserver> itemFixConfigEntries = [];

		public abstract void RegisterSettings(BaseUnityPlugin plugin);

		protected ConfigEntry<T> RegisterSetting<T>(BaseUnityPlugin plugin, ConfigEntryExt<T> configEntry) where T : IComparable {
			this.itemFixConfigEntries.Add(configEntry);
			return configEntry.RegisterFor(plugin);
		}

#if DEBUG
		private static int fixCount = 0;
#endif

		public void ApplyFix() {
#if DEBUG
			fixCount++;
			Plugin.Logging.LogDebug($"Applying fix #{fixCount} hooks...");
#endif
			this.ApplyHooks();

#if DEBUG
			Plugin.Logging.LogDebug($"Applying fix #{fixCount} description...");
#endif
			this.ApplyDescription();

#if DEBUG
			Plugin.Logging.LogDebug($"Applying fix #{fixCount} extras...");
#endif
			this.ApplyExtras();

#if DEBUG
			Plugin.Logging.LogDebug($"Registering fix #{fixCount} config entries...");
#endif
			Plugin.Enabled.SettingChanged          += this.ConfigEntry_SettingChanged;
			Plugin.ShowStackingType.SettingChanged += this.ConfigEntry_SettingChanged;
			Plugin.StackingFormat.SettingChanged   += this.ConfigEntry_SettingChanged;

			foreach(IConfigEntryChangeObserver configEntry in this.itemFixConfigEntries)
				configEntry.ObserveConfigEntryChanged(this.ConfigEntry_SettingChanged);

			this.itemFixConfigEntries.Clear();

#if DEBUG
			Plugin.Logging.LogDebug($"Fix #{fixCount} has been applied.");
#endif
			this.Applied = true;
		}

		private void ApplyExtras() {
			foreach(Action fix in this.extraFixes)
				fix.Invoke();
		}

		protected void AddExtraFix(Action extra) {
			this.extraFixes.Add(extra);
		}

		private void ConfigEntry_SettingChanged(object sender, EventArgs e) => this.ApplyDescription();

		protected abstract void ApplyDescription();

		protected abstract void ApplyHooks();

		public abstract void RegisterRiskOfOptionsSettings();

		public abstract void RegisterLookingGlassStats();
	}
}
