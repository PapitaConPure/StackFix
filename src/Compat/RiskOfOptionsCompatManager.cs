using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using System;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace StackFix.Compat {
	internal static class RiskOfOptionsCompatManager {
		private static bool? enabled;

		public static bool Enabled {
			get {
				enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
				return (bool)enabled;
			}
		}

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
		public static void AddCheckboxOption(ConfigEntry<bool> entry, AddCheckboxOptionConfig config = null) {
			ModSettingsManager.AddOption(new CheckBoxOption(
				entry,
				new CheckBoxConfig() {
					restartRequired = config?.RestartRequired ?? false,
				}));
		}

		public record AddIntSliderOptionConfig(bool RestartRequired = false, string FormatString = "{0}", string Prefix = "", string Suffix = "");

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddSliderOption(
			ConfigEntry<int> entry,
			AddIntSliderOptionConfig config = null) {
			GetRangeFromEntry(entry, out int min, out int max);

			ModSettingsManager.AddOption(new IntSliderOption(
				entry,
				new IntSliderConfig {
					min = min,
					max = max,
					restartRequired = config?.RestartRequired ?? false,
					formatString = $"{config?.Prefix}{config?.FormatString ?? "{0}"}{config?.Suffix}",
				}));
		}

		public record AddFloatSliderOptionConfig(bool RestartRequired = false, string FormatString = "{0:0.##}", string Prefix = "", string Suffix = "", float Step = 0.01f);

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddSliderOption(
			ConfigEntry<float> entry,
			AddFloatSliderOptionConfig config = null) {
			GetRangeFromEntry(entry, out float min, out float max);

			if(config != null && config.Step > 0)
				ModSettingsManager.AddOption(new StepSliderOption(
				entry,
				new StepSliderConfig {
					min = min,
					max = max,
					restartRequired = config.RestartRequired,
					FormatString = $"{config.Prefix}{config.FormatString ?? "{0:0.##}"}{config.Suffix}",
					increment = config.Step,
				}));
			else
				ModSettingsManager.AddOption(new SliderOption(
					entry,
					new SliderConfig {
						min = min,
						max = max,
						restartRequired = config?.RestartRequired ?? false,
						FormatString = $"{config?.Prefix ?? ""}{config?.FormatString ?? "{0:0.##}"}{config?.Suffix ?? ""}",
					}));
		}

		public record AddChoiceOptionConfig(bool RestartRequired = false);

		public static void AddChoiceOption<TEnum>(ConfigEntry<TEnum> entry, AddChoiceOptionConfig config = null) where TEnum : struct, Enum {
			ModSettingsManager.AddOption(new ChoiceOption(
				entry,
				new ChoiceConfig {
					restartRequired = config?.RestartRequired ?? false,
				}));
		}

		private static void GetRangeFromEntry(ConfigEntry<int> entry, out int min, out int max) {
			if(entry.Description?.AcceptableValues is AcceptableValueRange<int> range) {
				min = range.MinValue;
				max = range.MaxValue;
			} else {
				min = int.MinValue;
				max = int.MaxValue;
			}
		}

		private static void GetRangeFromEntry(ConfigEntry<float> entry, out float min, out float max) {
			if(entry.Description?.AcceptableValues is AcceptableValueRange<float> range) {
				min = range.MinValue;
				max = range.MaxValue;
			} else {
				min = float.MinValue;
				max = float.MaxValue;
			}
		}
	}
}
