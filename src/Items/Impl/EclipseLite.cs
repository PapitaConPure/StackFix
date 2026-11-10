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
	internal sealed class EclipseLite: ItemFix {
		private const float VANILLA_BARRIER_BASE = 0.01f;
		private const float VANILLA_BARRIER_STACK = 0.0025f;

		private ConfigEntry<float> barrierBase;
		private ConfigEntry<float> barrierStack;
		private ConfigEntry<ModSettings.EclipseLiteBarrierStacking> barrierStackType;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled       = this.RegisterSetting(plugin, ModSettings.EclipseLiteFixEnabled);
			this.barrierBase      = this.RegisterSetting(plugin, ModSettings.EclipseLiteBarrierBase);
			this.barrierStack     = this.RegisterSetting(plugin, ModSettings.EclipseLiteBarrierStack);
			this.barrierStackType = this.RegisterSetting(plugin, ModSettings.EclipseLiteBarrierStackType);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompatManager.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompatManager.AddSliderOption(this.barrierBase, new(Suffix: "%"));
			RiskOfOptionsCompatManager.AddSliderOption(this.barrierStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompatManager.AddChoiceOption(this.barrierStackType);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float barrierBase                  = enabled ? this.barrierBase.Value : VANILLA_BARRIER_BASE * 100f;
			float barrierStack                 = enabled ? this.barrierStack.Value : VANILLA_BARRIER_STACK * 100f;
			ItemStacking.Type barrierStackType = enabled ? ItemStacking.FromEnum(this.barrierStackType.Value) : ItemStacking.Type.Linear;

			LanguageAPI.AddOverlay("ITEM_BARRIERONCOOLDOWN_DESC", new ItD(
				$"When a skill comes off cooldown, gain a {ItD.Damage("temporary barrier")} for {ItD.Healing($"{barrierBase:0.##}%")}",
				$"{ItD.Stack(barrierStack, barrierStackType, "{0:+0.##;-0.##}%", ItemStacking.Type.Linear)} ",
				$"of your maximum health per second of the skill's base cooldown."));
		}

		protected override void ApplyHooks() {
			IL.RoR2.CharacterBody.OnSkillCooldown += this.CharacterBody_OnSkillCooldown;
		}

		private void CharacterBody_OnSkillCooldown(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_BARRIER_BASE)))
				throw new ILSeekException(il, "base barrier", nameof(CharacterBody.OnSkillCooldown));

			Instruction baseBarrierInstr = cursor.Prev;

			cursor.Index = 0;
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_BARRIER_STACK)))
				throw new ILSeekException(il, "stack barrier", nameof(CharacterBody.OnSkillCooldown));

			cursor.EmitDelegate(this.NullifyVanillaStackBarrierFactor);

			cursor.Goto(baseBarrierInstr, MoveType.After);
			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyBaseBarrierFactor);
		}

		private float ModifyBaseBarrierFactor(float baseBarrierFactor, CharacterBody body) {
			if(!this.Enabled)
				return baseBarrierFactor;

			int itemCount = body.inventory?.GetItemCountEffective(DLC3Content.Items.BarrierOnCooldown) ?? 0;

			if(itemCount <= 0)
				return baseBarrierFactor;

			float initial = this.barrierBase.Value * 0.01f;
			float perStack = this.barrierStack.Value * 0.01f;

			return this.barrierStackType.Value switch {
				ModSettings.EclipseLiteBarrierStacking.Linear
					=> ItemStacking.Linear(initial, perStack, itemCount),

				ModSettings.EclipseLiteBarrierStacking.Hyperbolic
					=> ItemStacking.Hyperbolic(initial, perStack, itemCount),

				_ => baseBarrierFactor,
			};
		}

		private float NullifyVanillaStackBarrierFactor(float stackBarrierFactor) {
			if(!this.Enabled)
				return stackBarrierFactor;

			return 0f;
		}
	}
}
