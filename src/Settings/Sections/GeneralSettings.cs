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
			name: "Display Non-Vanilla Stacking Types",
			description: Paragraph(
				"Determines whether to display the configured stacking type for any setting with a configurable stacking formula when it differs from vanilla.",
				"",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConfigEntryExt<string> StackingFormat = new(
			section: "General",
			name: "Stacking Type Display Format",
			description: Paragraph(
				"Template used to display non-vanilla stacking types.",
				"",
				"* {NUMBER}: the number that scales per stack, e.g. \"+20%\", \"+2\", \"-50%\"",
				"* {TYPE_LOWER}: the stacking type in lowercase, e.g. \"linear\", \"reciprocal\"",
				"* {TYPE_UPPER}: the stacking type in uppercase, e.g. \"HYPERBOLIC\"",
				"* {TYPE_CAP}: the stacking type w/ first letter capitalized, e.g. \"Exponential\"",
				"",
				"Vanilla value: \"({NUMBER} per stack)\""),
			defaultValue: "({NUMBER} per stack, {TYPE_LOWER})");

		public enum Meguminn {
			Megumin,
		};
		public static readonly ConfigEntryExt<Meguminn> Megumin = new(
			section: "Megumin",
			name: "Megumin",
			description: "Megumin",
			defaultValue: Meguminn.Megumin);
	}
}
