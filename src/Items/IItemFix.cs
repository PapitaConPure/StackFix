using RoR2;
using StackFix.Settings;

namespace StackFix.Items {
	internal interface IItemFix: ISettingsRegistrar {
		public bool Applied { get; }
		public void ApplyFix();
		public void RegisterLookingGlassStats();
	}
}
