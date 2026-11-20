using BepInEx;
using StackFix.Settings;

namespace StackFix.Items.Impl {
	internal sealed class BoxOfDynamite: ItemFix {

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteFixEnabled);

		}

		public override void RegisterRiskOfOptionsSettings() {
		}

		public override void RegisterLookingGlassStats() {
		}

		protected override void ApplyDescription() {
		}

		protected override void ApplyHooks() {
		}
	}
}
