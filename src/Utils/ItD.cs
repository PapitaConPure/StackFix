using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StackFix.Utils {
	internal static class ItD {
		public const string FORMAT_NUMBER_DEFAULT      = "{0:0.##}";
		public const string FORMAT_NUMBER_INCR_DEFAULT = "{0:+0.##;-0.##}";

		public static string Damage(object desc)
			=> $"<style=cIsDamage>{desc}</style>";

		public static string Healing(object desc)
			=> $"<style=cIsHealing>{desc}</style>";
		public static string ItemGreen(object desc)
			=> $"<style=cIsHealing>{desc}</style>";

		public static string Utility(object desc)
			=> $"<style=cIsUtility>{desc}</style>";

		public static string Health(object desc)
			=> $"<style=cIsHealth>{desc}</style>";

		public static string Stack(string desc)
			=> $"<style=cStack>{desc}</style>";

		public static string Stack(float value, string format = FORMAT_NUMBER_INCR_DEFAULT, float hideThreshold = 0.1f)
			=> value.AboutEquals(0f, hideThreshold)
				? ""
				: $" <style=cStack>({FormatValue(format, value)} per stack)</style>";

		public static string Stack<TVanilla>(float value, ItemStacking.Type type, string format = FORMAT_NUMBER_INCR_DEFAULT, TVanilla? vanillaType = null, float hideThreshold = 0.1f)
			where TVanilla : struct, Enum
			=> value.AboutEquals(0f, hideThreshold)
				? ""
				: $" <style=cStack>({FormatValue(format, (type == ItemStacking.Type.Reciprocal) ? -MathF.Abs(value) : value)} per stack" +
				  $"{(IsVanillaType(type, vanillaType) ? "" : $", {type.ToString().ToLower()}")}" +
				  $")</style>";

		public static string Stack(float value, ItemStacking.Type type, string format = FORMAT_NUMBER_INCR_DEFAULT, ItemStacking.Type? vanillaType = null, float hideThreshold = 0.1f)
			=> Stack<ItemStacking.Type>(value, type, format, vanillaType, hideThreshold);

		public static string Stack<TVanilla>(float value, ItemStacking.Type type, TVanilla? vanillaType = null, float hideThreshold = 0.1f)
			where TVanilla : struct, Enum
			=> Stack(value, type, FORMAT_NUMBER_INCR_DEFAULT, vanillaType, hideThreshold);

		public static string Stack(float value, ItemStacking.Type type, ItemStacking.Type? vanillaType = null, float hideThreshold = 0.1f)
			=> Stack<ItemStacking.Type>(value, type, vanillaType, hideThreshold);

		private static bool IsVanillaType<TVanilla>(ItemStacking.Type type, TVanilla? vanillaType) where TVanilla: struct, Enum {
			if(vanillaType is ItemStacking.Type itemStackingVanillaType)
				return type == itemStackingVanillaType;

			if(vanillaType is TVanilla vt)
				return type == ItemStacking.FromEnum(vt);

			return false;
		}

		public static string StackingType(ItemStacking.Type type)
			=> $"<style={type switch {
				ItemStacking.Type.Hyperbolic => "cIsHealing",
				ItemStacking.Type.Reciprocal => "cIsVoid",
				ItemStacking.Type.Exponential => "cIsHealth",
				_ => "cSub",
			}}>[{type}]</style>";

		public static string Mono(object desc)
			=> $"<style=cMono>{desc}</style>";

		public static string Death(object desc)
			=> $"<style=cDeath>{desc}</style>";
		public static string Danger(object desc)
			=> $"<style=cDeath>{desc}</style>";
		public static string ItemRed(object desc)
			=> $"<style=cDeath>{desc}</style>";

		public static string UserSetting(object desc)
			=> $"<style=cUserSetting>{desc}</style>";

		public static string Artifact(object desc)
			=> $"<style=cArtifact>{desc}</style>";

		public static string Sub(object desc)
			=> $"<style=cSub>{desc}</style>";

		public static string Event(object desc)
			=> $"<style=cEvent>{desc}</style>";

		public static string WorldEvent(object desc)
			=> $"<style=cWorldEvent>{desc}</style>";

		public static string KeywordName(object desc)
			=> $"<style=cKeywordName>{desc}</style>";

		public static string Shrine(object desc)
			=> $"<style=cShrine>{desc}</style>";
		public static string ItemYellow(object desc)
			=> $"<style=cShrine>{desc}</style>";

		public static string Void(object desc)
			=> $"<style=cIsVoid>{desc}</style>";
		public static string ItemVoid(object desc)
			=> $"<style=cIsVoid>{desc}</style>";

		public static string V<T>(T value, string format = FORMAT_NUMBER_DEFAULT) where T : IComparable
			=> FormatValue(format, value);

		private static string FormatValue(string format, object value)
			=> string.Format(CultureInfo.InvariantCulture, format, value);

		public static string ItemsWGRY(float white, float green, float red, float yellow, string format = FORMAT_NUMBER_DEFAULT, float hideThreshold = 0.1f)
			=> string.Join("/", FilterAndFormatValues([white, green, red, yellow], format, hideThreshold));
		public static string ItemsWGR(float white, float green, float red, string format = FORMAT_NUMBER_DEFAULT, float hideThreshold = 0.1f)
			=> string.Join("/", FilterAndFormatValues([white, green, red], format, hideThreshold));
		public static string ItemsGRY(float green, float red, float yellow, string format = FORMAT_NUMBER_DEFAULT, float hideThreshold = 0.1f)
			=> string.Join("/", FilterAndFormatValues([green, red, yellow], format, hideThreshold));
		public static string ItemsGR(float green, float red, string format = FORMAT_NUMBER_DEFAULT, float hideThreshold = 0.1f)
			=> string.Join("/", FilterAndFormatValues([green, red], format, hideThreshold));

		private static IEnumerable<string> FilterAndFormatValues(float[] values, string format, float hideThreshold)
			=> values
				.Where(value => !value.AboutEquals(0f, hideThreshold))
				.Select(value => string.Format(format, value));
	}
}
