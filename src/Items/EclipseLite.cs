using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using StackFix.Exceptions;
using StackFix.Settings;
using StackFix.Utils;
using System;

namespace StackFix.Items {
	internal sealed class EclipseLite: IItemFix {
		private const float VANILLA_BARRIER_BASE = 0.01f;
		private const float VANILLA_BARRIER_STACK = 0.0025f;

		public bool Enabled => Plugin.Enabled.Value && Plugin.EclipseLiteFixEnabled.Value;

		public bool Applied { get; private set; }

		public void ApplyFix() {
			IL.RoR2.CharacterBody.OnSkillCooldown += this.CharacterBody_OnSkillCooldown;

			Plugin.Enabled.SettingChanged                     += this.EclipseLite_SettingChanged;
			Plugin.EclipseLiteFixEnabled.SettingChanged       += this.EclipseLite_SettingChanged;
			Plugin.EclipseLiteBarrierBase.SettingChanged      += this.EclipseLite_SettingChanged;
			Plugin.EclipseLiteBarrierStack.SettingChanged     += this.EclipseLite_SettingChanged;
			Plugin.EclipseLiteBarrierStackType.SettingChanged += this.EclipseLite_SettingChanged;

			this.RegisterItemText();

			this.Applied = true;
		}

		private void RegisterItemText() {
			bool enabled = this.Enabled;
			float barrierBase                  = enabled ? Plugin.EclipseLiteBarrierBase.Value : VANILLA_BARRIER_BASE * 100f;
			float barrierStack                 = enabled ? Plugin.EclipseLiteBarrierStack.Value : VANILLA_BARRIER_STACK * 100f;
			ItemStacking.Type barrierStackType = enabled ? ItemStacking.FromEnum(Plugin.EclipseLiteBarrierStackType.Value) : ItemStacking.Type.Linear;

			LanguageAPI.AddOverlay("ITEM_BARRIERONCOOLDOWN_DESC", new ItD(
				$"When a skill comes off cooldown, gain a {ItD.Damage("temporary barrier")} for {ItD.Healing($"{barrierBase:0.##}%")}",
				$"{ItD.Stack(barrierStack, barrierStackType, "{0:+0.##;-0.##}%", ItemStacking.Type.Linear)} ",
				$"of your maximum health per second of the skill's base cooldown."));
		}

		private void EclipseLite_SettingChanged(object sender, EventArgs e) => this.RegisterItemText();

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

			float initial = Plugin.EclipseLiteBarrierBase.Value * 0.01f;
			float perStack = Plugin.EclipseLiteBarrierStack.Value * 0.01f;
			ModSettings.EclipseLiteBarrierStacking stackType = Plugin.EclipseLiteBarrierStackType.Value;

			return stackType switch {
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
