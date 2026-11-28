using ConfigEntryExt;
using StackFix.Utils;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> Enabled = new(
			section: "General",
			name: "Enable Mod",
			description: Paragraph(
				"Enables the mod.",
				Vanilla(false)),
			defaultValue: true);

		public static readonly ConfigEntryExt<bool> ShowStackingType = new(
			section: "General",
			name: "Display Non-Vanilla Stacking Types",
			description: Paragraph(
				"Determines whether to display the configured stacking type for any setting with a configurable stacking formula when it differs from vanilla.",
				Vanilla(false)),
			defaultValue: true);

		public static readonly ConfigEntryExt<string> StackingFormat = new(
			section: "General",
			name: "Stacking Type Display Format",
			description: Paragraph(
				"Template used to display non-vanilla stacking types.",
				"",
				$"* {ItD.Utility("{N}")}: the number that scales per stack, e.g. \"+20%\", \"+1\", \"-44.5%\"",
				$"* {ItD.Utility("{TL}")}: the stacking type in lowercase, e.g. \"linear\", \"reciprocal\"",
				$"* {ItD.Utility("{TU}")}: the stacking type in uppercase, e.g. \"HYPERBOLIC\"",
				$"* {ItD.Utility("{TC}")}: the stacking type w/ first letter capitalized, e.g. \"Exponential\"",
				Vanilla($"({ItD.Utility("{N}")} per stack)")),
			defaultValue: "({N} per stack, {TL})");

		public enum Megu {
			Megumin,
		};
		public static readonly ConfigEntryExt<Megu> Megumin = new(
			section: "Megumin",
			name: "Megumin",
			description: "Megumin",
			defaultValue: Megu.Megumin);
	}
}
