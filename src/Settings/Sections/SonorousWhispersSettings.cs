using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> SonorousWhispersFixEnabled = new (
			section: "General",
			name: "Fix Sonorous Whispers",
			description: Paragraph(
				"Enables Sonorous Whispers tweaks.",
				"Vanilla value: Off"),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> SonorousWhispersChanceBase = new(
			section: "Item: Sonorous Whispers",
			name: "Base Chance",
			description: Paragraph(
				"Chance percentage for elite monsters to drop an item for the first stack.",
				"Vanilla value: 5"),
			defaultValue: 0f,
			constraint: new(Min: 0f, Max: 5f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> SonorousWhispersChanceStack = new(
			section: "Item: Sonorous Whispers",
			name: "Chance per Stack",
			description: Paragraph(
				"Additional chance percentage for elite monsters to drop an item for every subsequent stack.",
				"Vanilla value: 1"),
			defaultValue: 2.5f,
			constraint: new(Min: 0f, Max: 5f));

		public enum SonorousWhispersChanceStacking {
			Linear,
			Hyperbolic,
		}
		public static readonly ConstrainedConfigEntry<SonorousWhispersChanceStacking, ListEntryConstraint<SonorousWhispersChanceStacking>> SonorousWhispersChanceStackType = new(
			section: "Item: Sonorous Whispers",
			name: "Chance Stacking Type",
			description: Paragraph(
				StackTypeDescription<SonorousWhispersChanceStacking>(),
				"Vanilla value: Linear"),
			defaultValue: SonorousWhispersChanceStacking.Linear,
			constraint: new(typeof(SonorousWhispersChanceStacking)));
	}
}
