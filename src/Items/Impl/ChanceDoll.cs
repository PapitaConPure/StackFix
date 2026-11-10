using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using R2API;
using StackFix.Compat;
using StackFix.Settings;
using StackFix.Utils;
using System;

namespace StackFix.Items.Impl {
	internal class ChanceDoll: ItemFix {
		private const float VANILLA_CHANCE_BASE = 0.4f;
		private const float VANILLA_CHANCE_STACK = 0.1f;
		private const ItemStacking.Type VANILLA_CHANCE_STACK_TYPE = ItemStacking.Type.Linear;

		private ConfigEntry<float> chanceBase;
		private ConfigEntry<float> chanceStack;
		private ConfigEntry<ModSettings.ChanceDollChanceStacking> chanceStackType;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled      = this.RegisterSetting(plugin, ModSettings.ChanceDollFixEnabled);
			this.chanceBase      = this.RegisterSetting(plugin, ModSettings.ChanceDollChanceBase);
			this.chanceStack     = this.RegisterSetting(plugin, ModSettings.ChanceDollChanceStack);
			this.chanceStackType = this.RegisterSetting(plugin, ModSettings.ChanceDollChanceStackType);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompatManager.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompatManager.AddSliderOption(this.chanceBase, new(Suffix: "%"));
			RiskOfOptionsCompatManager.AddSliderOption(this.chanceStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompatManager.AddChoiceOption(this.chanceStackType);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float chanceBase                  = enabled ? this.chanceBase.Value : VANILLA_CHANCE_BASE;
			float chanceStack                 = enabled ? this.chanceStack.Value : VANILLA_CHANCE_STACK;
			ItemStacking.Type chanceStackType = enabled ? ItemStacking.FromEnum(this.chanceStackType.Value) : VANILLA_CHANCE_STACK_TYPE;

			LanguageAPI.AddOverlay("ITEM_EXTRASHRINEITEM_DESC", new ItD(
				$"On Shrine of Chance success, {ItD.Utility($"{chanceBase:0.##}%")}",
				$"{ItD.Stack(chanceStack, chanceStackType, "+{0:0.##}%", VANILLA_CHANCE_STACK_TYPE)} chance to get higher rarity items."));
		}

		protected override void ApplyHooks() {
			IL.RoR2.ShrineChanceBehavior.AddShrineStack += this.ShrineChanceBehavior_AddShrineStack;
		}

		private void ShrineChanceBehavior_AddShrineStack(ILContext il) {
			ILCursor cursor = new(il);

			throw new NotImplementedException();
		}
	}
}
