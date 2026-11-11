using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> ChanceDollFixEnabled = new(
			section: "General",
			name: "Fix Chance Doll",
			description: Paragraph(
				"Enables Chance Doll tweaks.",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> ChanceDollChanceBase = new(
			section: "Item: Chance Doll",
			name: "Base Chance",
			description: Paragraph(
				"Improved reward chance percentage for the first stack.",
				"Vanilla value: 40"),
			defaultValue: 25f,
			constraint: new(Min: 10f, Max: 40f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> ChanceDollChanceStack = new(
			section: "Item: Chance Doll",
			name: "Chance per Stack",
			description: Paragraph(
				"Improved reward additional chance percentage for every subsequent stack.",
				"Vanilla value: 10"),
			defaultValue: 25f,
			constraint: new(Min: 5f, Max: 40f));

		public enum ChanceDollChanceStacking {
			Linear,
			Hyperbolic,
		}
		public static readonly ConstrainedConfigEntry<ChanceDollChanceStacking, ListEntryConstraint<ChanceDollChanceStacking>> ChanceDollChanceStackType = new(
			section: "Item: Chance Doll",
			name: "Chance Stacking Type",
			description: Paragraph(
				StackTypeDescription<ChanceDollChanceStacking>(),
				"Vanilla value: Linear"),
			defaultValue: ChanceDollChanceStacking.Hyperbolic,
			constraint: new(typeof(ChanceDollChanceStacking)));
	}
}
