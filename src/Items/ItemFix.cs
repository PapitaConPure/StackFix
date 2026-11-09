using BepInEx;
using BepInEx.Configuration;
using ConfigEntryExt;
using System;
using System.Collections.Generic;

namespace StackFix.Items {
	internal abstract class ItemFix: IItemFix {
		public abstract bool Enabled { get; }

		public bool Applied { get; private set; }


		private readonly List<IConfigEntryChangeObserver> itemFixConfigEntries = [];

		public abstract void RegisterSettings(BaseUnityPlugin plugin);

		protected ConfigEntry<T> RegisterSetting<T>(BaseUnityPlugin plugin, ConfigEntryExt<T> configEntry) where T : IComparable {
			this.itemFixConfigEntries.Add(configEntry);
			return configEntry.RegisterFor(plugin);
		}

		public void ApplyFix() {
			this.ApplyHooks();
			this.ApplyDescription();

			Plugin.Enabled.SettingChanged += this.ConfigEntry_SettingChanged;

			foreach(IConfigEntryChangeObserver configEntry in this.itemFixConfigEntries)
				configEntry.ObserveConfigEntryChanged(this.ConfigEntry_SettingChanged);

			this.itemFixConfigEntries.Clear();

			this.Applied = true;
		}

		private void ConfigEntry_SettingChanged(object sender, EventArgs e) => this.ApplyDescription();

		protected abstract void ApplyDescription();

		protected abstract void ApplyHooks();

		public abstract void RegisterRiskOfOptionsSettings();
	}
}
