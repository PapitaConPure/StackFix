using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using StackFix.Exceptions;
using StackFix.Settings;
using StackFix.Utils;

namespace StackFix.Items.Impl {
	internal sealed class BolsteringLantern: ItemFix {
		private const float VANILLA_ATTACK_SPEED_BASE = 0.065f;
		private const float VANILLA_ATTACK_SPEED_STACK = 0.035f;
		private const int VANILLA_CHARACTER_COUNT_MAX_BASE = 3;
		private const int VANILLA_CHARACTER_COUNT_MAX_STACK = 1;

		private ConfigEntry<bool> fixEnabled;
		private ConfigEntry<float> attackSpeedBase;
		private ConfigEntry<float> attackSpeedStack;
		private ConfigEntry<int> characterCountMaxBase;
		private ConfigEntry<int> characterCountMaxStack;

		public override bool Enabled => Plugin.Enabled.Value && this.fixEnabled.Value;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled             = this.RegisterSetting(plugin, ModSettings.BolsteringLanternFixEnabled);
			this.attackSpeedBase        = this.RegisterSetting(plugin, ModSettings.BolsteringLanternAttackSpeedBase);
			this.attackSpeedStack       = this.RegisterSetting(plugin, ModSettings.BolsteringLanternAttackSpeedStack);
			this.characterCountMaxBase  = this.RegisterSetting(plugin, ModSettings.BolsteringLanternCharacterCountMaxBase);
			this.characterCountMaxStack = this.RegisterSetting(plugin, ModSettings.BolsteringLanternCharacterCountMaxStack);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompatManager.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompatManager.AddSliderOption(this.attackSpeedBase, new(Suffix: "%"));
			RiskOfOptionsCompatManager.AddSliderOption(this.attackSpeedStack, new(FormatString: "+{0:0.##}%"));
			RiskOfOptionsCompatManager.AddSliderOption(this.characterCountMaxBase);
			RiskOfOptionsCompatManager.AddSliderOption(this.characterCountMaxStack, new(Prefix: "+"));
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float attackSpeedBase  = enabled ? this.attackSpeedBase.Value : VANILLA_ATTACK_SPEED_BASE * 100f;
			float attackSpeedStack = enabled ? this.attackSpeedStack.Value : VANILLA_ATTACK_SPEED_STACK * 100f;
			int charaCountMaxBase  = enabled ? this.characterCountMaxBase.Value : VANILLA_CHARACTER_COUNT_MAX_BASE;
			int charaCountMaxStack = enabled ? this.characterCountMaxStack.Value : VANILLA_CHARACTER_COUNT_MAX_STACK;

			LanguageAPI.AddOverlay("ITEM_ATTACKSPEEDPERNEARBYALLYORENEMY_DESC", new ItD(
				$"Increase your {ItD.Damage("attack speed")} by {ItD.Damage($"{attackSpeedBase:0.##}%")}{ItD.Stack(attackSpeedStack, "+{0:0.##}%")} ",
				$"for up to {ItD.Utility($"{charaCountMaxBase}")}{ItD.Stack(charaCountMaxStack)} enemies and allies within {ItD.Utility("20")} meters."));
		}

		protected override void ApplyHooks() {
			IL.RoR2.CharacterBody.RecalculateStats += this.CharacterBody_RecalculateStats;
			IL.RoR2.AttackSpeedPerNearbyCollider.UpdateValues += this.AttackSpeedPerNearbyCollider_UpdateValues;
		}

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

			return this.attackSpeedBase.Value * 0.01f;
		}

		private float ModifyStackAttackSpeed(float stackAttackSpeed) {
			if(!this.Enabled)
				return stackAttackSpeed;

			return this.attackSpeedStack.Value * 0.01f;
		}

		private void AttackSpeedPerNearbyCollider_UpdateValues(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(
				instr => instr.MatchLdcI4(VANILLA_CHARACTER_COUNT_MAX_BASE - 1), //Starts at a - 1 and adds x (not x - 1)
				instr => instr.MatchLdarg(1),
				instr => instr.MatchAdd()))
				throw new ILSeekException(il, "max character count", nameof(AttackSpeedPerNearbyCollider.UpdateValues));

			cursor.Emit(OpCodes.Ldarg_1);
			cursor.EmitDelegate(this.ModifyCharacterCountMax);
		}

		private int ModifyCharacterCountMax(int characterCountMax, int itemCount) {
			if(!this.Enabled)
				return characterCountMax;

			return this.characterCountMaxBase.Value + itemCount * this.characterCountMaxStack.Value;
		}
	}
}
