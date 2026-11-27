using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> NetworkedSufferingFixEnabled = new(
			section: "General",
			name: "Fix Networked Suffering",
			description: Paragraph(
				"Enables Networked Suffering tweaks.",
				Vanilla(false)),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> NetworkedSufferingSpreadBase = new(
			section: "Item: Networked Suffering",
			name: "Base Virus Spread",
			description: Paragraph(
				"Maximum number of infected enemies for the first stack.",
				Vanilla(4)),
			defaultValue: 3,
			constraint: new(Min: 2, Max: 8));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> NetworkedSufferingSpreadStack = new(
			section: "Item: Networked Suffering",
			name: "Virus Spread per Stack",
			description: Paragraph(
				"Increased maximum number of infected enemies for every subsequent stack.",
				Vanilla(2)),
			defaultValue: 2,
			constraint: new(Min: 0, Max: 4));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> NetworkedSufferingDamageBase = new(
			section: "Item: Networked Suffering",
			name: "Base Damage",
			description: Paragraph(
				"Virus damage percentage for the first stack.",
				Vanilla(45f)),
			defaultValue: 35f,
			constraint: new(Min: 5f, Max: 50f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> NetworkedSufferingDamageStack = new(
			section: "Item: Networked Suffering",
			name: "Damage per Stack",
			description: Paragraph(
				"Increased virus damage percentage for every subsequent stack.",
				Vanilla(0f)),
			defaultValue: 1f,
			constraint: new(Min: 0f, Max: 10f));
	}
}
