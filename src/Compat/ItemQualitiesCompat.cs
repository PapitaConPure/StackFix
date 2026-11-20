using ItemQualities;
using R2API;
using RoR2;
using StackFix.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace StackFix.Compat {
	internal class ItemQualitiesCompat {
		public const string GUID = "com.Gorakh.ItemQualities";

		public static bool Installed => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(GUID);

		/// <summary>
		/// Applies the supplied <paramref name="patterns"/> to the description of every quality variant
		/// of the specified <paramref name="itemToken"/>.
		/// </summary>
		/// <remarks>The patterns are applied in order.</remarks>
		/// <param name="itemToken">Vanilla item token or internal item name with no spaces nor underscores.</param>
		/// <param name="patterns">Replacement patterns in the correct order.</param>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void ApplyQualityDescriptions(string itemToken, ReplacementPattern[] patterns) {
			if(ItemCatalog.availability.available)
				apply();
			else
				ItemCatalog.availability.CallWhenAvailable(apply);

			void apply() {
				string strippedItemToken = StripItemToken(itemToken);

				for(QualityTier qualityTier = 0; qualityTier < QualityTier.Count; qualityTier++) {
					string qualityTierName = qualityTier.ToString().ToUpper();
					string qualityToken = $"ITEM_{strippedItemToken}_{qualityTierName}_DESC";

					string qualityDescription = Language.english.GetLocalizedStringByToken(qualityToken)
						?? throw new InvalidOperationException(
							$"Couldn't find ItemQualities description token: '{qualityToken}' for item: '{strippedItemToken}', quality: '{qualityTierName}'");

	#if DEBUG
					UnityEngine.Debug.Log($"qualityDescription = {qualityDescription}", Plugin.Instance);
	#endif

					string finalDescription = ApplyReplacementPatterns(qualityDescription, patterns);

	#if DEBUG
					UnityEngine.Debug.Log($"finalDescription = {finalDescription}", Plugin.Instance);
	#endif

					LanguageAPI.AddOverlay(qualityToken, finalDescription);
				}
			}
		}

		/// <summary>
		/// Applies the supplied <paramref name="zeroFormatReplacementDescription"/> to the "{0}" format replacement indicator
		/// within the description of every quality variant of the specified <paramref name="itemToken"/>.
		/// </summary>
		/// <remarks>Only use this for Quality items that use the "{0}" format replacement indicator.</remarks>
		/// <param name="itemToken">Vanilla item token or internal item name with no spaces nor underscores.</param>
		/// <param name="patterns">Replacement patterns in the correct order.</param>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void ApplyQualityDescriptions(string itemToken, string zeroFormatReplacementDescription) {
			if(ItemCatalog.availability.available)
				apply();
			else
				ItemCatalog.availability.CallWhenAvailable(apply);

			void apply() {
				string strippedItemToken = StripItemToken(itemToken);

				for(QualityTier qualityTier = 0; qualityTier < QualityTier.Count; qualityTier++) {
					string qualityTierName = qualityTier.ToString().ToUpper();
					string qualityToken = $"ITEM_{strippedItemToken}_{qualityTierName}_DESC";

					string qualityDescription = Language.english.GetLocalizedStringByToken(qualityToken)
						?? throw new InvalidOperationException(
							$"Couldn't find ItemQualities description token: '{qualityToken}' for item: '{strippedItemToken}', quality: '{qualityTierName}'");

					if(!qualityDescription.Contains("{0}"))
						throw new InvalidOperationException(
							$"Cannot replace zeroth format replacement indicator (\"{{0}}\") for ItemQualities description token: '{qualityToken}' " +
							$"for item: '{strippedItemToken}', quality: '{qualityTierName}'");

					string finalDescription = string.Format(qualityDescription, zeroFormatReplacementDescription);

					LanguageAPI.AddOverlay(qualityToken, finalDescription);
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static string StripItemToken(string itemToken) {
			itemToken = itemToken.Trim().ToUpper();

			if(string.IsNullOrEmpty(itemToken))
				throw new ArgumentNullException(nameof(itemToken));

			if(itemToken.StartsWith("ITEM_", StringComparison.Ordinal))
				itemToken = itemToken["ITEM_".Length..];

			if(itemToken.EndsWith("_DESC", StringComparison.Ordinal))
				itemToken = itemToken[..^"_DESC".Length];

			return itemToken;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static string ApplyReplacementPatterns(string qualityDescription, IEnumerable<ReplacementPattern> patterns) {
			StringBuilder finalDescriptionBuilder = new();

			int lastReplacementEndIndex = 0;
			foreach(ReplacementPattern replacementPattern in patterns) {
				if(lastReplacementEndIndex >= qualityDescription.Length) {
					UnityEngine.Debug.LogWarning("Couldn't apply all StackFix pattern replacements before reaching the end of the quality-tiered item description.", Plugin.Instance);
					break;
				}

				(IMatchablePattern pattern, string replacementFormat, IEnumerable<object> replacementArgs) = replacementPattern;

				if(pattern == null)
					throw new ArgumentNullException(nameof(replacementPattern.Pattern));

#if DEBUG
				UnityEngine.Debug.Log($"Applying {pattern} from string index {lastReplacementEndIndex}", Plugin.Instance);
#endif

				MatchResult match = pattern.Match(qualityDescription, lastReplacementEndIndex);

				if(!match.Success) {
					UnityEngine.Debug.LogWarning($"Couldn't match {pattern} from string index {lastReplacementEndIndex}, to apply StackFix replacement.");
					continue;
				}

				string previous = qualityDescription[lastReplacementEndIndex..match.Index];
				string replacement = string.Format(CultureInfo.InvariantCulture, replacementFormat, [.. replacementArgs]);

				finalDescriptionBuilder.Append(previous);
				finalDescriptionBuilder.Append(replacement);

				lastReplacementEndIndex = match.Index + match.Length;

#if DEBUG
				UnityEngine.Debug.Log($"Matched {pattern} and appended to final string:\n'{previous}'\n'{replacement}'", Plugin.Instance);
#endif
			}

			if(lastReplacementEndIndex < qualityDescription.Length) {
				string remainder = qualityDescription[lastReplacementEndIndex..];
				finalDescriptionBuilder.Append(remainder);
			}

			return finalDescriptionBuilder.ToString();
		}
	}
}
