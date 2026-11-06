using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StackFix.Utils {
	internal static class ItD {
		public static string Damage(string desc)
			=> $"<style=cIsDamage>{desc}</style>";

		public static string Healing(string desc)
			=> $"<style=cIsHealing>{desc}</style>";
		public static string ItemGreen(string desc)
			=> $"<style=cIsHealing>{desc}</style>";

		public static string Utility(string desc)
			=> $"<style=cIsUtility>{desc}</style>";

		public static string Health(string desc)
			=> $"<style=cIsHealth>{desc}</style>";

		public static string Stack(float value, string format = "{0:+0;-0}", float hideThreshold = 0.1f)
			=> value.AboutEquals(0f, hideThreshold)
				? ""
				: $" <style=cStack>({string.Format(CultureInfo.InvariantCulture, format, value)} per stack)</style>";

		public static string Mono(string desc)
			=> $"<style=cMono>{desc}</style>";

		public static string Death(string desc)
			=> $"<style=cDeath>{desc}</style>";
		public static string Danger(string desc)
			=> $"<style=cDeath>{desc}</style>";
		public static string ItemRed(string desc)
			=> $"<style=cDeath>{desc}</style>";

		public static string UserSetting(string desc)
			=> $"<style=cUserSetting>{desc}</style>";

		public static string Artifact(string desc)
			=> $"<style=cArtifact>{desc}</style>";

		public static string Sub(string desc)
			=> $"<style=cSub>{desc}</style>";

		public static string Event(string desc)
			=> $"<style=cEvent>{desc}</style>";

		public static string WorldEvent(string desc)
			=> $"<style=cWorldEvent>{desc}</style>";

		public static string KeywordName(string desc)
			=> $"<style=cKeywordName>{desc}</style>";

		public static string Shrine(string desc)
			=> $"<style=cShrine>{desc}</style>";
		public static string ItemYellow(string desc)
			=> $"<style=cShrine>{desc}</style>";

		public static string ItemsWGRY(float white, float green, float red, float yellow, string format = "{0:+0;-0}", float hideThreshold = 0.1f)
			=> string.Join("/", FilterAndFormatValues([white, green, red, yellow], format, hideThreshold));
		public static string ItemsWGR(float white, float green, float red, string format = "{0:+0;-0}", float hideThreshold = 0.1f)
			=> string.Join("/", FilterAndFormatValues([white, green, red], format, hideThreshold));
		public static string ItemsGRY(float green, float red, float yellow, string format = "{0:+0;-0}", float hideThreshold = 0.1f)
			=> string.Join("/", FilterAndFormatValues([green, red, yellow], format, hideThreshold));
		public static string ItemsGR(float green, float red, string format = "{0:+0;-0}", float hideThreshold = 0.1f)
			=> string.Join("/", FilterAndFormatValues([green, red], format, hideThreshold));

		private static IEnumerable<string> FilterAndFormatValues(float[] values, string format, float hideThreshold)
			=> values
				.Where(value => !value.AboutEquals(0f, hideThreshold))
				.Select(value => string.Format(format, value));
	}
}
