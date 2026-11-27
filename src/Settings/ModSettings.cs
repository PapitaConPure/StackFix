using ConfigEntryExt;
using StackFix.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StackFix.Settings {
	internal static partial class ModSettings {
		private static readonly Regex descModifierRegex = new(@"</?style(?:=[^>]*)?>", RegexOptions.Compiled);

		internal static void Awake() {
			DescriptionModifiers.Register(description => descModifierRegex.Replace(description, ""));
		}

		internal static void OnDestroy() {
			DescriptionModifiers.Clear();
		}

		private static string Paragraph(params string[] lines)
			=> string.Join("\n", lines);

		private static string StackTypeDescription<TEnum>() where TEnum : struct, Enum {
			Dictionary<ItemStacking.Type, string> descs = new() {
				[ItemStacking.Type.Linear] = "effect increases by a fixed amount per stack",
				[ItemStacking.Type.Hyperbolic] = "effect increases with diminishing returns per stack, approaching a limit",
				[ItemStacking.Type.Reciprocal] = "effect decreases proportionally with diminishing returns per stack",
				[ItemStacking.Type.Exponential] = "effect compounds by a constant factor per stack. May increase or reduce a stat based on context",
			};
			IEnumerable<ItemStacking.Type> types = typeof(ItemStacking.Type)
				.GetEnumValues()
				.OfType<ItemStacking.Type>();
			Type choiceEnumType = typeof(TEnum);

			return Paragraph(
				"Determines how the effect will scale based on the number of stacks you have of the item.",
				"",
				string.Join("\n",
					types
						.Where(t => Enum.IsDefined(choiceEnumType, t.ToString()))
						.Select(t => $"* {ItD.UserSetting(t)}: {(descs.TryGetValue(t, out string d) ? d : ItD.Sub("description not available"))}.")));
		}

		private static string NoVanilla()
			=> BasicVanillaFormat(ItD.Danger("N/A"));

		private static string Vanilla(bool value)
			=> BasicVanillaFormat(value ? "On" : "Off");

		private static string Vanilla(string value)
			=> BasicVanillaFormat($"{ItD.Sub("\"")}{value}{ItD.Sub("\"")}");

		private static string Vanilla(float value, string format = null)
			=> BasicVanillaFormat(ItD.V(value, format));

		private static string Vanilla<TEnum>(TEnum value) where TEnum : struct, Enum
			=> BasicVanillaFormat(value);

		private static string BasicVanillaFormat(object value)
			=> "\n" + ItD.UserSetting($"Vanilla value: {value}");

		private static string Note(string note)
			=> "\n" + ItD.Sub($"< ! > {note}");
	}
}
