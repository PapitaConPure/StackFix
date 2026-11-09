using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static class ModSettings {
		public static readonly ConfigEntryExt<bool> Enabled = new(
			section: "General",
			name: "Enable Mod",
			description: Paragraph(
				"Enables the mod.",
				"Vanilla value: Off"),
			defaultValue: true);

		#region Bolstering Lantern
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
		#endregion

		#region Eclipse Lite
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
				GenericStackingTypeDescription,
				"Vanilla value: Linear"),
			defaultValue: EclipseLiteBarrierStacking.Hyperbolic,
			constraint: new(typeof(EclipseLiteBarrierStacking)));
		#endregion

		#region Warped Echo
		public static readonly ConfigEntryExt<bool> WarpedEchoFixEnabled = new(
			section: "General",
			name: "Fix Warped Echo",
			description: Paragraph(
				"Enables Warped Echo tweaks.",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> WarpedEchoDamageReductionBase = new(
			section: "Item: Warped Echo",
			name: "Base Damage Reduction",
			description: Paragraph(
				"Percentage of damage reduction for the first stack.",
				"Vanilla value: 10"),
			defaultValue: 5f,
			constraint: new(Min: 1f, Max: 10f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> WarpedEchoDamageReductionStack = new(
			section: "Item: Warped Echo",
			name: "Damage Reduction per Stack",
			description: Paragraph(
				"Additional percentage of damage reduction for every subsequent stack.",
				"Vanilla value: 0"),
			defaultValue: 5f,
			constraint: new(Min: 0.1f, Max: 8f));

		public enum WarpedEchoDamageReductionStacking {
			Linear,
			Reciprocal,
			Exponential,
		}
		public static readonly ConstrainedConfigEntry<WarpedEchoDamageReductionStacking, ListEntryConstraint<WarpedEchoDamageReductionStacking>> WarpedEchoDamageReductionStackType = new(
			section: "Item: Warped Echo",
			name: "Damage Reduction Stacking Type",
			description: Paragraph(
				GenericStackingTypeDescription,
				"Vanilla value: N/A",
				"Not recommended: Linear, Exponential"),
			defaultValue: WarpedEchoDamageReductionStacking.Reciprocal,
			constraint: new(typeof(WarpedEchoDamageReductionStacking)));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> WarpedEchoDamageSpreadBase = new(
			section: "Item: Warped Echo",
			name: "Base Damage Spread",
			description: Paragraph(
				"Damage spread instances for the first stack.",
				"Vanilla value: 3"),
			defaultValue: 3,
			constraint: new(Min: 2, Max: 6));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> WarpedEchoDamageSpreadStack = new(
			section: "Item: Warped Echo",
			name: "Damage Spread per Stack",
			description: Paragraph(
				"Additional damage spread instances for every subsequent stack.",
				"Vanilla value: 1"),
			defaultValue: 0,
			constraint: new(Min: 0, Max: 2));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> WarpedEchoCooldownBase = new(
			section: "Item: Warped Echo",
			name: "Base Cooldown",
			description: Paragraph(
				"Base cooldown in seconds.",
				"Vanilla value: 15"),
			defaultValue: 15f,
			constraint: new(Min: 10f, Max: 20f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> WarpedEchoCooldownStack = new(
			section: "Item: Warped Echo",
			name: "Cooldown Reduction per Stack",
			description: Paragraph(
				"Percentage of cooldown reduction for every subsequent stack.",
				"Vanilla value: 0"),
			defaultValue: 2.5f,
			constraint: new(Min: 0f, Max: 10f));

		public enum WarpedEchoCooldownStacking {
			Linear,
			Reciprocal,
			Exponential,
		}
		public static readonly ConstrainedConfigEntry<WarpedEchoCooldownStacking, ListEntryConstraint<WarpedEchoCooldownStacking>> WarpedEchoCooldownStackType = new(
			section: "Item: Warped Echo",
			name: "Cooldown Stacking Type",
			description: Paragraph(
				GenericStackingTypeDescription,
				"Vanilla value: N/A",
				"Not recommended: Linear"),
			defaultValue: WarpedEchoCooldownStacking.Exponential,
			constraint: new(typeof(WarpedEchoCooldownStacking)));
		#endregion

		#region Faraday Spur
		public static readonly ConfigEntryExt<bool> FaradaySpurFixEnabled = new(
			section: "General",
			name: "Fix Faraday Spur",
			description: Paragraph(
				"Enables Faraday Spur tweaks.",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurMoveSpeedBase = new(
			section: "Item: Faraday Spur",
			name: "Base Movement Speed",
			description: Paragraph(
				"Bonus movement speed percentage at maximum charge for the first stack.",
				"Vanilla value: 160"),
			defaultValue: 70f,
			constraint: new(Min: 0f, Max: 160f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurMoveSpeedStack = new(
			section: "Item: Faraday Spur",
			name: "Movement Speed per Stack",
			description: Paragraph(
				"Increased bonus movement speed percentage at maximum charge for every subsequent stack.",
				"Vanilla value: 0"),
			defaultValue: 30f,
			constraint: new(Min: 0f, Max: 160f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurJumpHeightBase = new(
			section: "Item: Faraday Spur",
			name: "Base Jump Height",
			description: Paragraph(
				"Bonus jump height percentage at maximum charge for the first stack.",
				"Vanilla value: 200"),
			defaultValue: 80f,
			constraint: new(Min: 0f, Max: 200f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurJumpHeightStack = new(
			section: "Item: Faraday Spur",
			name: "Jump Height per Stack",
			description: Paragraph(
				"Increased bonus jump height percentage at maximum charge for every subsequent stack.",
				"Vanilla value: 0"),
			defaultValue: 40f,
			constraint: new(Min: 0f, Max: 200f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurDamageBase = new(
			section: "Item: Faraday Spur",
			name: "Base Damage",
			description: Paragraph(
				"Explosion damage percentage for the first stack.",
				"Vanilla value: 400"),
			defaultValue: 300f,
			constraint: new(Min: 0f, Max: 400f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurDamageStack = new(
			section: "Item: Faraday Spur",
			name: "Damage per Stack",
			description: Paragraph(
				"Increased explosion damage percentage for every subsequent stack.",
				"Vanilla value: 280"),
			defaultValue: 200f,
			constraint: new(Min: 0f, Max: 400f));
		#endregion

		#region Networked Suffering
		public static readonly ConfigEntryExt<bool> NetworkedSufferingFixEnabled = new(
			section: "General",
			name: "Fix Networked Suffering",
			description: Paragraph(
				"Enables Networked Suffering tweaks.",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> NetworkedSufferingSpreadBase = new(
			section: "Item: Networked Suffering",
			name: "Base Virus Spread",
			description: Paragraph(
				"Maximum number of infected enemies for the first stack.",
				"Vanilla value: 4"),
			defaultValue: 3,
			constraint: new(Min: 2, Max: 8));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> NetworkedSufferingSpreadStack = new(
			section: "Item: Networked Suffering",
			name: "Virus Spread per Stack",
			description: Paragraph(
				"Increased maximum number of infected enemies for every subsequent stack.",
				"Vanilla value: 2"),
			defaultValue: 2,
			constraint: new(Min: 0, Max: 4));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> NetworkedSufferingDamageBase = new(
			section: "Item: Networked Suffering",
			name: "Base Damage",
			description: Paragraph(
				"Virus damage percentage for the first stack.",
				"Vanilla value: 45"),
			defaultValue: 35f,
			constraint: new(Min: 5f, Max: 50f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> NetworkedSufferingDamageStack = new(
			section: "Item: Networked Suffering",
			name: "Damage per Stack",
			description: Paragraph(
				"Increased virus damage percentage for every subsequent stack.",
				"Vanilla value: 0"),
			defaultValue: 1f,
			constraint: new(Min: 0f, Max: 10f));
		#endregion

		public static readonly ConfigEntryExt<bool> Megumin = new(
			section: "Megumin",
			name: "Megumin",
			description: "Megumin",
			defaultValue: true);

		private static string Paragraph(params string[] lines)
			=> string.Join("\n", lines);

		private static string GenericStackingTypeDescription => Paragraph(
			"Determines how the effect will scale based on the number of stacks you have of the item.",
			"",
			"* Linear: effect increases by a fixed amount per stack.",
			"* Hyperbolic: effect increases with diminishing returns per stack, approaching a limit.",
			"* Reciprocal: effect decreases proportionally with diminishing returns per stack.",
			"* Exponential: effect compounds by a constant factor per stack. May increase or reduce a stat based on context.",
			"");
	}
}
