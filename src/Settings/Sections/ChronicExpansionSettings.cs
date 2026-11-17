using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> ChronicExpansionFixEnabled = new(
			section: "General",
			name: "Fix Chronic Expansion",
			description: Paragraph(
				"Enables Chronic Expansion tweaks.",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> ChronicExpansionDamageBase = new(
			section: "Item: Chronic Expansion",
			name: "Base Damage",
			description: Paragraph(
				"Percentage of damage each buff stack grants on the first item stack.",
				"Vanilla value: 3.5"),
			defaultValue: 3f,
			constraint: new(Min: 2f, Max: 4f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> ChronicExpansionDamageStack = new(
			section: "Item: Chronic Expansion",
			name: "Damage per Stack",
			description: Paragraph(
				"Additional percentage of damage each buff stack grants for every subsequent stack.",
				"Vanilla value: 1"),
			defaultValue: 1.5f,
			constraint: new(Min: 0f, Max: 2f));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> ChronicExpansionComboMaxBase = new(
			section: "Item: Chronic Expansion",
			name: "Base Max Combo",
			description: Paragraph(
				"Maximum combo the buff can reach on the first stack.",
				"Vanilla value: 10"),
			defaultValue: 10,
			constraint: new(Min: 6, Max: 12));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> ChronicExpansionComboMaxStack = new(
			section: "Item: Chronic Expansion",
			name: "Max Combo per Stack",
			description: Paragraph(
				"Increased maximum combo the buff can reach per every subsequent stack.",
				"Vanilla value: 5"),
			defaultValue: 2,
			constraint: new(Min: 0, Max: 6));
	}
}
