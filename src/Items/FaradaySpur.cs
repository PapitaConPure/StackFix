using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Items;
using StackFix.Exceptions;
using StackFix.Utils;

namespace StackFix.Items {
	internal sealed class FaradaySpur: IItemFix {
		private const float VANILLA_MAX_CHARGE_MOVE_SPEED_BASE_PERCENT = 160f;
		private const float VANILLA_MAX_CHARGE_JUMP_HEIGHT_BASE_PERCENT = 200f;
		private const float VANILLA_EXPLOSION_DAMAGE_BASE_PERCENT = 400f;
		private const float VANILLA_EXPLOSION_DAMAGE_STACK_PERCENT = 280f;
		private const float VANILLA_THRESHOLD_CHARGE_DISTANCE_BASE = 5f;
		private const float VANILLA_MAX_CHARGE_DISTANCE_BASE = 32.3f;
		private const float VANILLA_MAX_CHARGE_DISTANCE_STACK = 7.5f;
		private const float VANILLA_CHARGE_THRESHOLD_BASE_PERCENT = 25f;

		public bool Enabled => Plugin.Enabled.Value && Plugin.FaradaySpurFixEnabled.Value;

		public void ApplyFix() {
			IL.RoR2.CharacterBody.RecalculateStats += this.CharacterBody_RecalculateStats;
			IL.RoR2.Items.JumpDamageStrikeBodyBehavior.DischargeEffects += this.JumpDamageStrikeBodyBehavior_DischargeEffects;

			Plugin.Enabled.SettingChanged                    += this.FaradaySpur_SettingChanged;
			Plugin.FaradaySpurFixEnabled.SettingChanged      += this.FaradaySpur_SettingChanged;
			Plugin.FaradaySpurMoveSpeedBase.SettingChanged   += this.FaradaySpur_SettingChanged;
			Plugin.FaradaySpurMoveSpeedStack.SettingChanged  += this.FaradaySpur_SettingChanged;
			Plugin.FaradaySpurJumpHeightBase.SettingChanged  += this.FaradaySpur_SettingChanged;
			Plugin.FaradaySpurJumpHeightStack.SettingChanged += this.FaradaySpur_SettingChanged;
			Plugin.FaradaySpurDamageBase.SettingChanged      += this.FaradaySpur_SettingChanged;
			Plugin.FaradaySpurDamageStack.SettingChanged     += this.FaradaySpur_SettingChanged;

			this.RegisterItemText();
		}

		private void RegisterItemText() {
			bool enabled             = this.Enabled;
			float moveSpeedBase      = enabled ? Plugin.FaradaySpurMoveSpeedBase.Value : VANILLA_MAX_CHARGE_MOVE_SPEED_BASE_PERCENT;
			float moveSpeedStack     = enabled ? Plugin.FaradaySpurMoveSpeedStack.Value : 0f;
			float jumpHeightBase     = enabled ? Plugin.FaradaySpurJumpHeightBase.Value : VANILLA_MAX_CHARGE_JUMP_HEIGHT_BASE_PERCENT;
			float jumpHeightStack    = enabled ? Plugin.FaradaySpurJumpHeightStack.Value : 0f;
			float damageBase         = enabled ? Plugin.FaradaySpurDamageBase.Value : VANILLA_EXPLOSION_DAMAGE_BASE_PERCENT;
			float damageStack        = enabled ? Plugin.FaradaySpurDamageStack.Value : VANILLA_EXPLOSION_DAMAGE_STACK_PERCENT;
			float thresDistanceBase  = VANILLA_THRESHOLD_CHARGE_DISTANCE_BASE;
			float thresDistanceStack = 0f;
			float maxDistanceBase    = VANILLA_MAX_CHARGE_DISTANCE_BASE;
			float maxDistanceStack   = VANILLA_MAX_CHARGE_DISTANCE_STACK;
			float thresBase          = VANILLA_CHARGE_THRESHOLD_BASE_PERCENT;
			float thresStack         = 0f;

			LanguageAPI.AddOverlay("ITEM_JUMPDAMAGESTRIKE_DESC",
				$"Moving around builds up {ItD.Utility("charge")}, granting up to {ItD.Utility($"+{moveSpeedBase:0}%{ItD.Stack(moveSpeedStack, "+{0:0}%")} movement speed")} " +
				$"and {ItD.Utility($"+{jumpHeightBase:0}%{ItD.Stack(jumpHeightStack, "+{0:0}%")} jump height")} at 100%. At {thresBase:0}%{ItD.Stack(thresStack, "{+0;-0:0}%")} charge " +
				$"or higher, jumping triggers an {ItD.Damage("explosive discharge")} for {ItD.Damage($"+{damageBase:0}%{ItD.Stack(damageStack, "+{0:0}%")} damage")} in a " +
				$"{ItD.Damage($"{thresDistanceBase:0.#}m{ItD.Stack(thresDistanceStack, "+{0:0.#}m")}")} to {ItD.Damage($"{maxDistanceBase:0.#}m{ItD.Stack(maxDistanceStack, "+{0:0.#}m")}")} area.");
		}

		private void FaradaySpur_SettingChanged(object sender, System.EventArgs e) => this.RegisterItemText();

		private void CharacterBody_RecalculateStats(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld<JumpDamageStrikeBodyBehavior>(
				nameof(JumpDamageStrikeBodyBehavior.MoveSpeedVelocityPerCharge)
			)))
				throw new ILSeekException("Failed to match MoveSpeedVelocityPerCharge IL in CharacterBody.RecalculateStats");

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyMoveSpeedPerCharge);

			cursor.Index = 0;
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld<JumpDamageStrikeBodyBehavior>(
				nameof(JumpDamageStrikeBodyBehavior.JumpVelocityPerCharge)
			)))
				throw new ILSeekException("Failed to match JumpVelocityPerCharge IL in CharacterBody.RecalculateStats");

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyJumpHeightPerCharge);
		}

		private float ModifyMoveSpeedPerCharge(float moveSpeedPerCharge, CharacterBody body) {
			if(!this.Enabled)
				return moveSpeedPerCharge;

			int itemCount = body.inventory?.GetItemCountEffective(DLC3Content.Items.JumpDamageStrike) ?? 0;

			if(itemCount <= 0)
				return moveSpeedPerCharge;

			float baseMaxChargeBuff = Plugin.FaradaySpurMoveSpeedBase.Value;
			float stackMaxChargeBuff = Plugin.FaradaySpurMoveSpeedStack.Value;
			float targetMaxChargeBuff = ItemStacking.Linear(baseMaxChargeBuff, stackMaxChargeBuff, itemCount);

			return targetMaxChargeBuff * 0.0001f; //charge is 0-100 too i guess, so divide by 100^2
		}

		private float ModifyJumpHeightPerCharge(float jumpHeightPerCharge, CharacterBody body) {
			if(!this.Enabled)
				return jumpHeightPerCharge;

			int itemCount = body.inventory?.GetItemCountEffective(DLC3Content.Items.JumpDamageStrike) ?? 0;

			if(itemCount <= 0)
				return jumpHeightPerCharge;

			float baseMaxChargeBuff = Plugin.FaradaySpurJumpHeightBase.Value;
			float stackMaxChargeBuff = Plugin.FaradaySpurJumpHeightStack.Value;
			float targetMaxChargeBuff = ItemStacking.Linear(baseMaxChargeBuff, stackMaxChargeBuff, itemCount);

			return targetMaxChargeBuff * 0.0001f; //charge is 0-100 too i guess, so divide by 100^2
		}

		private void JumpDamageStrikeBodyBehavior_DischargeEffects(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(4f)))
				throw new ILSeekException("Failed to match base damage IL in JumpDamageStrikeBodyBehavior.DischargeEffects");

			cursor.EmitDelegate(this.ModifyBaseDamage);

			cursor.Index = 0;
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(2.8f)))
				throw new ILSeekException("Failed to match stack damage IL in JumpDamageStrikeBodyBehavior.DischargeEffects");

			cursor.EmitDelegate(this.ModifyStackDamage);
		}

		private float ModifyBaseDamage(float baseDamage) {
			if(!this.Enabled)
				return baseDamage;

			return Plugin.FaradaySpurDamageBase.Value * 0.01f;
		}

		private float ModifyStackDamage(float stackDamage) {
			if(!this.Enabled)
				return stackDamage;

			return Plugin.FaradaySpurDamageStack.Value * 0.01f;
		}
	}
}
