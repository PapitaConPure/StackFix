using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> LuminousShotFixEnabled = new(
			section: "General",
			name: "Fix Luminous Shot",
			description: Paragraph(
				"Enables Luminous Shot tweaks.",
				"",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> LuminousShotDamageBase = new(
			section: "Item: Luminous Shot",
			name: "Base Damage",
			description: Paragraph(
				"Percentage of lightning strike TOTAL damage for the first stack.",
				"",
				"Vanilla value: 175"),
			defaultValue: 140f,
			constraint: new(Min: 100, Max: 190f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> LuminousShotDamageStack = new(
			section: "Item: Luminous Shot",
			name: "Damage per Stack",
			description: Paragraph(
				"Additional percentage of lightning strike TOTAL damage for every subsequent stack.",
				"",
				"Vanilla value: 50"),
			defaultValue: 60f,
			constraint: new(Min: 0f, Max: 80f));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> LuminousShotChargesMaxBase = new(
			section: "Item: Luminous Shot",
			name: "Base Max Charges",
			description: Paragraph(
				"Maximum number of charges for the first stack.",
				"",
				"Vanilla value: 5"),
			defaultValue: 3,
			constraint: new(Min: 3, Max: 6));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> LuminousShotChargesMaxStack = new(
			section: "Item: Luminous Shot",
			name: "Max Charges per Stack",
			description: Paragraph(
				"Increased maximum number of charges for every subsequent stack.",
				"",
				"Vanilla value: 1"),
			defaultValue: 1,
			constraint: new(Min: 0, Max: 2));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> LuminousShotCooldownReductionBase = new(
			section: "Item: Luminous Shot",
			name: "Base Cooldown Reduction",
			description: Paragraph(
				"Percentage of Secondary skill cooldown reduction for the first stack.",
				"",
				"Vanilla value: 20"),
			defaultValue: 15f,
			constraint: new(Min: 0f, Max: 30f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> LuminousShotCooldownReductionStack = new(
			section: "Item: Luminous Shot",
			name: "Cooldown Reduction per Stack",
			description: Paragraph(
				"Percentage of Secondary skill cooldown reduction for every subsequent stack.",
				"",
				"Vanilla value: 0"),
			defaultValue: 0f,
			constraint: new(Min: 0f, Max: 10f));
		public enum LuminousShotCooldownReductionStacking {
			Linear,
			Hyperbolic,
			Exponential,
		}
		public static readonly ConstrainedConfigEntry<LuminousShotCooldownReductionStacking, ListEntryConstraint<LuminousShotCooldownReductionStacking>> LuminousShotCooldownReductionStackType = new(
			section: "Item: Luminous Shot",
			name: "Cooldown Reduction Stacking Type",
			description: Paragraph(
				StackTypeDescription<LuminousShotCooldownReductionStacking>(),
				"Vanilla value: N/A",
				"Not recommended: Linear"),
			defaultValue: LuminousShotCooldownReductionStacking.Hyperbolic,
			constraint: new(typeof(LuminousShotCooldownReductionStacking)));
	}
}
