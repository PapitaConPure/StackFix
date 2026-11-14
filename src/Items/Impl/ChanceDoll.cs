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
	internal class ChanceDoll: ItemFix {
		private const int VANILLA_CHANCE_BASE_PERCENT = 40;
		private const int VANILLA_CHANCE_STACK_PERCENT = 10;
		private const float VANILLA_CHANCE_BASE = VANILLA_CHANCE_BASE_PERCENT * 0.01f;
		private const float VANILLA_CHANCE_STACK = VANILLA_CHANCE_STACK_PERCENT * 0.01f;
		private const ModSettings.ChanceDollChanceStacking VANILLA_CHANCE_STACK_TYPE
			= ModSettings.ChanceDollChanceStacking.Linear;

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
			RiskOfOptionsCompat.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompat.AddSliderOption(this.chanceBase, new(Suffix: "%"));
			RiskOfOptionsCompat.AddSliderOption(this.chanceStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompat.AddChoiceOption(this.chanceStackType);
		}

		public override void RegisterLookingGlassStats() {
			float chanceBase() => this.Enabled ? this.chanceBase.Value * 0.01f : VANILLA_CHANCE_BASE;
			float chanceStack() => this.Enabled ? this.chanceStack.Value * 0.01f : VANILLA_CHANCE_STACK;
			ModSettings.ChanceDollChanceStacking chanceStackType()
				=> this.Enabled ? this.chanceStackType.Value : VANILLA_CHANCE_STACK_TYPE;

			LookingGlassCompat.AddProcChanceItem(DLC2Content.Items.ExtraShrineItem, [
				new("Chance",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Percentage,
					(luck, itemCount) => RoR2Utils.ChanceWithLuck(
						chanceStackType() switch {
							ModSettings.ChanceDollChanceStacking.Linear
								=> ItemStacking.Linear(chanceBase(), chanceStack(), itemCount),

							ModSettings.ChanceDollChanceStacking.Hyperbolic
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

			LanguageAPI.AddOverlay("ITEM_EXTRASHRINEITEM_DESC",
				$"On Shrine of Chance success, {ItD.Utility($"{ItD.V(chanceBasePercent)}%")}" +
				$"{ItD.Stack<ModSettings.ChanceDollChanceStacking>(chanceStackPercent, chanceStackType, VANILLA_CHANCE_STACK_TYPE)} chance " +
				"to get higher rarity items.");
		}

		protected override void ApplyHooks() {
			IL.RoR2.ShrineChanceBehavior.AddShrineStack += this.ShrineChanceBehavior_AddShrineStack;
		}

		private void ShrineChanceBehavior_AddShrineStack(ILContext il) {
			ILCursor cursor = new(il);

			int itemCountEffectiveLocalIndex = default;
			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdsfld(typeof(DLC2Content.Items), nameof(DLC2Content.Items.ExtraShrineItem)),
				instr => instr.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCountEffective)),
				instr => instr.MatchStloc(out itemCountEffectiveLocalIndex)))
				throw new ILSeekException(il, "itemCountEffective assignment", nameof(ShrineChanceBehavior.AddShrineStack));

			if(!cursor.TryGotoNext(MoveType.Before,
				instr => instr.MatchLdloc(out _),
				instr => instr.MatchCallOrCallvirt(typeof(CharacterBody), "get_master"),
				instr => instr.MatchCallOrCallvirt(typeof(Util).FullName, nameof(Util.CheckRoll))))
				throw new ILSeekException(il, "CheckRoll call", nameof(ShrineChanceBehavior.AddShrineStack));

			if(!cursor.TryGotoPrev(MoveType.After, instr => instr.MatchConvR4()))
				throw new ILSeekException(il, "jackpot chance f32 conversion", nameof(ShrineChanceBehavior.AddShrineStack));

			cursor.Emit(OpCodes.Ldloc, itemCountEffectiveLocalIndex);
			cursor.EmitDelegate(this.ModifyJackpotChancePercent);
		}

		private float ModifyJackpotChancePercent(float jackpotChancePercent, int itemCount) {
			if(!this.Enabled)
				return jackpotChancePercent;

			float initialPercent = this.chanceBase.Value;
			float perStackPercent = this.chanceStack.Value;

			return this.chanceStackType.Value switch {
				ModSettings.ChanceDollChanceStacking.Linear
					=> ItemStacking.Linear(initialPercent, perStackPercent, itemCount),

				ModSettings.ChanceDollChanceStacking.Hyperbolic
					=> ItemStacking.HyperbolicPercent(initialPercent, perStackPercent, itemCount),

				_ => initialPercent,
			};
		}
	}
}
