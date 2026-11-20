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
	internal sealed class SonorousWhispers: ItemFix {
		private const float VANILLA_CHANCE_BASE_PERCENT = 4f;
		private const float VANILLA_ACTUAL_CHANCE_BASE_PERCENT = 5f;
		private const float VANILLA_CHANCE_STACK_PERCENT = 1f;
		private const float VANILLA_CHANCE_BASE = VANILLA_CHANCE_BASE_PERCENT * 0.01f;
		private const float VANILLA_ACTUAL_CHANCE_BASE = VANILLA_ACTUAL_CHANCE_BASE_PERCENT * 0.01f;
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
			float chanceBase() => this.Enabled ? this.chanceBase.Value * 0.01f : VANILLA_ACTUAL_CHANCE_BASE;
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
					(luck, itemCount) => RoR2Utils.LuckChance(luck,
						chanceStackType() switch {
							ModSettings.SonorousWhispersChanceStacking.Linear
								=> ItemStacking.Linear(chanceBase(), chanceStack(), itemCount),

							ModSettings.SonorousWhispersChanceStacking.Hyperbolic
								=> ItemStacking.Hyperbolic(chanceBase(), chanceStack(), itemCount),

							_ => chanceBase(),
						})),
			]);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float chanceBasePercent           = enabled ? this.chanceBase.Value : VANILLA_ACTUAL_CHANCE_BASE_PERCENT;
			float chanceStackPercent          = enabled ? this.chanceStack.Value : VANILLA_CHANCE_STACK_PERCENT;
			ItemStacking.Type chanceStackType = ItemStacking.FromEnum(enabled ? this.chanceStackType.Value : VANILLA_CHANCE_STACK_TYPE);

			string description =
				$"When a large monster is killed, it will always drop an item. All elites have a {ItD.Utility($"{ItD.V(chanceBasePercent)}%")} chance " +
				$"of dropping an item{ItD.Stack<ModSettings.SonorousWhispersChanceStacking>(chanceStackPercent, chanceStackType, "+{0:0.##}%", VANILLA_CHANCE_STACK_TYPE)}.";

			LanguageAPI.AddOverlay("ITEM_ITEMDROPCHANCEONKILL_DESC", description);

			if(ItemQualitiesCompat.Installed)
				ItemQualitiesCompat.ApplyQualityDescriptions("ITEMDROPCHANCEONKILL", description);
		}

		protected override void ApplyHooks() {
			IL.RoR2.GlobalEventManager.OnCharacterDeath += this.GlobalEventManager_OnCharacterDeath;
		}

		private void GlobalEventManager_OnCharacterDeath(ILContext il) {
			ILCursor cursor = new(il);

			int itemCountEffectiveLocalIndex = default;
			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdsfld(typeof(DLC2Content.Items), nameof(DLC2Content.Items.ItemDropChanceOnKill)),
				instr => instr.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCountEffective)),
				instr => instr.MatchStloc(out itemCountEffectiveLocalIndex)))
				throw new ILSeekException(il, "itemCountEffective assignment", "GlobalEventManager.OnCharacterDeath");

			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchCallOrCallvirt<CharacterBody>("get_isElite")))
				throw new ILSeekException(il, "isElite call", "GlobalEventManager.OnCharacterDeath");

			Instruction isEliteInstr = cursor.Prev;

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_CHANCE_BASE_PERCENT)))
				throw new ILSeekException(il, "base elite drop chance", "GlobalEventManager.OnCharacterDeath");

			Instruction chanceBaseInstr = cursor.Prev;

			cursor.Goto(isEliteInstr, MoveType.After);
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_CHANCE_STACK_PERCENT)))
				throw new ILSeekException(il, "stack elite drop chance", "GlobalEventManager.OnCharacterDeath");

			cursor.EmitDelegate(this.NullifyVanillaStackEliteDropChancePercent);

			cursor.Goto(chanceBaseInstr, MoveType.After);
			cursor.Emit(OpCodes.Ldloc, itemCountEffectiveLocalIndex);
			cursor.EmitDelegate(this.ModifyEliteDropChancePercent);
		}

		private float ModifyEliteDropChancePercent(float baseEliteDropChancePercent, int itemCount) {
			if(!this.Enabled)
				return baseEliteDropChancePercent;

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

		private float NullifyVanillaStackEliteDropChancePercent(float stackEliteDropChancePercent)
			=> this.Enabled ? 0f : stackEliteDropChancePercent;
	}
}
