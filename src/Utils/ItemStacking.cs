using System;

namespace StackFix.Utils {
	internal static class ItemStacking {
		public enum Type {
			Linear,
			Hyperbolic,
			Reciprocal,
			Exponential,
		}

		public static int Linear(int initial, int perStack, int stacks) => initial + perStack * (stacks - 1);
		public static float Linear(float initial, float perStack, int stacks) => initial + perStack * (stacks - 1f);

		public static float Hyperbolic(float coefficient, int stacks)
			=> DivideOrDefault(coefficient * stacks, 1f + coefficient * stacks, coefficient);
		public static float Hyperbolic(float initial, float coefficient, int stacks)
			=> DivideOrDefault(initial + coefficient * (stacks - 1f), 1f + initial + coefficient * (stacks - 1f), initial);
		public static float HyperbolicPercent(float initial, float perStack, int stacks)
			=> Hyperbolic(initial * 0.01f, perStack * 0.01f, stacks) * 100f;

		public static float Reciprocal(float initial, int stacks)
			=> DivideOrNumerator(initial, stacks);
		public static float Reciprocal(float initial, float coefficient, int stacks)
			=> DivideOrNumerator(initial, 1f + coefficient * stacks);
		public static float ReciprocalPercent(float initial, float coefficient, int stacks)
			=> Reciprocal(initial, coefficient * 0.01f, stacks);

		public static float Exponential(float multiplier, int stacks)
			=> MathF.Pow(multiplier, stacks);
		public static float Exponential(float initial, float multiplier, int stacks)
			=> initial * MathF.Pow(multiplier, stacks - 1f);

		private static float DivideOrNumerator(float numerator, float denominator)
			=> denominator == 0 ? numerator : (numerator / denominator);
		private static float DivideOrDefault(float numerator, float denominator, float defaultValue)
			=> denominator == 0 ? defaultValue : (numerator / denominator);
	}
}
