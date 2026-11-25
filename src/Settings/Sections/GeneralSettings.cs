using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> Enabled = new(
			section: "General",
			name: "Enable Mod",
			description: Paragraph(
				"Enables the mod.",
				"",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConfigEntryExt<bool> ShowStackingType = new(
			section: "General",
			name: "Display Stacking Type",
			description: Paragraph(
				"Determines whether to display the stacking type when it differs from the vanilla stacking type.",
				"",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConfigEntryExt<bool> Megumin = new(
			section: "Megumin",
			name: "Megumin",
			description: "Megumin",
			defaultValue: true);
	}
}
