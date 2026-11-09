using BepInEx;
using BepInEx.Configuration;
using System;

namespace ConfigEntryExt {
	public class ConfigEntryExt<TValue>(
		string section,
		string name,
		string description,
		TValue defaultValue)
		: IConfigEntryChangeObserver
		where TValue : IComparable {
		public string Section { get; } = section;

		public string Name { get; } = name;

		public string Description { get; } = description;

		public TValue Default { get; } = defaultValue;

		public ConfigEntry<TValue> ConfigEntry { get; private set; }

		public ConfigEntry<TValue> RegisterFor(BaseUnityPlugin plugin)
			=> this.ConfigEntry = this.MaterializeAndBind(plugin);

		protected virtual ConfigEntry<TValue> MaterializeAndBind(BaseUnityPlugin plugin)
			=> plugin.Config.Bind(
				this.Section, this.Name, this.Default,
				new ConfigDescription(this.Description, null));

		public void ObserveConfigEntryChanged(EventHandler handler) {
			this.ConfigEntry.SettingChanged += handler;
		}
	}
}
