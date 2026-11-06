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

		public static float Hyperbolic(float normal, int stacks) => normal * stacks / (1f + normal * stacks);

		public static float Reciprocal(float initial, float perStack, int stacks) => initial / (1f + perStack * stacks);

		public static float Exponential(float multiplier, int stacks) => MathF.Pow(multiplier, stacks);
	}
}
