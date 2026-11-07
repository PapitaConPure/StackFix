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
	}
}
