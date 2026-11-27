using ConfigEntryExt;
using ConfigEntryExt.Constraints;
using StackFix.Utils;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> BreachingFinFixEnabled = new(
			section: "General",
			name: "Fix Breaching Fin",
			description: Paragraph(
				"Enables Breaching Fin tweaks.",
				Vanilla(false)),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> BreachingFinAirLaunchesBase = new(
			section: "Item: Breaching Fin",
			name: "Base Airborne Launches",
			description: Paragraph(
				"Maximum number of airborne launches for the first stack.",
				Vanilla(2)),
			defaultValue: 1,
			constraint: new(Min: 0, Max: 2));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> BreachingFinAirLaunchesStack = new(
			section: "Item: Breaching Fin",
			name: "Airborne Launches per Stack",
			description: Paragraph(
				"Increased maximum number of airborne launches for every subsequent stack.",
				Vanilla(1)),
			defaultValue: 1,
			constraint: new(Min: 0, Max: 2));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BreachingFinDamageBase = new(
			section: "Item: Breaching Fin",
			name: "Base Damage",
			description: Paragraph(
				"Percentage of airborne increased damage per launch for the first stack.",
				Vanilla(10f),
				Note("If playing with Quality Mod, this setting may override or be overriden by Quality variants of the item.")),
			defaultValue: 10f,
			constraint: new(Min: 2f, Max: 20f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BreachingFinDamageStack = new(
			section: "Item: Breaching Fin",
			name: "Damage per Stack",
			description: Paragraph(
				"Additional percentage of airborne increased damage per launch for every subsequent stack.",
				Vanilla(0f),
				Note("If playing with Quality Mod, this setting may override or be overriden by Quality variants of the item.")),
			defaultValue: 2f,
			constraint: new(Min: 0f, Max: 10f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BreachingFinCooldownBase = new(
			section: "Item: Breaching Fin",
			name: "Base Cooldown",
			description: Paragraph(
				"Base cooldown in seconds.",
				Vanilla(15f)),
			defaultValue: 15f,
			constraint: new(Min: 10f, Max: 20f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BreachingFinCooldownStack = new(
			section: "Item: Breaching Fin",
			name: "Cooldown Reduction per Stack",
			description: Paragraph(
				"Percentage of cooldown reduction for every subsequent stack.",
				Vanilla(0f)),
			defaultValue: 1f,
			constraint: new(Min: 0f, Max: 10f));

		public enum BreachingFinCooldownStacking {
			Linear,
			Reciprocal,
			Exponential,
		}
		public static readonly ConstrainedConfigEntry<BreachingFinCooldownStacking, ListEntryConstraint<BreachingFinCooldownStacking>> BreachingFinCooldownStackType = new(
			section: "Item: Breaching Fin",
			name: "Cooldown Reduction Stacking Type",
			description: Paragraph(
				StackTypeDescription<BreachingFinCooldownStacking>(),
				NoVanilla(),
				ItD.Danger("Not recommended: Linear")),
			defaultValue: BreachingFinCooldownStacking.Exponential,
			constraint: new(typeof(BreachingFinCooldownStacking)));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> BreachingFinCooldownCap = new(
			section: "Item: Breaching Fin",
			name: "Minimum Cooldown",
			description: Paragraph(
				"Absolute minimum seconds of cooldown.",
				NoVanilla()),
			defaultValue: 5f,
			constraint: new(Min: 0.5f, Max: 10f));
	}
}
