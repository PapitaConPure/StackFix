using BepInEx;

namespace StackFix.Settings {
	public interface ISettingsRegistrar {
		public void RegisterSettings(BaseUnityPlugin plugin);
		public void RegisterRiskOfOptionsSettings();
	}
}
