namespace StackFix.Utils {
	internal static class StringUtils {
		public static string ToCapitalized(this string str) {
			return str[..1].ToUpper() + str[1..].ToLower();
		}
	}
}
