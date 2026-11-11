using System;

namespace StackFix.Utils {
	internal static class RoR2Utils {
		/// <summary>
		/// Returns a proc chance affected by luck.
		/// </summary>
		/// <param name="chance">The proc chance. From 0 to 1.</param>
		/// <param name="luck">The luck value (cloverCount - purityCount).</param>
		/// <returns></returns>
		public static float ChanceWithLuck(float chance, float luck) {
			chance = MathF.Min(chance, 1f);

			int rolls = (int)MathF.Ceiling(luck);

			if(rolls > 0)
				return 1f - MathF.Pow(1f - chance, rolls + 1);

			if(rolls < 0)
				return MathF.Pow(chance, -rolls + 1);

			return chance;
		}
	}
}
