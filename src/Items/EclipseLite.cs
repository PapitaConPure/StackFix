
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using StackFix.Exceptions;
using StackFix.Utils;

namespace StackFix.Items {
	internal sealed class EclipseLite: IItemFix {
		private const float VANILLA_BARRIER_BASE_PERCENT = 1f;
		private const float VANILLA_BARRIER_STACK_PERCENT = 0.25f;

		public bool Enabled => Plugin.Enabled.Value && Plugin.EclipseLiteFixEnabled.Value;

		public void ApplyFix() {
			IL.RoR2.CharacterBody.OnSkillCooldown += this.CharacterBody_OnSkillCooldown;

			Plugin.Enabled.SettingChanged                     += this.EclipseLite_SettingChanged;
			Plugin.EclipseLiteFixEnabled.SettingChanged       += this.EclipseLite_SettingChanged;
			Plugin.EclipseLiteBarrierBase.SettingChanged      += this.EclipseLite_SettingChanged;
			Plugin.EclipseLiteBarrierStack.SettingChanged     += this.EclipseLite_SettingChanged;
			Plugin.EclipseLiteBarrierStackType.SettingChanged += this.EclipseLite_SettingChanged;

			this.RegisterItemText();
		}

		private void RegisterItemText() {
			bool enabled                      = this.Enabled;
			float barrierBase                 = enabled ? Plugin.EclipseLiteBarrierBase.Value : VANILLA_BARRIER_BASE_PERCENT;
			float barrierStack                = enabled ? Plugin.EclipseLiteBarrierStack.Value : VANILLA_BARRIER_STACK_PERCENT;
			ItemStacking.Type barrierStackType = enabled ? Plugin.EclipseLiteBarrierStackType.Value : ItemStacking.Type.Linear;

			LanguageAPI.AddOverlay("ITEM_BARRIERONCOOLDOWN_DESC",
				$"When a skill comes off cooldown, gain a {ItD.Damage("temporary barrier")} for {ItD.Healing($"{barrierBase:0.##}%{ItD.Stack(barrierStack, barrierStackType, "+{0:0.##}%")}")} " +
				$"of your maximum health per second of the skill's base cooldown.");
		}

		private void EclipseLite_SettingChanged(object sender, System.EventArgs e) => this.RegisterItemText();

		private void CharacterBody_OnSkillCooldown(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.01f)))
				throw new ILSeekException("Failed to match base barrier IL in CharacterBody.OnSkillCooldown");

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyBaseBarrierFactor);

			cursor.Index = 0;

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.0025f)))
				throw new ILSeekException("Failed to match stack barrier IL in CharacterBody.OnSkillCooldown");

			cursor.EmitDelegate(this.NullifyVanillaStackBarrierFactor);
		}

		private float ModifyBaseBarrierFactor(float baseBarrierFactor, CharacterBody body) {
			if(!this.Enabled)
				return baseBarrierFactor;

			int itemCount = body.inventory?.GetItemCountEffective(DLC3Content.Items.BarrierOnCooldown) ?? 0;

			if(itemCount <= 0)
				return baseBarrierFactor;

			float initial = Plugin.EclipseLiteBarrierBase.Value;
			float perStack = Plugin.EclipseLiteBarrierStack.Value;
			ItemStacking.Type stackType = Plugin.EclipseLiteBarrierStackType.Value;

			float totalPercent = stackType switch {
				ItemStacking.Type.Linear => ItemStacking.Linear(initial, perStack, itemCount),
				ItemStacking.Type.Hyperbolic => ItemStacking.Hyperbolic(initial * 0.01f, perStack * 0.01f, itemCount) * 100f,
				ItemStacking.Type.Reciprocal => ItemStacking.Reciprocal(initial, perStack * 0.01f, itemCount),
				ItemStacking.Type.Exponential => ItemStacking.Exponential(initial, 1f + perStack * 0.01f, itemCount),
				_ => baseBarrierFactor * 100f,
			};

			return totalPercent * 0.01f;
		}

		private float NullifyVanillaStackBarrierFactor(float stackBarrierFactor) {
			if(!this.Enabled)
				return stackBarrierFactor;

			return 0f;
		}
	}
}
