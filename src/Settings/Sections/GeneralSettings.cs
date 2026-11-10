using ConfigEntryExt;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> Enabled = new(
			section: "General",
			name: "Enable Mod",
			description: Paragraph(
				"Enables the mod.",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConfigEntryExt<bool> Megumin = new(
			section: "Megumin",
			name: "Megumin",
			description: "Megumin",
			defaultValue: true);
	}
}
