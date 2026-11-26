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

		private static readonly Dictionary<string, string> vanillaDescriptions = [];

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

#if DEBUG
				Plugin.Logging.LogDebug($"Applying pattern-based ItemQualities descriptions for item: {itemToken}");
#endif

				string strippedItemToken = StripItemToken(itemToken);

				for(QualityTier qualityTier = 0; qualityTier < QualityTier.Count; qualityTier++) {
					string qualityTierName = qualityTier.ToString().ToUpper();
					string qualityToken = $"ITEM_{strippedItemToken}_{qualityTierName}_DESC";
					string qualityDescription = SaveOrRestoreVanillaDescription(qualityToken);

#if DEBUG
					Plugin.Logging.LogDebug($"qualityDescription = {qualityDescription}");
#endif

					string finalDescription = ApplyReplacementPatterns(qualityDescription, patterns);

#if DEBUG
					Plugin.Logging.LogDebug($"finalDescription = {finalDescription}");
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
					string qualityDescription = SaveOrRestoreVanillaDescription(qualityToken);

					if(!qualityDescription.Contains("{0}"))
						throw new InvalidOperationException(
							$"Cannot replace zeroth format replacement indicator (\"{{0}}\") for ItemQualities description token: '{qualityToken}' " +
							$"for item: '{strippedItemToken}', quality: '{qualityTierName}'\nDescription was:\n\"{qualityDescription}\"");

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
					Plugin.Logging.LogWarning("Couldn't apply all StackFix pattern replacements before reaching the end of the quality-tiered item description.");
					break;
				}

				(IMatchablePattern pattern, string replacementFormat, IEnumerable<object> replacementArgs) = replacementPattern;

				if(pattern == null)
					throw new ArgumentNullException(nameof(replacementPattern.Pattern));

				MatchResult match = pattern.Match(qualityDescription, lastReplacementEndIndex);

				if(!match.Success) {
					Plugin.Logging.LogWarning($"Couldn't match {pattern} from string index {lastReplacementEndIndex}, to apply StackFix replacement.");
					continue;
				}

				string previous = qualityDescription[lastReplacementEndIndex..match.Index];
				string replacement = string.Format(CultureInfo.InvariantCulture, replacementFormat, [.. replacementArgs]);

				finalDescriptionBuilder.Append(previous);
				finalDescriptionBuilder.Append(replacement);

				lastReplacementEndIndex = match.Index + match.Length;

#if DEBUG
				Plugin.Logging.LogDebug($"Matched {pattern} and appended to final string:\n\"{previous}\"\n\"{replacement}\"");
#endif
			}

			if(lastReplacementEndIndex < qualityDescription.Length) {
				string remainder = qualityDescription[lastReplacementEndIndex..];
				finalDescriptionBuilder.Append(remainder);
			}

			return finalDescriptionBuilder.ToString();
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static string SaveOrRestoreVanillaDescription(string qualityToken) {
			bool descriptionExists = vanillaDescriptions.TryGetValue(qualityToken, out string restoredDescription);

			if(descriptionExists) {
#if DEBUG
				Plugin.Logging.LogDebug($"Restored original ItemQualities description for token: {qualityToken}");
#endif

				return restoredDescription;
			}

			string vanillaDescription = Language.english.GetLocalizedStringByToken(qualityToken)
				?? throw new InvalidOperationException(
					$"Couldn't get original ItemQualities description for token: '{qualityToken}'");

			vanillaDescriptions.Add(qualityToken, vanillaDescription);

#if DEBUG
			Plugin.Logging.LogDebug($"Stored original ItemQualities description for token: '{qualityToken}'");
#endif

			return vanillaDescription;
		}
	}
}
