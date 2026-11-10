using StackFix.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StackFix.Settings {
	internal static partial class ModSettings {
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
						.Select(t => $"* {t}: {(descs.TryGetValue(t, out string d) ? d : "description not available")}.")),
				"");
		}
	}
}
