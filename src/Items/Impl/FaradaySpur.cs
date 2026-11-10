using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Items;
using StackFix.Compat;
using StackFix.Exceptions;
using StackFix.Settings;
using StackFix.Utils;

namespace StackFix.Items.Impl {
	internal sealed class FaradaySpur: ItemFix {
		private const float VANILLA_MAX_CHARGE_MOVE_SPEED_BASE = 1.6f;
		private const float VANILLA_MAX_CHARGE_JUMP_HEIGHT_BASE = 2f;
		private const float VANILLA_EXPLOSION_DAMAGE_BASE = 4f;
		private const float VANILLA_EXPLOSION_DAMAGE_STACK = 2.8f;
		private const float VANILLA_THRESHOLD_CHARGE_DISTANCE_BASE = 5f;
		private const float VANILLA_MAX_CHARGE_DISTANCE_BASE = 32.3f;
		private const float VANILLA_MAX_CHARGE_DISTANCE_STACK = 7.5f;
		private const float VANILLA_CHARGE_THRESHOLD_BASE = 0.25f;

		private ConfigEntry<bool> fixEnabled;
		private ConfigEntry<float> moveSpeedBase;
		private ConfigEntry<float> moveSpeedStack;
		private ConfigEntry<float> jumpHeightBase;
		private ConfigEntry<float> jumpHeightStack;
		private ConfigEntry<float> damageBase;
		private ConfigEntry<float> damageStack;

		public override bool Enabled => Plugin.Enabled.Value && this.fixEnabled.Value;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled      = this.RegisterSetting(plugin, ModSettings.FaradaySpurFixEnabled);
			this.moveSpeedBase   = this.RegisterSetting(plugin, ModSettings.FaradaySpurMoveSpeedBase);
			this.moveSpeedStack  = this.RegisterSetting(plugin, ModSettings.FaradaySpurMoveSpeedStack);
			this.jumpHeightBase  = this.RegisterSetting(plugin, ModSettings.FaradaySpurJumpHeightBase);
			this.jumpHeightStack = this.RegisterSetting(plugin, ModSettings.FaradaySpurJumpHeightStack);
			this.damageBase      = this.RegisterSetting(plugin, ModSettings.FaradaySpurDamageBase);
			this.damageStack     = this.RegisterSetting(plugin, ModSettings.FaradaySpurDamageStack);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompatManager.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompatManager.AddSliderOption(this.moveSpeedBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompatManager.AddSliderOption(this.moveSpeedStack, new(FormatString: "+{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompatManager.AddSliderOption(this.jumpHeightBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompatManager.AddSliderOption(this.jumpHeightStack, new(FormatString: "+{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompatManager.AddSliderOption(this.damageBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompatManager.AddSliderOption(this.damageStack, new(FormatString: "+{0:0.#}%", Step: 0.5f));
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float moveSpeedBase      = enabled ? this.moveSpeedBase.Value : VANILLA_MAX_CHARGE_MOVE_SPEED_BASE * 100f;
			float moveSpeedStack     = enabled ? this.moveSpeedStack.Value : 0f;
			float jumpHeightBase     = enabled ? this.jumpHeightBase.Value : VANILLA_MAX_CHARGE_JUMP_HEIGHT_BASE * 100f;
			float jumpHeightStack    = enabled ? this.jumpHeightStack.Value : 0f;
			float damageBase         = enabled ? this.damageBase.Value : VANILLA_EXPLOSION_DAMAGE_BASE * 100f;
			float damageStack        = enabled ? this.damageStack.Value : VANILLA_EXPLOSION_DAMAGE_STACK * 100f;
			float thresDistanceBase  = VANILLA_THRESHOLD_CHARGE_DISTANCE_BASE;
			float thresDistanceStack = 0f;
			float maxDistanceBase    = VANILLA_MAX_CHARGE_DISTANCE_BASE;
			float maxDistanceStack   = VANILLA_MAX_CHARGE_DISTANCE_STACK;
			float thresBase          = VANILLA_CHARGE_THRESHOLD_BASE * 100f;
			float thresStack         = 0f;

			LanguageAPI.AddOverlay("ITEM_JUMPDAMAGESTRIKE_DESC", new ItD(
				$"Moving around builds up {ItD.Utility("charge")}, granting up to {ItD.Utility($"+{moveSpeedBase:0}%{ItD.Stack(moveSpeedStack, "+{0:0}%")} movement speed")} ",
				$"and {ItD.Utility($"+{jumpHeightBase:0}%{ItD.Stack(jumpHeightStack, "+{0:0}%")} jump height")} at 100%. At {thresBase:0}%{ItD.Stack(thresStack, "{+0;-0:0}%")} charge ",
				$"or higher, jumping triggers an {ItD.Damage("explosive discharge")} for {ItD.Damage($"+{damageBase:0}%{ItD.Stack(damageStack, "+{0:0}%")} damage")} in a ",
				$"{ItD.Damage($"{thresDistanceBase:0.#}m{ItD.Stack(thresDistanceStack, "+{0:0.#}m")}")} to {ItD.Damage($"{maxDistanceBase:0.#}m{ItD.Stack(maxDistanceStack, "+{0:0.#}m")}")} area."));
		}

		protected override void ApplyHooks() {
			IL.RoR2.CharacterBody.RecalculateStats += this.CharacterBody_RecalculateStats;
			IL.RoR2.Items.JumpDamageStrikeBodyBehavior.DischargeEffects += this.JumpDamageStrikeBodyBehavior_DischargeEffects;
		}

		private void CharacterBody_RecalculateStats(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld<JumpDamageStrikeBodyBehavior>(
				nameof(JumpDamageStrikeBodyBehavior.MoveSpeedVelocityPerCharge)
			)))
				throw new ILSeekException(il, "MoveSpeedVelocityPerCharge", nameof(CharacterBody.RecalculateStats));

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyMoveSpeedPerCharge);

			cursor.Index = 0;
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld<JumpDamageStrikeBodyBehavior>(
				nameof(JumpDamageStrikeBodyBehavior.JumpVelocityPerCharge)
			)))
				throw new ILSeekException(il, "JumpVelocityPerCharge", nameof(CharacterBody.RecalculateStats));

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyJumpHeightPerCharge);
		}

		private float ModifyMoveSpeedPerCharge(float moveSpeedPerCharge, CharacterBody body) {
			if(!this.Enabled)
				return moveSpeedPerCharge;

			int itemCount = body.inventory?.GetItemCountEffective(DLC3Content.Items.JumpDamageStrike) ?? 0;

			if(itemCount <= 0)
				return moveSpeedPerCharge;

			float baseMaxChargeBuff = this.moveSpeedBase.Value;
			float stackMaxChargeBuff = this.moveSpeedStack.Value;
			float targetMaxChargeBuff = ItemStacking.Linear(baseMaxChargeBuff, stackMaxChargeBuff, itemCount);

			return targetMaxChargeBuff * 0.0001f; //charge is 0-100 too i guess, so divide by 100^2
		}

		private float ModifyJumpHeightPerCharge(float jumpHeightPerCharge, CharacterBody body) {
			if(!this.Enabled)
				return jumpHeightPerCharge;

			int itemCount = body.inventory?.GetItemCountEffective(DLC3Content.Items.JumpDamageStrike) ?? 0;

			if(itemCount <= 0)
				return jumpHeightPerCharge;

			float baseMaxChargeBuff = this.jumpHeightBase.Value;
			float stackMaxChargeBuff = this.jumpHeightStack.Value;
			float targetMaxChargeBuff = ItemStacking.Linear(baseMaxChargeBuff, stackMaxChargeBuff, itemCount);

			return targetMaxChargeBuff * 0.0001f; //charge is 0-100 too i guess, so divide by 100^2
		}

		private void JumpDamageStrikeBodyBehavior_DischargeEffects(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_EXPLOSION_DAMAGE_BASE)))
				throw new ILSeekException(il, "base damage", nameof(JumpDamageStrikeBodyBehavior.DischargeEffects));

			cursor.EmitDelegate(this.ModifyBaseDamage);

			cursor.Index = 0;
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_EXPLOSION_DAMAGE_STACK)))
				throw new ILSeekException(il, "stack damage", nameof(JumpDamageStrikeBodyBehavior.DischargeEffects));

			cursor.EmitDelegate(this.ModifyStackDamage);
		}

		private float ModifyBaseDamage(float baseDamage) {
			if(!this.Enabled)
				return baseDamage;

			return this.damageBase.Value * 0.01f;
		}

		private float ModifyStackDamage(float stackDamage) {
			if(!this.Enabled)
				return stackDamage;

			return this.damageStack.Value * 0.01f;
		}
	}
}
