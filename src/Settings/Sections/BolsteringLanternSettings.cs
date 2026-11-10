using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {

		public static readonly ConfigEntryExt<bool> BolsteringLanternFixEnabled = new(
			section: "General",
			name: "Fix Bolstering Lantern",
			description: Paragraph(
				"Enables Bolstering Lantern tweaks.",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BolsteringLanternAttackSpeedBase = new(
			section: "Item: Bolstering Lantern",
			name: "Base Attack Speed",
			description: Paragraph(
				"Percentage of attack speed each buff stack grants on the first item stack.",
				"Vanilla value: 6.5"),
			defaultValue: 6f,
			constraint: new(Min: 4f, Max: 8f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BolsteringLanternAttackSpeedStack = new(
			section: "Item: Bolstering Lantern",
			name: "Attack Speed per Stack",
			description: Paragraph(
				"Additional percentage of attack speed each buff stack grants for every subsequent stack.",
				"Vanilla value: 3.5"),
			defaultValue: 4f,
			constraint: new(Min: 2f, Max: 6f));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> BolsteringLanternCharacterCountMaxBase = new(
			section: "Item: Bolstering Lantern",
			name: "Base Max Character Count",
			description: Paragraph(
				"Maximum number of close allies and enemies that count towards the buff for the first stack.",
				"Vanilla value: 3"),
			defaultValue: 3,
			constraint: new(Min: 2, Max: 6));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> BolsteringLanternCharacterCountMaxStack = new(
			section: "Item: Bolstering Lantern",
			name: "Max Character Count per Stack",
			description: Paragraph(
				"Increased maximum number of close allies and enemies that count towards the buff for every subsequent stack.",
				"Vanilla value: 1"),
			defaultValue: 1,
			constraint: new(Min: 0, Max: 3));
	}
}
