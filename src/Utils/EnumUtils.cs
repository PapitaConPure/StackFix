using System;

namespace StackFix.Utils {
	internal static class EnumUtils {

		/// <summary>
		/// Converts the specified <paramref name="value"/> from <typeparamref name="TSource"/>
		/// to <typeparamref name="TResult"/> via matching value names.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TResult EnumToEnum<TSource, TResult>(TSource value)
			where TSource : struct, Enum
			where TResult : struct, Enum
			=> Enum.Parse<TResult>(value.ToString());
	}
}
