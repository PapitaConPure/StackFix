using BepInEx;

namespace StackFix.Items {
	internal interface IItemFix {
		public bool Applied { get; }
		public void ApplyFix();
		public void RegisterSettings(BaseUnityPlugin plugin);
		public void RegisterRiskOfOptionsSettings();
	}
}
