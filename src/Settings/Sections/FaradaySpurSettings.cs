using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> FaradaySpurFixEnabled = new(
			section: "General",
			name: "Fix Faraday Spur",
			description: Paragraph(
				"Enables Faraday Spur tweaks.",
				"",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurMoveSpeedBase = new(
			section: "Item: Faraday Spur",
			name: "Base Movement Speed",
			description: Paragraph(
				"Bonus movement speed percentage at maximum charge for the first stack.",
				"",
				"Vanilla value: 160"),
			defaultValue: 70f,
			constraint: new(Min: 0f, Max: 160f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurMoveSpeedStack = new(
			section: "Item: Faraday Spur",
			name: "Movement Speed per Stack",
			description: Paragraph(
				"Increased bonus movement speed percentage at maximum charge for every subsequent stack.",
				"",
				"Vanilla value: 0"),
			defaultValue: 30f,
			constraint: new(Min: 0f, Max: 160f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurJumpHeightBase = new(
			section: "Item: Faraday Spur",
			name: "Base Jump Height",
			description: Paragraph(
				"Bonus jump height percentage at maximum charge for the first stack.",
				"",
				"Vanilla value: 200"),
			defaultValue: 80f,
			constraint: new(Min: 0f, Max: 200f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurJumpHeightStack = new(
			section: "Item: Faraday Spur",
			name: "Jump Height per Stack",
			description: Paragraph(
				"Increased bonus jump height percentage at maximum charge for every subsequent stack.",
				"",
				"Vanilla value: 0"),
			defaultValue: 40f,
			constraint: new(Min: 0f, Max: 200f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurDamageBase = new(
			section: "Item: Faraday Spur",
			name: "Base Discharge Damage",
			description: Paragraph(
				"Explosion damage percentage for the first stack.",
				"",
				"Vanilla value: 400"),
			defaultValue: 360f,
			constraint: new(Min: 0f, Max: 400f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurDamageStack = new(
			section: "Item: Faraday Spur",
			name: "Discharge Damage per Stack",
			description: Paragraph(
				"Increased explosion damage percentage for every subsequent stack.",
				"",
				"Vanilla value: 280"),
			defaultValue: 0f,
			constraint: new(Min: 0f, Max: 400f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurDistanceBase = new(
			section: "Item: Faraday Spur",
			name: "Base Discharge Distance",
			description: Paragraph(
				"Explosion radius (in meters) for the first stack.",
				"",
				"Vanilla value: 32.3"),
			defaultValue: 32.3f,
			constraint: new(Min: 0f, Max: 44.5f)); //( ^)o(^ )b

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> FaradaySpurDistanceStack = new(
			section: "Item: Faraday Spur",
			name: "Discharge Distance per Stack",
			description: Paragraph(
				"Increased explosion radius (in meters) for every subsequent stack.",
				"",
				"Vanilla value: 7.5"),
			defaultValue: 7.5f,
			constraint: new(Min: 0f, Max: 7.5f));

		public enum FaradaySpurCategory {
			Damage,
			Utility,
		}
		public static readonly ConstrainedConfigEntry<FaradaySpurCategory, ListEntryConstraint<FaradaySpurCategory>> FaradaySpurSelectedCategory = new(
			section: "Item: Faraday Spur",
			name: "Category",
			description: Paragraph(
				"Determines which kind of category chest can contain this item.",
				"This setting will take effect next stage if changed during a run.",
				"",
				"Vanilla value: Damage"),
			defaultValue: FaradaySpurCategory.Utility,
			constraint: new(typeof(FaradaySpurCategory)));
	}
}
