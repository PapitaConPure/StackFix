///==================================================================
///Heavily based on this guide:
///https://github.com/Phreelosu/BaseGameFixes/blob/main/docs/LookingGlass-Item-Stats-Guide.md#step-3-call-init-from-your-plugin
///(thanks! <3)
///==================================================================

using LookingGlass.ItemStatsNameSpace;
using RoR2;
using StackFix.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StackFix.Compat {
	internal class LookingGlassCompat {
		public const string GUID = "droppod.lookingglass";

		public static bool Installed => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(GUID);

		public enum StatType {
			Healing,
			Damage,
			Utility,
			Health,
			Void,
			HumanObjective,
			LunarObjective,
			Stack,
			WorldEvent,
			Artifact,
			UserSetting,
			Death,
			Sub,
			Mono,
			Shrine,
			Event,
			Gold,
			Armor,
			None,
		}

		public enum StatUnits {
			/// <summary>
			/// Displays "{X}m".
			/// </summary>
			Meters,
			/// <summary>
			/// Expects a FRACTION! Displays "{X * 100f}%".
			/// </summary>
			Percentage,
			/// <summary>
			/// Displays "{X} flat HP".
			/// </summary>
			FlatHealth,
			/// <summary>
			/// Displays "{X} HP or HP/s".
			/// </summary>
			FlatHealing,
			/// <summary>
			/// Displays "{X}".
			/// </summary>
			Number,
			/// <summary>
			/// Displays "${X}".
			/// </summary>
			Money,
			/// <summary>
			/// Displays "{X}s".
			/// </summary>
			Seconds,
			/// <summary>
			/// Expects a FRACTION! Displays "{X * 100f}% of max HP".
			/// </summary>
			PercentHealth,
			/// <summary>
			/// Expects a FRACTION! Displays "{X * 100f}% HP/s".
			/// </summary>
			PercentHealing,
			ProcCoeff,
			PlainString,
		}

		//Avoid publicly referencing the LookingGlass assembly enums
		public record StatEntry<TFunc>(string Label, StatType Type, StatUnits Units, TFunc ValueFn) where TFunc : Delegate;

		/// <summary>
		/// Values that only depend on the stack count.
		/// </summary>
		/// <param name="itemDef">The item definition.</param>
		/// <param name="stats">The display specifier for each stat.</param>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddItem(ItemDef itemDef, params StatEntry<Func<int, float>>[] stats) {
			ItemStatsDef itemStatsDef = Create(stats);
			itemStatsDef.calculateValuesFlat = itemCount => [.. stats.Select(l => l.ValueFn(itemCount))];
			Register(itemDef, itemStatsDef);
		}

		/// <summary>
		/// Values that include a proc chance, which LookingGlass should show adjusted for luck.
		/// </summary>
		/// <param name="itemDef">The item definition.</param>
		/// <param name="stats">The display specifier for each stat.</param>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddProcChanceItem(ItemDef itemDef, params StatEntry<Func<float, int, float>>[] stats) {
			ItemStatsDef itemStatsDef = Create(stats);
			itemStatsDef.hasChance = true;
			itemStatsDef.calculateValuesNew = (luck, itemCount, procChance) => [.. stats.Select(l => l.ValueFn(luck, itemCount))];
			Register(itemDef, itemStatsDef);
		}

		/// <summary>
		/// Values that depend on the holder's other items. master can be null (e.g. in the logbook).
		/// </summary>
		/// <param name="itemDef">The item definition.</param>
		/// <param name="stats">The display specifier for each stat.</param>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddMasterItem(ItemDef itemDef, params StatEntry<Func<CharacterMaster, int, float>>[] stats) {
			ItemStatsDef itemStatsDef = Create(stats);
			itemStatsDef.calculateValues = (master, itemCount) => [.. stats.Select(l => l.ValueFn(master, itemCount))];
			Register(itemDef, itemStatsDef);
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static ItemStatsDef Create<TFunc>(StatEntry<TFunc>[] stats) where TFunc : Delegate {
			ItemStatsDef itemStatsDef = new();

			foreach(StatEntry<TFunc> entry in stats) {
				itemStatsDef.descriptions.Add($"{entry.Label}: ");
				itemStatsDef.valueTypes.Add(typeMap[entry.Type]);
				itemStatsDef.measurementUnits.Add(unitMap[entry.Units]);
			}

			return itemStatsDef;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void Register(ItemDef itemDef, ItemStatsDef itemStatsDef) {
			if(!itemDef || itemDef.itemIndex == ItemIndex.None)
				return;

			int itemIndex = (int)itemDef.itemIndex;
			ItemDefinitions.allItemDefinitions[itemIndex] = itemStatsDef;
		}

		private static readonly Dictionary<StatType, ItemStatsDef.ValueType> typeMap =
			typeof(StatType)
				.GetEnumValues()
				.OfType<StatType>()
				.ToDictionary(
					t => t,
					EnumUtils.EnumToEnum<StatType, ItemStatsDef.ValueType>);

		private static readonly Dictionary<StatUnits, ItemStatsDef.MeasurementUnits> unitMap =
			typeof(StatUnits)
				.GetEnumValues()
				.OfType<StatUnits>()
				.ToDictionary(
					t => t,
					EnumUtils.EnumToEnum<StatUnits, ItemStatsDef.MeasurementUnits>);
	}
}
