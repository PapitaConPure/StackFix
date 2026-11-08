using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using StackFix.Exceptions;
using StackFix.Settings;
using StackFix.Utils;
using System;
using UnityEngine;

namespace StackFix.Items {
	internal class WarpedEcho: IItemFix {
		private const float VANILLA_DAMAGE_REDUCTION_BASE_PERCENT = 10f;
		private const int VANILLA_DAMAGE_SPREAD_BASE = 3;
		private const int VANILLA_DAMAGE_SPREAD_STACK = 1;
		private const float VANILLA_COOLDOWN_BASE = 15f;

		public bool Enabled => Plugin.Enabled.Value && Plugin.WarpedEchoFixEnabled.Value;

		public void ApplyFix() {
			IL.RoR2.HealthComponent.TakeDamageProcess += this.HealthComponent_TakeDamageProcess;
			IL.RoR2.CharacterBody.UpdateDelayedDamage += this.CharacterBody_UpdateDelayedDamage;

			Plugin.Enabled.SettingChanged                            += this.WarpedEcho_SettingChanged;
			Plugin.WarpedEchoFixEnabled.SettingChanged               += this.WarpedEcho_SettingChanged;
			Plugin.WarpedEchoDamageReductionBase.SettingChanged      += this.WarpedEcho_SettingChanged;
			Plugin.WarpedEchoDamageReductionStack.SettingChanged     += this.WarpedEcho_SettingChanged;
			Plugin.WarpedEchoDamageReductionStackType.SettingChanged += this.WarpedEcho_SettingChanged;
			Plugin.WarpedEchoDamageSpreadBase.SettingChanged         += this.WarpedEcho_SettingChanged;
			Plugin.WarpedEchoDamageSpreadStack.SettingChanged        += this.WarpedEcho_SettingChanged;
			Plugin.WarpedEchoCooldownBase.SettingChanged             += this.WarpedEcho_SettingChanged;
			Plugin.WarpedEchoCooldownStack.SettingChanged            += this.WarpedEcho_SettingChanged;
			Plugin.WarpedEchoCooldownStackType.SettingChanged        += this.WarpedEcho_SettingChanged;

			this.RegisterItemText();
		}

		private void RegisterItemText() {
			bool enabled = this.Enabled;
			float damageReductionBase                  = enabled ? Plugin.WarpedEchoDamageReductionBase.Value  : VANILLA_DAMAGE_REDUCTION_BASE_PERCENT;
			float damageReductionStack                 = enabled ? Plugin.WarpedEchoDamageReductionStack.Value : 0f;
			ItemStacking.Type damageReductionStackType = enabled ? (ItemStacking.Type)Plugin.WarpedEchoDamageReductionStackType.Value : ItemStacking.Type.Linear;
			int damageSpreadBase                       = enabled ? Plugin.WarpedEchoDamageSpreadBase.Value  : VANILLA_DAMAGE_SPREAD_BASE;
			int damageSpreadStack                      = enabled ? Plugin.WarpedEchoDamageSpreadStack.Value : VANILLA_DAMAGE_SPREAD_STACK;
			float cooldownBase                         = enabled ? Plugin.WarpedEchoCooldownBase.Value  : VANILLA_COOLDOWN_BASE;
			float cooldownStack                        = enabled ? Plugin.WarpedEchoCooldownStack.Value : 0f;
			ItemStacking.Type cooldownStackType        = enabled ? (ItemStacking.Type)Plugin.WarpedEchoCooldownStackType.Value : ItemStacking.Type.Linear;

			LanguageAPI.AddOverlay("ITEM_DELAYEDDAMAGE_DESC",
				$"The next source of damage is {ItD.Healing("reduced")} by {ItD.Healing($"{damageReductionBase:0.##}%{ItD.Stack(damageReductionStack, damageReductionStackType, "{0:+0.##;-0.##}%")}")} and " +
				$"{ItD.Healing("spread")} into {ItD.Utility($"{damageSpreadBase}{ItD.Stack(damageSpreadStack)} hits")}. All echoed damage is {ItD.Utility("non-lethal")} except for the " +
				$"final hit. Recharges every {ItD.Utility($"{cooldownBase}s{ItD.Stack(cooldownStack, cooldownStackType, "{0:+0.##;-0.##}%")}")}.");
		}

		private void WarpedEcho_SettingChanged(object sender, EventArgs e) => this.RegisterItemText();

		private void HealthComponent_TakeDamageProcess(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.9f)))
				throw new ILSeekException("Failed to match damage reduction multiplier IL in HealthComponent.TakeDamageProcess");

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyDamageFactor);

			cursor.Index = 0;
			if(!cursor.TryGotoNext(MoveType.After,
				instr => instr.MatchCallvirt<Inventory>(nameof(Inventory.GetItemCountEffective)),
				instr => instr.MatchAdd())) {
				throw new ILSeekException("Failed to match damage spread calculation in HealthComponent.TakeDamageProcess");
			}

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyDamageSpread);
		}

		private float ModifyDamageFactor(float vanillaFactor, HealthComponent self) {
			if(!this.Enabled || !self.body)
				return vanillaFactor;

			int itemCount = self.body.inventory?.GetItemCountEffective(DLC2Content.Items.DelayedDamage) ?? 0;

			if(itemCount <= 0)
				return vanillaFactor;

			float initial = Plugin.WarpedEchoDamageReductionBase.Value * 0.01f;
			float perStack = Plugin.WarpedEchoDamageReductionStack.Value * 0.01f;

			float factor = Plugin.WarpedEchoDamageReductionStackType.Value switch {
				ModSettings.WarpedEchoDamageReductionStacking.Linear
					=> 1f - ItemStacking.Linear(initial, perStack, itemCount),

				ModSettings.WarpedEchoDamageReductionStacking.Reciprocal
					=> ItemStacking.Reciprocal(initial, perStack, itemCount),

				ModSettings.WarpedEchoDamageReductionStacking.Exponential
					=> ItemStacking.Reciprocal(1f - initial, 1f - perStack, itemCount),

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

			int initial = Plugin.WarpedEchoDamageSpreadBase.Value;
			int perStack = Plugin.WarpedEchoDamageSpreadStack.Value;

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
				throw new ILSeekException("Failed to match delayedDamageRefreshTime IL in CharacterBody.UpdateDelayedDamage");
		}

		private float ModifyCooldown(float cooldown, CharacterBody body) {
			if(!this.Enabled)
				return cooldown;

			int itemCount = body.inventory?.GetItemCountEffective(DLC2Content.Items.DelayedDamage) ?? 0;

			if(itemCount <= 0)
				return cooldown;

			float baseValue = Plugin.WarpedEchoCooldownBase.Value;
			float perStack = Plugin.WarpedEchoCooldownStack.Value * 0.01f;

			float factor = Plugin.WarpedEchoCooldownStackType.Value switch {
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
