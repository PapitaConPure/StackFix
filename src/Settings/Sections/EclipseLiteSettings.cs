using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> EclipseLiteFixEnabled = new(
			section: "General",
			name: "Fix Eclipse Lite",
			description: Paragraph(
				"Enables Eclipse Lite tweaks.",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> EclipseLiteBarrierBase = new(
			section: "Item: Eclipse Lite",
			name: "Base Cooldown Barrier",
			description: Paragraph(
				"Percentage of max health gained in barrier for every second of a skill's cooldown for the first stack.",
				"Vanilla value: 1"),
			defaultValue: 0.5f,
			constraint: new(Min: 0.25f, Max: 1.5f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> EclipseLiteBarrierStack = new(
			section: "Item: Eclipse Lite",
			name: "Cooldown Barrier per Stack",
			description: Paragraph(
				"Additional percentage of max health gained in barrier for every second of a skill's cooldown for every subsequent stack.",
				"Vanilla value: 0.25"),
			defaultValue: 0.5f,
			constraint: new(Min: 0.1f, Max: 1f));

		public enum EclipseLiteBarrierStacking {
			Linear,
			Hyperbolic,
		}
		public static readonly ConstrainedConfigEntry<EclipseLiteBarrierStacking, ListEntryConstraint<EclipseLiteBarrierStacking>> EclipseLiteBarrierStackType = new(
			section: "Item: Eclipse Lite",
			name: "Cooldown Barrier Stacking Type",
			description: Paragraph(
				StackTypeDescription<EclipseLiteBarrierStacking>(),
				"Vanilla value: Linear"),
			defaultValue: EclipseLiteBarrierStacking.Hyperbolic,
			constraint: new(typeof(EclipseLiteBarrierStacking)));
	}
}
