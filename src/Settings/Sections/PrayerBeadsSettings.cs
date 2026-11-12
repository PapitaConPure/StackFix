using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> PrayerBeadsFixEnabled = new(
			section: "General",
			name: "Fix Prayer Beads",
			description: Paragraph(
				"Enables Prayer Beads tweaks.",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> PrayerBeadsStatsBase = new(
			section: "Item: Prayer Beads",
			name: "Base Extra Stats",
			description: Paragraph(
				"Stored extra stat gains percentage for the first stack.",
				"Vanilla value: 20"),
			defaultValue: 10f,
			constraint: new(Min: 10f, Max: 30f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> PrayerBeadsStatsStack = new(
			section: "Item: Prayer Beads",
			name: "Extra Stats per Stack",
			description: Paragraph(
				"Additional stored extra stat gains percentage for every subsequent stack.",
				"Vanilla value: 5"),
			defaultValue: 10f,
			constraint: new(Min: 5f, Max: 25f));
	}
}
