using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> BoxOfDynamiteFixEnabled = new(
			section: "General",
			name: "Fix Box of Dynamite",
			description: Paragraph(
				"Enables Box of Dynamite tweaks.",
				Vanilla(false)),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BoxOfDynamiteDamageBase = new(
			section: "Item: Box of Dynamite",
			name: "Base Damage",
			description: Paragraph(
				"Percentage of dynamite damage for the first stack.",
				Vanilla(240f)),
			defaultValue: 240f,
			constraint: new(Min: 160f, Max: 300f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BoxOfDynamiteDamageStack = new(
			section: "Item: Box of Dynamite",
			name: "Damage per Stack",
			description: Paragraph(
				"Additional percentage of dynamite damage for every subsequent stack.",
				Vanilla(85f)),
			defaultValue: 0f,
			constraint: new(Min: 0f, Max: 120f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BoxOfDynamiteCooldownBase = new(
			section: "Item: Box of Dynamite",
			name: "Base Cooldown",
			description: Paragraph(
				"Seconds of cooldown for the first stack.",
				Vanilla(10f)),
			defaultValue: 10f,
			constraint: new(Min: 8f, Max: 12f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BoxOfDynamiteCooldownStack = new(
			section: "Item: Box of Dynamite",
			name: "Cooldown Reduction per Stack",
			description: Paragraph(
				"Percentage of cooldown reduction for every subsequent stack.",
				Vanilla(0f)),
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
				NoVanilla()),
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
				NoVanilla()),
			defaultValue: 0.25f,
			constraint: new(Min: 0.25f, Max: 2.5f));
	}
}
