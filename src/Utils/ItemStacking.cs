using System;

namespace StackFix.Utils {
	/// <summary>
	/// Collection of utilities for calculating item stacking.
	/// </summary>
	internal static class ItemStacking {
		/// <summary>
		/// Represents a kind of item stacking formula.
		/// </summary>
		public enum Type {
			/// <summary>
			/// Effect scales by a fixed amount per stack.
			/// </summary>
			Linear = 0,
			/// <summary>
			/// Effect scales with diminishing returns per stack, approaching a limit.
			/// </summary>
			Hyperbolic = 1,
			/// <summary>
			/// Effect decreases proportionally per stack.
			/// </summary>
			Reciprocal = 2,
			/// <summary>
			/// Effect compounds by a constant factor per stack.
			/// </summary>
			Exponential = 3,
		}

		/// <summary>
		/// Calculates the result of linear stacking at the specified <paramref name="stacks"/> number.
		/// </summary>
		/// <param name="initial">The initial strength of the effect.</param>
		/// <param name="perStack">The constant strength added per stack.</param>
		/// <param name="stacks">The number of stacks to calculate for.</param>
		public static int Linear(int initial, int perStack, int stacks)
			=> initial + perStack * (stacks - 1);

		/// <inheritdoc cref="Linear(int, int, int)"/>
		public static float Linear(float initial, float perStack, int stacks)
			=> initial + perStack * (stacks - 1f);

		/// <summary>
		/// Calculates the result of hyperbolic stacking at the specified <paramref name="stacks"/> number.
		/// Approaches full strength.
		/// </summary>
		/// <param name="coefficient">Factor between 0 and 1 for how much the effect scales per stack.</param>
		/// <param name="stacks">The number of stacks to calculate for.</param>
		/// <remarks>The first stack will always be slightly off due to how hyperbolic stacking works.</remarks>
		public static float Hyperbolic(float coefficient, int stacks)
			=> DivideOrDefault(coefficient * stacks, 1f + coefficient * stacks, coefficient);

		/// <inheritdoc cref="Hyperbolic(float, int)"/>
		/// <param name="initial">Factor between 0 and 1 indicating the approximate initial strength of the effect.</param>
		public static float Hyperbolic(float initial, float coefficient, int stacks)
			=> DivideOrDefault(initial + coefficient * (stacks - 1f), 1f + initial + coefficient * (stacks - 1f), initial);

		/// <inheritdoc cref="Hyperbolic(float, int)"/>
		/// <param name="initial">Percentage between 0% and 100% indicating the approximate initial strength of the effect.</param>
		/// <param name="perStack">Percentage between 0% and 100% for how much the effect scales per stack.</param>
		public static float HyperbolicPercent(float initial, float perStack, int stacks)
			=> Hyperbolic(initial * 0.01f, perStack * 0.01f, stacks) * 100f;

		/// <summary>
		/// Calculates the result of reciprocal stacking at the specified <paramref name="stacks"/> number.
		/// Approaches zero.
		/// </summary>
		/// <param name="coefficient">Factor between 0 and 1 indicating how much the effect decreases from 1 per stack.</param>
		/// <param name="stacks">The number of stacks to calculate for.</param>
		/// <remarks>The effect is at a theoretical full strength with no stacks, and starts approaching zero at 1 stack.</remarks>
		public static float Reciprocal(float coefficient, int stacks)
			=> Reciprocal(coefficient, coefficient, stacks);

		/// <inheritdoc cref="Reciprocal(float, int)"/>
		/// <param name="initial">Factor between 0 and 1 indicating the initial decrease of the effect from 1 at 1 stack.</param>
		public static float Reciprocal(float initial, float coefficient, int stacks)
			=> DivideOrNumerator(1f - initial, 1f + coefficient * (stacks - 1f));

		/// <inheritdoc cref="Reciprocal(float, int)"/>
		/// <param name="initial">Percentage between 0% and 100% indicating the initial decrease of the effect from 100% at 1 stack.</param>
		/// <param name="coefficient">Percentage between 0% and 100% indicating how much the effect decreases from 100% per stack.</param>
		public static float ReciprocalPercent(float initial, float coefficient, int stacks)
			=> Reciprocal(initial * 0.01f, coefficient * 0.01f, stacks);

		/// <summary>
		/// Calculates the result of reciprocal stacking at the specified <paramref name="stacks"/> number.
		/// Grows or approaches zero depending on the the multiplier(s).
		/// </summary>
		/// <param name="multiplier">Base to exponentiate per stack.</param>
		/// <param name="stacks">The number of stacks to calculate for.</param>
		public static float Exponential(float multiplier, int stacks)
			=> MathF.Pow(multiplier, stacks);

		/// <inheritdoc cref="Exponential(float, int)"/>
		/// <param name="initial">Strength of the effect with 1 stack.</param>
		/// <param name="initial">Compounding multiplier per stack after the first one.</param>
		public static float Exponential(float initial, float perStack, int stacks)
			=> initial * MathF.Pow(perStack, stacks - 1f);

		private static float DivideOrNumerator(float numerator, float denominator)
			=> denominator == 0 ? numerator : (numerator / denominator);
		private static float DivideOrDefault(float numerator, float denominator, float defaultValue)
			=> denominator == 0 ? defaultValue : (numerator / denominator);
	}
}
