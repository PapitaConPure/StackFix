using ConfigEntryExt;
using ConfigEntryExt.Constraints;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		public static readonly ConfigEntryExt<bool> SonorousWhispersFixEnabled = new (
			section: "General",
			name: "Fix Sonorous Whispers",
			description: Paragraph(
				"Enables Sonorous Whispers tweaks.",
				Vanilla(false)),
			defaultValue: true);

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> SonorousWhispersChanceBase = new(
			section: "Item: Sonorous Whispers",
			name: "Base Elite Drop Chance",
			description: Paragraph(
				"Chance percentage for elite monsters to drop an item for the first stack.",
				Vanilla(5f),
				Note("Vanilla value is incorrectly reported as 4, but is actually 5. Trust me bro.")),
			defaultValue: 0f,
			constraint: new(Min: 0f, Max: 5f));

		public static readonly ConstrainedConfigEntry<float, RangeEntryConstraint<float>> SonorousWhispersChanceStack = new(
			section: "Item: Sonorous Whispers",
			name: "Elite Drop Chance per Stack",
			description: Paragraph(
				"Additional chance percentage for elite monsters to drop an item for every subsequent stack.",
				Vanilla(1f)),
			defaultValue: 2.5f,
			constraint: new(Min: 0f, Max: 5f));

		public enum SonorousWhispersChanceStacking {
			Linear,
			Hyperbolic,
		}
		public static readonly ConstrainedConfigEntry<SonorousWhispersChanceStacking, ListEntryConstraint<SonorousWhispersChanceStacking>> SonorousWhispersChanceStackType = new(
			section: "Item: Sonorous Whispers",
			name: "Elite Drop Chance Stacking Type",
			description: Paragraph(
				StackTypeDescription<SonorousWhispersChanceStacking>(),
				Vanilla(SonorousWhispersChanceStacking.Linear)),
			defaultValue: SonorousWhispersChanceStacking.Linear,
			constraint: new(typeof(SonorousWhispersChanceStacking)));
	}
}
