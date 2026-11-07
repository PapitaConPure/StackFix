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

		public static float Hyperbolic(float coefficient, int stacks) => coefficient * stacks / (1f + coefficient * stacks);
		public static float Hyperbolic(float initial, float coefficient, int stacks) => 1f - 1f / (1f + initial + coefficient * (stacks - 1f));
		public static float HyperbolicPercent(float initial, float perStack, int stacks)
			=> Hyperbolic(initial * 0.01f, perStack * 0.01f, stacks) * 100f;

		public static float Reciprocal(float initial, int stacks) => initial / stacks;
		public static float Reciprocal(float initial, float coefficient, int stacks) => initial / (1f + coefficient * stacks);
		public static float ReciprocalPercent(float initial, float coefficient, int stacks)
			=> Reciprocal(initial, coefficient * 0.01f, stacks);

		public static float Exponential(float multiplier, int stacks) => MathF.Pow(multiplier, stacks);
		public static float Exponential(float initial, float multiplier, int stacks) => initial * MathF.Pow(multiplier, stacks - 1f);
	}
}
