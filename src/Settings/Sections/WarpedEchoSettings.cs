using ConfigEntryExt;
using ConfigEntryExt.Constraints;
using StackFix.Utils;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> WarpedEchoFixEnabled = new(
			section: "General",
			name: "Fix Warped Echo",
			description: Paragraph(
				"Enables Warped Echo tweaks.",
				Vanilla(false)),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> WarpedEchoDamageReductionBase = new(
			section: "Item: Warped Echo",
			name: "Base Damage Reduction",
			description: Paragraph(
				"Percentage of damage reduction for the first stack.",
				Vanilla(10f)),
			defaultValue: 5f,
			constraint: new(Min: 1f, Max: 10f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> WarpedEchoDamageReductionStack = new(
			section: "Item: Warped Echo",
			name: "Damage Reduction per Stack",
			description: Paragraph(
				"Additional percentage of damage reduction for every subsequent stack.",
				Vanilla(0f)),
			defaultValue: 5f,
			constraint: new(Min: 0f, Max: 8f));

		public enum WarpedEchoDamageReductionStacking {
			Linear,
			Hyperbolic,
			Exponential,
		}
		public static readonly ConstrainedConfigEntry<WarpedEchoDamageReductionStacking, ListEntryConstraint<WarpedEchoDamageReductionStacking>> WarpedEchoDamageReductionStackType = new(
			section: "Item: Warped Echo",
			name: "Damage Reduction Stacking Type",
			description: Paragraph(
				StackTypeDescription<WarpedEchoDamageReductionStacking>(),
				NoVanilla(),
				ItD.Danger("Not recommended: Linear, Exponential")),
			defaultValue: WarpedEchoDamageReductionStacking.Hyperbolic,
			constraint: new(typeof(WarpedEchoDamageReductionStacking)));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> WarpedEchoDamageSpreadBase = new(
			section: "Item: Warped Echo",
			name: "Base Damage Spread",
			description: Paragraph(
				"Damage spread instances for the first stack.",
				Vanilla(3)),
			defaultValue: 3,
			constraint: new(Min: 2, Max: 6));

		public static readonly ConstrainedConfigEntry<int, RangeEntryConstraint<int>> WarpedEchoDamageSpreadStack = new(
			section: "Item: Warped Echo",
			name: "Damage Spread per Stack",
			description: Paragraph(
				"Additional damage spread instances for every subsequent stack.",
				Vanilla(1)),
			defaultValue: 0,
			constraint: new(Min: 0, Max: 2));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> WarpedEchoCooldownBase = new(
			section: "Item: Warped Echo",
			name: "Base Cooldown",
			description: Paragraph(
				"Base cooldown in seconds.",
				Vanilla(15f)),
			defaultValue: 15f,
			constraint: new(Min: 10f, Max: 20f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> WarpedEchoCooldownStack = new(
			section: "Item: Warped Echo",
			name: "Cooldown Reduction per Stack",
			description: Paragraph(
				"Percentage of cooldown reduction for every subsequent stack.",
				Vanilla(0f)),
			defaultValue: 2.5f,
			constraint: new(Min: 0f, Max: 10f));

		public enum WarpedEchoCooldownStacking {
			Linear,
			Reciprocal,
			Exponential,
		}
		public static readonly ConstrainedConfigEntry<WarpedEchoCooldownStacking, ListEntryConstraint<WarpedEchoCooldownStacking>> WarpedEchoCooldownStackType = new(
			section: "Item: Warped Echo",
			name: "Cooldown Reduction Stacking Type",
			description: Paragraph(
				StackTypeDescription<WarpedEchoCooldownStacking>(),
				NoVanilla(),
				ItD.Danger("Not recommended: Linear")),
			defaultValue: WarpedEchoCooldownStacking.Exponential,
			constraint: new(typeof(WarpedEchoCooldownStacking)));
	}
}
