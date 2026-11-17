using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using StackFix.Compat;
using StackFix.Exceptions;
using StackFix.Settings;
using StackFix.Utils;

namespace StackFix.Items.Impl {
	internal class SonorousWhispers: ItemFix {
		private const int VANILLA_CHANCE_BASE_PERCENT = 5;
		private const int VANILLA_CHANCE_STACK_PERCENT = 1;
		private const float VANILLA_CHANCE_BASE = VANILLA_CHANCE_BASE_PERCENT * 0.01f;
		private const float VANILLA_CHANCE_STACK = VANILLA_CHANCE_STACK_PERCENT * 0.01f;
		private const ModSettings.SonorousWhispersChanceStacking VANILLA_CHANCE_STACK_TYPE
			= ModSettings.SonorousWhispersChanceStacking.Linear;

		private ConfigEntry<float> chanceBase;
		private ConfigEntry<float> chanceStack;
		private ConfigEntry<ModSettings.SonorousWhispersChanceStacking> chanceStackType;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled      = this.RegisterSetting(plugin, ModSettings.SonorousWhispersFixEnabled);
			this.chanceBase      = this.RegisterSetting(plugin, ModSettings.SonorousWhispersChanceBase);
			this.chanceStack     = this.RegisterSetting(plugin, ModSettings.SonorousWhispersChanceStack);
			this.chanceStackType = this.RegisterSetting(plugin, ModSettings.SonorousWhispersChanceStackType);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompat.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompat.AddSliderOption(this.chanceBase, new(Suffix: "%"));
			RiskOfOptionsCompat.AddSliderOption(this.chanceStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompat.AddChoiceOption(this.chanceStackType);
		}

		public override void RegisterLookingGlassStats() {
			float chanceBase() => this.Enabled ? this.chanceBase.Value * 0.01f : VANILLA_CHANCE_BASE;
			float chanceStack() => this.Enabled ? this.chanceStack.Value * 0.01f : VANILLA_CHANCE_STACK;
			ModSettings.SonorousWhispersChanceStacking chanceStackType()
				=> this.Enabled ? this.chanceStackType.Value : VANILLA_CHANCE_STACK_TYPE;

			LookingGlassCompat.AddProcChanceItem(DLC2Content.Items.ItemDropChanceOnKill, [
				new("Large Monster Drop Chance",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Percentage,
					(_, _) => 1f),
				new("Elite Drop Chance",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Percentage,
					(luck, itemCount) => RoR2Utils.ChanceWithLuck(
						chanceStackType() switch {
							ModSettings.SonorousWhispersChanceStacking.Linear
								=> ItemStacking.Linear(chanceBase(), chanceStack(), itemCount),

							ModSettings.SonorousWhispersChanceStacking.Hyperbolic
								=> ItemStacking.Hyperbolic(chanceBase(), chanceStack(), itemCount),

							_ => chanceBase(),
						},
						luck)),
			]);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float chanceBasePercent           = enabled ? this.chanceBase.Value : VANILLA_CHANCE_BASE_PERCENT;
			float chanceStackPercent          = enabled ? this.chanceStack.Value : VANILLA_CHANCE_STACK_PERCENT;
			ItemStacking.Type chanceStackType = ItemStacking.FromEnum(enabled ? this.chanceStackType.Value : VANILLA_CHANCE_STACK_TYPE);

			LanguageAPI.AddOverlay("ITEM_ITEMDROPCHANCEONKILL_DESC",
				$"When a large monster is killed, it will always drop an item. All elites have a {ItD.Utility($"{ItD.V(chanceBasePercent)}%")} chance " +
				$"of dropping an item{ItD.Stack<ModSettings.SonorousWhispersChanceStacking>(chanceStackPercent, chanceStackType, VANILLA_CHANCE_STACK_TYPE)}.");
		}

		protected override void ApplyHooks() {
			//TODO: HOOC
		}

		private float ModifyEliteDropChancePercent(float eliteDropChancePercent, int itemCount) {
			if(!this.Enabled)
				return eliteDropChancePercent;

			float initialPercent = this.chanceBase.Value;
			float perStackPercent = this.chanceStack.Value;

			return this.chanceStackType.Value switch {
				ModSettings.SonorousWhispersChanceStacking.Linear
					=> ItemStacking.Linear(initialPercent, perStackPercent, itemCount),

				ModSettings.SonorousWhispersChanceStacking.Hyperbolic
					=> ItemStacking.HyperbolicPercent(initialPercent, perStackPercent, itemCount),

				_ => initialPercent,
			};
		}
	}
}
