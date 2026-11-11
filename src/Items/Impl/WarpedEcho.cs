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
using System;
using UnityEngine;

namespace StackFix.Items.Impl {
	internal sealed class WarpedEcho: ItemFix {
		private const float VANILLA_DAMAGE_REDUCTION_BASE = 0.1f;
		private const int VANILLA_DAMAGE_SPREAD_BASE = 3;
		private const int VANILLA_DAMAGE_SPREAD_STACK = 1;
		private const float VANILLA_COOLDOWN_BASE = 15f;

		private ConfigEntry<float> damageReductionBase;
		private ConfigEntry<float> damageReductionStack;
		private ConfigEntry<ModSettings.WarpedEchoDamageReductionStacking> damageReductionStackType;
		private ConfigEntry<int> damageSpreadBase;
		private ConfigEntry<int> damageSpreadStack;
		private ConfigEntry<float> cooldownBase;
		private ConfigEntry<float> cooldownStack;
		private ConfigEntry<ModSettings.WarpedEchoCooldownStacking> cooldownStackType;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled               = this.RegisterSetting(plugin, ModSettings.WarpedEchoFixEnabled);
			this.damageReductionBase      = this.RegisterSetting(plugin, ModSettings.WarpedEchoDamageReductionBase);
			this.damageReductionStack     = this.RegisterSetting(plugin, ModSettings.WarpedEchoDamageReductionStack);
			this.damageReductionStackType = this.RegisterSetting(plugin, ModSettings.WarpedEchoDamageReductionStackType);
			this.damageSpreadBase         = this.RegisterSetting(plugin, ModSettings.WarpedEchoDamageSpreadBase);
			this.damageSpreadStack        = this.RegisterSetting(plugin, ModSettings.WarpedEchoDamageSpreadStack);
			this.cooldownBase             = this.RegisterSetting(plugin, ModSettings.WarpedEchoCooldownBase);
			this.cooldownStack            = this.RegisterSetting(plugin, ModSettings.WarpedEchoCooldownStack);
			this.cooldownStackType        = this.RegisterSetting(plugin, ModSettings.WarpedEchoCooldownStackType);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompatManager.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompatManager.AddSliderOption(this.damageReductionBase, new(Suffix: "%"));
			RiskOfOptionsCompatManager.AddSliderOption(this.damageReductionStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompatManager.AddChoiceOption(this.damageReductionStackType);
			RiskOfOptionsCompatManager.AddSliderOption(this.damageSpreadBase);
			RiskOfOptionsCompatManager.AddSliderOption(this.damageSpreadStack, new(Prefix: "+"));
			RiskOfOptionsCompatManager.AddSliderOption(this.cooldownBase, new(FormatString: "{0:0.#}s", Step: 0.5f));
			RiskOfOptionsCompatManager.AddSliderOption(this.cooldownStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompatManager.AddChoiceOption(this.cooldownStackType);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			float damageReductionBase                  = enabled ? this.damageReductionBase.Value  : VANILLA_DAMAGE_REDUCTION_BASE * 100f;
			float damageReductionStack                 = enabled ? this.damageReductionStack.Value : 0f;
			ItemStacking.Type damageReductionStackType = enabled ? ItemStacking.FromEnum(this.damageReductionStackType.Value) : ItemStacking.Type.Linear;
			int damageSpreadBase                       = enabled ? this.damageSpreadBase.Value  : VANILLA_DAMAGE_SPREAD_BASE;
			int damageSpreadStack                      = enabled ? this.damageSpreadStack.Value : VANILLA_DAMAGE_SPREAD_STACK;
			float cooldownBase                         = enabled ? this.cooldownBase.Value  : VANILLA_COOLDOWN_BASE;
			float cooldownStack                        = enabled ? this.cooldownStack.Value : 0f;
			ItemStacking.Type cooldownStackType        = enabled ? ItemStacking.FromEnum(this.cooldownStackType.Value) : ItemStacking.Type.Linear;

			LanguageAPI.AddOverlay("ITEM_DELAYEDDAMAGE_DESC",
				$"The next source of damage is {ItD.Healing("reduced")} by {ItD.Healing($"{damageReductionBase:0.##}%")}" +
				$"{ItD.Stack(damageReductionStack, damageReductionStackType, "+{0:0.##}%")} and {ItD.Healing("spread")} " +
				$"into {ItD.Utility($"{damageSpreadBase}{ItD.Stack(damageSpreadStack)} hits")}. All echoed damage is {ItD.Utility("non-lethal")} except for " +
				$"the final hit. Recharges every {ItD.Utility($"{cooldownBase:0.#}s")}{ItD.Stack(cooldownStack, cooldownStackType, "{0:-0.##;-0.##}%")}.");
		}

		protected override void ApplyHooks() {
			IL.RoR2.HealthComponent.TakeDamageProcess += this.HealthComponent_TakeDamageProcess;
			IL.RoR2.CharacterBody.UpdateDelayedDamage += this.CharacterBody_UpdateDelayedDamage;
		}

		private void HealthComponent_TakeDamageProcess(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchLdsfld(typeof(DLC2Content.Items), nameof(DLC2Content.Items.DelayedDamage)),
				instr => instr.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCountEffective))))
				throw new ILSeekException(il, "delayed damage item count", nameof(HealthComponent.TakeDamageProcess));

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(1f - VANILLA_DAMAGE_REDUCTION_BASE)))
				throw new ILSeekException(il, "damage reduction multiplier", nameof(HealthComponent.TakeDamageProcess));

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyDamageFactor);

			cursor.Index = 0;
			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCountEffective)),
				instr => instr.MatchAdd()))
				throw new ILSeekException(il, "damage spread calculation", nameof(HealthComponent.TakeDamageProcess));

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyDamageSpread);
		}

		private float ModifyDamageFactor(float vanillaFactor, HealthComponent self) {
			if(!this.Enabled || !self.body)
				return vanillaFactor;

			int itemCount = self.body.inventory?.GetItemCountEffective(DLC2Content.Items.DelayedDamage) ?? 0;

			if(itemCount <= 0)
				return vanillaFactor;

			float initial = this.damageReductionBase.Value * 0.01f;
			float perStack = this.damageReductionStack.Value * 0.01f;

			float factor = this.damageReductionStackType.Value switch {
				ModSettings.WarpedEchoDamageReductionStacking.Linear
					=> 1f - ItemStacking.Linear(initial, perStack, itemCount),

				ModSettings.WarpedEchoDamageReductionStacking.Hyperbolic
					=> 1f - ItemStacking.Hyperbolic(initial, perStack, itemCount),

				ModSettings.WarpedEchoDamageReductionStacking.Exponential
					=> ItemStacking.Exponential(1f - initial, 1f - perStack, itemCount),

				_ => initial,
			};

			return Mathf.Clamp01(factor);
		}

		private int ModifyDamageSpread(int vanillaSpread, HealthComponent self) {
			if(!this.Enabled || !self.body)
				return vanillaSpread;

			int itemCount = self.body.inventory?.GetItemCountEffective(DLC2Content.Items.DelayedDamage) ?? 0;

			if(itemCount <= 0)
				return vanillaSpread;

			int initial = this.damageSpreadBase.Value;
			int perStack = this.damageSpreadStack.Value;

			return ItemStacking.Linear(initial, perStack, itemCount);
		}

		private void CharacterBody_UpdateDelayedDamage(ILContext il) {
			ILCursor cursor = new(il);

			bool found = false;
			while(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<CharacterBody>("delayedDamageRefreshTime"))) {
				cursor.Emit(OpCodes.Ldarg_0);
				cursor.EmitDelegate(this.ModifyCooldown);
				found = true;
			}

			if(!found)
				throw new ILSeekException(il, "delayedDamageRefreshTime", nameof(CharacterBody.UpdateDelayedDamage));
		}

		private float ModifyCooldown(float cooldown, CharacterBody body) {
			if(!this.Enabled)
				return cooldown;

			int itemCount = body.inventory?.GetItemCountEffective(DLC2Content.Items.DelayedDamage) ?? 0;

			if(itemCount <= 0)
				return cooldown;

			float baseValue = this.cooldownBase.Value;
			float perStack = this.cooldownStack.Value * 0.01f;

			float factor = this.cooldownStackType.Value switch {
				ModSettings.WarpedEchoCooldownStacking.Linear
					=> ItemStacking.Linear(1f, -perStack, itemCount),

				ModSettings.WarpedEchoCooldownStacking.Reciprocal
					=> ItemStacking.Reciprocal(0f, perStack, itemCount),

				ModSettings.WarpedEchoCooldownStacking.Exponential
					=> ItemStacking.Exponential(1f - perStack, itemCount - 1),

				_ => 1f,
			};

			return MathF.Max(0.1f, baseValue * factor);
		}
	}
}
