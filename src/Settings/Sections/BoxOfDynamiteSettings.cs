using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> BoxOfDynamiteFixEnabled = new(
			section: "General",
			name: "Fix Box of Dynamite",
			description: Paragraph(
				"Enables Box of Dynamite tweaks.",
				"",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BoxOfDynamiteDamageBase = new(
			section: "Item: Box of Dynamite",
			name: "Base Damage",
			description: Paragraph(
				"Percentage of dynamite damage for the first stack.",
				"",
				"Vanilla value: 240"),
			defaultValue: 240f,
			constraint: new(Min: 160f, Max: 300f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BoxOfDynamiteDamageStack = new(
			section: "Item: Box of Dynamite",
			name: "Damage per Stack",
			description: Paragraph(
				"Additional percentage of dynamite damage for every subsequent stack.",
				"",
				"Vanilla value: 85"),
			defaultValue: 0f,
			constraint: new(Min: 0f, Max: 120f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BoxOfDynamiteCooldownBase = new(
			section: "Item: Box of Dynamite",
			name: "Base Cooldown",
			description: Paragraph(
				"Seconds of cooldown for the first stack.",
				"",
				"Vanilla value: 10"),
			defaultValue: 10f,
			constraint: new(Min: 8f, Max: 12f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BoxOfDynamiteCooldownStack = new(
			section: "Item: Box of Dynamite",
			name: "Cooldown Reduction per Stack",
			description: Paragraph(
				"Percentage of cooldown reduction for every subsequent stack.",
				"",
				"Vanilla value: N/A"),
			defaultValue: 10f,
			constraint: new(Min: 0f, Max: 15f));

		public enum BoxOfDynamiteCooldownStacking {
			Linear,
			Reciprocal,
			Exponential,
		}
		public static readonly ConstrainedConfigEntry<BoxOfDynamiteCooldownStacking, ListEntryConstraint<BoxOfDynamiteCooldownStacking>> BoxOfDynamiteCooldownStackType = new(
			section: "Item: Box of Dynamite",
			name: "Cooldown Reduction Stacking Type",
			description: Paragraph(
				StackTypeDescription<BoxOfDynamiteCooldownStacking>(),
				"Vanilla value: N/A"),
			defaultValue: BoxOfDynamiteCooldownStacking.Exponential,
			constraint: new(typeof(BoxOfDynamiteCooldownStacking)));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BoxOfDynamiteCooldownCap = new(
			section: "Item: Box of Dynamite",
			name: "Minimum Cooldown",
			description: Paragraph(
				"Absolute minimum seconds of cooldown.",
				"",
				"This setting is intended for performance, not balancing.",
				"Increase this to improve CPU performance at higher drone and stack counts.",
				"",
				"Vanilla value: N/A"),
			defaultValue: 0.5f,
			constraint: new(Min: 0.25f, Max: 2.5f));
	}
}
