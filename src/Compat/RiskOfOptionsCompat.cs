using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using System;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
using ConfigEntryExt;

namespace StackFix.Compat {
	internal static class RiskOfOptionsCompat {
		public const string GUID = "com.rune580.riskofoptions";

		public static bool Installed => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(GUID);

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetModIcon(UnityEngine.Sprite sprite) {
			ModSettingsManager.SetModIcon(sprite);
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetModDescription(string desc) {
			ModSettingsManager.SetModDescription(desc);
		}

		public record AddCheckboxOptionConfig(bool RestartRequired = false);

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddCheckboxOption(ConfigEntryExt<bool> entry, AddCheckboxOptionConfig config = null) {
			ModSettingsManager.AddOption(new CheckBoxOption(
				entry.ConfigEntry,
				new CheckBoxConfig() {
					restartRequired = config?.RestartRequired ?? false,
					description = entry.Description,
				}));
		}

		public enum TextInputOptionLineType {
			Single,
			MultiSubmit,
			MultiNewline,
		}
		public record AddTextInputOptionConfig(
			bool RestartRequired = false,
			bool RichText = false,
			TextInputOptionLineType LineType = TextInputOptionLineType.Single);

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddTextInputOption(ConfigEntryExt<string> entry, AddTextInputOptionConfig config = null) {
			ModSettingsManager.AddOption(new StringInputFieldOption(
				entry.ConfigEntry,
				new InputFieldConfig() {
					restartRequired = config?.RestartRequired ?? false,
					description = entry.Description,
					richText = config?.RichText ?? false,
					lineType = (config?.LineType) switch {
						TextInputOptionLineType.MultiSubmit => TMPro.TMP_InputField.LineType.MultiLineSubmit,
						TextInputOptionLineType.MultiNewline => TMPro.TMP_InputField.LineType.MultiLineNewline,
						_ => TMPro.TMP_InputField.LineType.SingleLine,
					},
				}));
		}

		public record AddIntSliderOptionConfig(bool RestartRequired = false, string FormatString = "{0}", string Prefix = "", string Suffix = "");

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddSliderOption(
			ConfigEntryExt<int> entry,
			AddIntSliderOptionConfig config = null) {
			GetRangeFromEntry(entry, out int min, out int max);

			ModSettingsManager.AddOption(new IntSliderOption(
				entry.ConfigEntry,
				new IntSliderConfig {
					min = min,
					max = max,
					restartRequired = config?.RestartRequired ?? false,
					description = entry.Description,
					formatString = $"{config?.Prefix}{config?.FormatString ?? "{0}"}{config?.Suffix}",
				}));
		}

		public record AddFloatSliderOptionConfig(bool RestartRequired = false, string FormatString = "{0:0.##}", string Prefix = "", string Suffix = "", float Step = 0.01f);

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddSliderOption(
			ConfigEntryExt<float> entry,
			AddFloatSliderOptionConfig config = null) {
			GetRangeFromEntry(entry, out float min, out float max);

			if(config != null && config.Step > 0)
				ModSettingsManager.AddOption(new StepSliderOption(
				entry.ConfigEntry,
				new StepSliderConfig {
					min = min,
					max = max,
					restartRequired = config.RestartRequired,
					description = entry.Description,
					FormatString = $"{config.Prefix}{config.FormatString ?? "{0:0.##}"}{config.Suffix}",
					increment = config.Step,
				}));
			else
				ModSettingsManager.AddOption(new SliderOption(
					entry.ConfigEntry,
					new SliderConfig {
						min = min,
						max = max,
						restartRequired = config?.RestartRequired ?? false,
						FormatString = $"{config?.Prefix ?? ""}{config?.FormatString ?? "{0:0.##}"}{config?.Suffix ?? ""}",
					}));
		}

		public record AddChoiceOptionConfig(bool RestartRequired = false);

		public static void AddChoiceOption<TEnum>(ConfigEntryExt<TEnum> entry, AddChoiceOptionConfig config = null) where TEnum : struct, Enum {
			ModSettingsManager.AddOption(new ChoiceOption(
				entry.ConfigEntry,
				new ChoiceConfig {
					restartRequired = config?.RestartRequired ?? false,
					description = entry.Description,
				}));
		}

		private static void GetRangeFromEntry(ConfigEntryExt<int> entry, out int min, out int max) {
			if(entry.ConfigEntry.Description?.AcceptableValues is AcceptableValueRange<int> range) {
				min = range.MinValue;
				max = range.MaxValue;
			} else {
				min = int.MinValue;
				max = int.MaxValue;
			}
		}

		private static void GetRangeFromEntry(ConfigEntryExt<float> entry, out float min, out float max) {
			if(entry.ConfigEntry.Description?.AcceptableValues is AcceptableValueRange<float> range) {
				min = range.MinValue;
				max = range.MaxValue;
			} else {
				min = float.MinValue;
				max = float.MaxValue;
			}
		}
	}
}
