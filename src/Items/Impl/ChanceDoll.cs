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
			float chanceBasePercent           = enabled ? this.chanceBase.Value : VANILLA_CHANCE_BASE_PERCENT;
			float chanceStackPercent          = enabled ? this.chanceStack.Value : VANILLA_CHANCE_STACK_PERCENT;
			ItemStacking.Type chanceStackType = enabled ? ItemStacking.FromEnum(this.chanceStackType.Value) : VANILLA_CHANCE_STACK_TYPE;

			LanguageAPI.AddOverlay("ITEM_EXTRASHRINEITEM_DESC", new ItD(
				$"On Shrine of Chance success, {ItD.Utility($"{chanceBasePercent:0.##}%")}",
				$"{ItD.Stack(chanceStackPercent, chanceStackType, "+{0:0.##}%", VANILLA_CHANCE_STACK_TYPE)} chance to get ",
				$"higher rarity items."));
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
				throw new ILSeekException(il, "itemCountEffective assignment", "ShrineChanceBehavior.AddShrineStack");

			if(!cursor.TryGotoNext(MoveType.Before,
				instr => instr.MatchLdloc(out int _),
				instr => instr.MatchCallOrCallvirt(typeof(CharacterBody), "get_master"),
				instr => instr.MatchCallOrCallvirt(typeof(Util).FullName, nameof(Util.CheckRoll))))
				throw new ILSeekException(il, "CheckRoll call", "ShrineChanceBehavior.AddShrineStack");

			if(!cursor.TryGotoPrev(MoveType.After, instr => instr.MatchConvR4()))
				throw new ILSeekException(il, "jackpot chance f32 conversion", "ShrineChanceBehavior.AddShrineStack");

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
