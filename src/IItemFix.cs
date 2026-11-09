using BepInEx;

namespace StackFix {
	internal interface IItemFix {
		public bool Enabled { get; }
		public bool Applied { get; }
		public void ApplyFix();
		public void RegisterSettings(BaseUnityPlugin plugin);
		public void RegisterRiskOfOptionsSettings();
	}
}
