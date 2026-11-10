using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
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
	}
}
