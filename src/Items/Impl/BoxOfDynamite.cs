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
using System;

namespace StackFix.Items.Impl {
	internal sealed class BoxOfDynamite: ItemFix {
		private const float VANILLA_DAMAGE_BASE = 2.4f;
		private const float VANILLA_DAMAGE_STACK = 0.85f;
		private const float VANILLA_DAMAGE_BASE_PERCENT = VANILLA_DAMAGE_BASE * 100f;
		private const float VANILLA_DAMAGE_STACK_PERCENT = VANILLA_DAMAGE_STACK * 100f;
		private const float VANILLA_COOLDOWN_BASE = 10f;

		private ConfigEntry<float> damageBase;
		private ConfigEntry<float> damageStack;
		private ConfigEntry<float> cooldownBase;
		private ConfigEntry<float> cooldownStack;
		private ConfigEntry<ModSettings.BoxOfDynamiteCooldownStacking> cooldownStackType;
		private ConfigEntry<float> cooldownCap;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled        = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteFixEnabled);
			this.damageBase        = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteDamageBase);
			this.damageStack       = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteDamageStack);
			this.cooldownBase      = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteCooldownBase);
			this.cooldownStack     = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteCooldownStack);
			this.cooldownStackType = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteCooldownStackType);
			this.cooldownCap       = this.RegisterSetting(plugin, ModSettings.BoxOfDynamiteCooldownCap);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompat.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompat.AddSliderOption(this.damageBase, new(FormatString: "{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompat.AddSliderOption(this.damageStack, new(FormatString: "+{0:0.#}%", Step: 0.5f));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownBase, new(FormatString: "{0:0.#}s", Step: 0.1f));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompat.AddChoiceOption(this.cooldownStackType);
			RiskOfOptionsCompat.AddSliderOption(this.cooldownCap, new(Suffix: "s"));
		}

		public override void RegisterLookingGlassStats() {
			float damageBase() => this.Enabled ? this.damageBase.Value * 0.01f : VANILLA_DAMAGE_BASE;
			float damageStack() => this.Enabled ? this.damageStack.Value * 0.01f : VANILLA_DAMAGE_STACK;

			float cooldownBase() => this.Enabled ? this.cooldownBase.Value : VANILLA_COOLDOWN_BASE;
			float cooldownCap() => this.Enabled ? this.cooldownCap.Value : 0f;
			float cooldownStack() => this.Enabled ? this.cooldownStack.Value * 0.01f : 0f;
			ModSettings.BoxOfDynamiteCooldownStacking cooldownStackType()
				=> this.Enabled ? this.cooldownStackType.Value : ModSettings.BoxOfDynamiteCooldownStacking.Linear;

			LookingGlassCompat.AddItem(DLC3Content.Items.DronesDropDynamite, [
				new("Damage",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.Percentage,
					itemCount => ItemStacking.Linear(damageBase(), damageStack(), itemCount)),
				new("Cooldown",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Seconds,
					itemCount => MathF.Max(cooldownCap(), cooldownBase() * cooldownStackType() switch {
						ModSettings.BoxOfDynamiteCooldownStacking.Linear
							=> ItemStacking.Linear(1f, -cooldownStack(), itemCount),

						ModSettings.BoxOfDynamiteCooldownStacking.Reciprocal
							=> ItemStacking.Reciprocal(0f, cooldownStack(), itemCount),

						ModSettings.BoxOfDynamiteCooldownStacking.Exponential
							=> ItemStacking.Exponential(1f - cooldownStack(), itemCount - 1),

						_ => 1f,
					})),
			]);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float damageBase                    = enabled ? this.damageBase.Value : VANILLA_DAMAGE_BASE_PERCENT;
			float damageStack                   = enabled ? this.damageStack.Value : VANILLA_DAMAGE_STACK_PERCENT;
			float cooldownBase                  = enabled ? this.cooldownBase.Value : VANILLA_COOLDOWN_BASE;
			float cooldownStack                 = enabled ? this.cooldownStack.Value : 0f;
			ItemStacking.Type cooldownStackType = enabled ? ItemStacking.FromEnum(this.cooldownStackType.Value) : ItemStacking.Type.Linear;

			string description =
				$"Gain {ItD.Damage("Lt.Droneboy")}. While in combat, your drones drop sticks of dynamite that detonate for " +
				$"{ItD.Damage($"{damageBase}% damage{ItD.Stack(damageStack, "+{0:0.#}%")}")}, stunning enemies. " +
				$"Recharges after {ItD.Utility(cooldownBase)} seconds{ItD.Stack(cooldownStack, cooldownStackType, "{0:-0.##;-0.##}%")}.";

			LanguageAPI.AddOverlay("ITEM_DRONESDROPDYNAMITE_DESC", description);

			if(ItemQualitiesCompat.Installed)
				ItemQualitiesCompat.ApplyQualityDescriptions("DRONESDROPDYNAMITE", description);
		}

		protected override void ApplyHooks() {
			IL.RoR2.Items.DroneDynamiteBehaviour.FixedUpdate += this.DroneDynamiteBehaviour_FixedUpdate;
		}

		private void DroneDynamiteBehaviour_FixedUpdate(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchCallOrCallvirt<DroneDynamiteBehaviour>(nameof(DroneDynamiteBehaviour.IsEnemyNearby))))
				throw new ILSeekException(il, "IsEnemyNearby call", nameof(DroneDynamiteBehaviour.FixedUpdate));

			Instruction isEnemyNearbyCallInstr = cursor.Prev;

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_DAMAGE_BASE)))
				throw new ILSeekException(il, "base damage", nameof(DroneDynamiteBehaviour.FixedUpdate));

			Instruction baseDamageInstr = cursor.Prev;

			cursor.Goto(isEnemyNearbyCallInstr, MoveType.After);
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_DAMAGE_STACK)))
				throw new ILSeekException(il, "stack damage", nameof(DroneDynamiteBehaviour.FixedUpdate));

			Instruction stackDamageInstr = cursor.Prev;

			cursor.Goto(isEnemyNearbyCallInstr, MoveType.After);
			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(VANILLA_COOLDOWN_BASE)))
				throw new ILSeekException(il, "cooldown", nameof(DroneDynamiteBehaviour.FixedUpdate));

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyCooldown);

			cursor.Goto(baseDamageInstr, MoveType.After);
			cursor.EmitDelegate(this.ModifyBaseDamage);

			cursor.Goto(stackDamageInstr, MoveType.After);
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

		private float ModifyCooldown(float cooldown, DroneDynamiteBehaviour behavior) {
			if(!this.Enabled)
				return cooldown;

			int itemCount = behavior.stack;

			if(itemCount <= 0)
				return cooldown;

			float baseValue = this.cooldownBase.Value;
			float perStack = this.cooldownStack.Value * 0.01f;
			float minValue = this.cooldownCap.Value * 0.01f;

			float factor = this.cooldownStackType.Value switch {
				ModSettings.BoxOfDynamiteCooldownStacking.Linear
					=> ItemStacking.Linear(1f, -perStack, itemCount),

				ModSettings.BoxOfDynamiteCooldownStacking.Reciprocal
					=> ItemStacking.Reciprocal(0f, perStack, itemCount),

				ModSettings.BoxOfDynamiteCooldownStacking.Exponential
					=> ItemStacking.Exponential(1f - perStack, itemCount - 1),

				_ => 1f,
			};

			return MathF.Max(minValue, baseValue * factor);
		}
	}
}
