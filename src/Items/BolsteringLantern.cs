using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using StackFix.Exceptions;
using StackFix.Utils;
using System;

namespace StackFix.Items {
	internal sealed class BolsteringLantern(): IItemFix {
		private const float VANILLA_ATTACK_SPEED_BASE = 0.065f;
		private const float VANILLA_ATTACK_SPEED_STACK = 0.035f;
		private const int VANILLA_CHARACTER_COUNT_MAX_BASE = 3;
		private const int VANILLA_CHARACTER_COUNT_MAX_STACK = 1;

		public bool Enabled => Plugin.Enabled.Value && Plugin.BolsteringLanternFixEnabled.Value;

		public bool Applied { get; private set; }

		public void ApplyFix() {
			IL.RoR2.CharacterBody.RecalculateStats += this.CharacterBody_RecalculateStats;
			IL.RoR2.AttackSpeedPerNearbyCollider.UpdateValues += this.AttackSpeedPerNearbyCollider_UpdateValues;

			Plugin.Enabled.SettingChanged                                 += this.BolsteringLantern_SettingChanged;
			Plugin.BolsteringLanternFixEnabled.SettingChanged             += this.BolsteringLantern_SettingChanged;
			Plugin.BolsteringLanternAttackSpeedBase.SettingChanged        += this.BolsteringLantern_SettingChanged;
			Plugin.BolsteringLanternAttackSpeedStack.SettingChanged       += this.BolsteringLantern_SettingChanged;
			Plugin.BolsteringLanternCharacterCountMaxBase.SettingChanged  += this.BolsteringLantern_SettingChanged;
			Plugin.BolsteringLanternCharacterCountMaxStack.SettingChanged += this.BolsteringLantern_SettingChanged;

			this.RegisterItemText();

			this.Applied = true;
		}

		private void RegisterItemText() {
			bool enabled = this.Enabled;
			float attackSpeedBase  = enabled ? Plugin.BolsteringLanternAttackSpeedBase.Value : VANILLA_ATTACK_SPEED_BASE * 100f;
			float attackSpeedStack = enabled ? Plugin.BolsteringLanternAttackSpeedStack.Value : VANILLA_ATTACK_SPEED_STACK * 100f;
			int charaCountMaxBase  = enabled ? Plugin.BolsteringLanternCharacterCountMaxBase.Value : VANILLA_CHARACTER_COUNT_MAX_BASE;
			int charaCountMaxStack = enabled ? Plugin.BolsteringLanternCharacterCountMaxStack.Value : VANILLA_CHARACTER_COUNT_MAX_STACK;

			LanguageAPI.AddOverlay("ITEM_ATTACKSPEEDPERNEARBYALLYORENEMY_DESC", new ItD(
				$"Increase your {ItD.Damage("attack speed")} by {ItD.Damage($"{attackSpeedBase:0.##}%")}{ItD.Stack(attackSpeedStack, "+{0:0.##}%")} ",
				$"for up to {ItD.Utility($"{charaCountMaxBase}")}{ItD.Stack(charaCountMaxStack)} enemies and allies within {ItD.Utility("20")} meters."));
		}

		private void BolsteringLantern_SettingChanged(object sender, EventArgs e) => this.RegisterItemText();

		private void CharacterBody_RecalculateStats(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdsfld(typeof(DLC2Content.Buffs), nameof(DLC2Content.Buffs.AttackSpeedPerNearbyAllyOrEnemyBuff)),
				instr => instr.MatchCallOrCallvirt<CharacterBody>(nameof(CharacterBody.GetBuffCount))))
				throw new ILSeekException(il, "GetBuffCount(AttackSpeedPerNearbyAllyOrEnemyBuff)", nameof(CharacterBody.RecalculateStats));

			int buffPos = cursor.Index;

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_ATTACK_SPEED_BASE)))
				throw new ILSeekException(il, "initial attack speed", nameof(CharacterBody.RecalculateStats));

			Instruction initialValueInstr = cursor.Prev;

			cursor.Index = buffPos;
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_ATTACK_SPEED_STACK)))
				throw new ILSeekException(il, "attack speed per stack", nameof(CharacterBody.RecalculateStats));

			cursor.EmitDelegate(this.ModifyStackAttackSpeed);

			cursor.Goto(initialValueInstr, MoveType.After);
			cursor.EmitDelegate(this.ModifyBaseAttackSpeed);
		}

		private float ModifyBaseAttackSpeed(float baseAttackSpeed) {
			if(!this.Enabled)
				return baseAttackSpeed;

			return Plugin.BolsteringLanternAttackSpeedBase.Value * 0.01f;
		}

		private float ModifyStackAttackSpeed(float stackAttackSpeed) {
			if(!this.Enabled)
				return stackAttackSpeed;

			return Plugin.BolsteringLanternAttackSpeedStack.Value * 0.01f;
		}

		private void AttackSpeedPerNearbyCollider_UpdateValues(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(
				instr => instr.MatchLdcI4(VANILLA_CHARACTER_COUNT_MAX_BASE),
				instr => instr.MatchLdarg(1),
				instr => instr.MatchAdd()))
				throw new ILSeekException(il, "max character count", nameof(AttackSpeedPerNearbyCollider.UpdateValues));

			cursor.Emit(OpCodes.Ldarg_1);
			cursor.EmitDelegate(this.ModifyCharacterCountMax);
		}

		private int ModifyCharacterCountMax(int characterCountMax, int itemCount) {
			if(!this.Enabled)
				return characterCountMax;

			return Plugin.BolsteringLanternCharacterCountMaxBase.Value + itemCount * Plugin.BolsteringLanternCharacterCountMaxStack.Value;
		}
	}
}
