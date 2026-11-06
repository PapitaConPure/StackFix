using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API;
using RoR2;
using RoR2.Items;
using StackFix.Exceptions;
using StackFix.Utils;
using System;
using System.Reflection;

namespace StackFix.Items {
	internal sealed class NetworkedSuffering: IItemFix, IDisposable {
		private const int VANILLA_VIRUS_SPREAD_COUNT_BASE = 4;
		private const int VANILLA_VIRUS_SPREAD_COUNT_STACK = 2;
		private const float VANILLA_VIRUS_DAMAGE_BASE_PERCENT = 45f;

		private ILHook currentMaxHook;

		public bool Enabled => Plugin.Enabled.Value && Plugin.NetworkedSufferingFixEnabled.Value;

		public void ApplyFix() {
			this.InstallCurrentMaxHook();

			IL.RoR2.SharedSufferingManager.SendToDamagePool += this.SharedSufferingManager_SendToDamagePool;

			Plugin.Enabled.SettingChanged                       += this.NetworkedSuffering_SettingChanged;
			Plugin.NetworkedSufferingFixEnabled.SettingChanged  += this.NetworkedSuffering_SettingChanged;
			Plugin.NetworkedSufferingSpreadBase.SettingChanged  += this.NetworkedSuffering_SettingChanged;
			Plugin.NetworkedSufferingSpreadStack.SettingChanged += this.NetworkedSuffering_SettingChanged;
			Plugin.NetworkedSufferingDamageBase.SettingChanged  += this.NetworkedSuffering_SettingChanged;
			Plugin.NetworkedSufferingDamageStack.SettingChanged += this.NetworkedSuffering_SettingChanged;

			this.RegisterItemText();
		}

		private void RegisterItemText() {
			bool enabled           = this.Enabled;
			int maxVirusCountBase  = enabled ? Plugin.NetworkedSufferingSpreadBase.Value : VANILLA_VIRUS_SPREAD_COUNT_BASE;
			int maxVirusCountStack = enabled ? Plugin.NetworkedSufferingSpreadStack.Value : VANILLA_VIRUS_SPREAD_COUNT_STACK;
			float damageBase       = enabled ? Plugin.NetworkedSufferingDamageBase.Value : VANILLA_VIRUS_DAMAGE_BASE_PERCENT;
			float damageStack      = enabled ? Plugin.NetworkedSufferingDamageStack.Value : 0f;

			LanguageAPI.AddOverlay("ITEM_SHAREDSUFFERING_DESC",
				$"Infect up to {ItD.Utility($"{maxVirusCountBase}{ItD.Stack(maxVirusCountStack)}")} enemies with a computer virus that " +
				$"duplicates {ItD.Damage($"{damageBase}%{ItD.Stack(damageStack, "+{0:0}%")} of all damage taken")} and sends it to a global " +
				$"damage pool. Every 3 seconds, ALL infected enemies suffer {ItD.Damage("100% pooled damage")}.");
		}

		private void NetworkedSuffering_SettingChanged(object sender, EventArgs e) => this.RegisterItemText();

		private void InstallCurrentMaxHook() {
			PropertyInfo property = typeof(SharedSufferingItemBehaviour).GetProperty(
				"currentMax",
				BindingFlags.Instance | BindingFlags.NonPublic);

			MethodInfo getter = (property?.GetGetMethod(nonPublic: true))
				?? throw new MissingMethodException(typeof(SharedSufferingItemBehaviour).FullName, "get_currentMax");

			this.currentMaxHook = new ILHook(getter, this.CurrentMax);
		}

		private void CurrentMax(ILContext il) {
			il.Body.Instructions.Clear();

			ILCursor cursor = new(il);

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.OverwriteCurrentMax);
			cursor.Emit(OpCodes.Ret);
		}

		private int OverwriteCurrentMax(SharedSufferingItemBehaviour self) {
			bool enabled = this.Enabled;
			int initial  = enabled ? Plugin.NetworkedSufferingSpreadBase.Value : VANILLA_VIRUS_SPREAD_COUNT_BASE;
			int perStack = enabled ? Plugin.NetworkedSufferingSpreadStack.Value : VANILLA_VIRUS_SPREAD_COUNT_STACK;

			return ItemStacking.Linear(initial, perStack, self.stack);
		}

		private void SharedSufferingManager_SendToDamagePool(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(
				MoveType.Before,
				x => x.MatchLdarg(0),
				x => x.MatchLdfld<SharedSufferingManager>("_damageSentToPool"),
				x => x.MatchMul()))
				throw new ILSeekException("Failed to match damageSentToPool IL inSharedSufferingManager.SendToDamagePool");

			cursor.RemoveRange(3);

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.Emit(OpCodes.Ldarg_1);

			cursor.EmitDelegate(this.ModifyDamageMultiplier);
			cursor.Emit(OpCodes.Mul);
		}

		private float ModifyDamageMultiplier(SharedSufferingManager manager, DamageReport report) {
			if(!this.Enabled)
				return manager._damageSentToPool;

			CharacterBody victimBody = report.victimBody;

			if(!victimBody)
				return manager._damageSentToPool;

			SharedSufferingBuffController buffController = victimBody.GetComponent<SharedSufferingBuffController>();

			CharacterBody attackerBody = buffController ? buffController.attacker : null;

			int itemCount = attackerBody?.inventory?.GetItemCountEffective(DLC3Content.Items.SharedSuffering) ?? 0;

			if(itemCount <= 0)
				return manager._damageSentToPool;

			float initial = Plugin.NetworkedSufferingDamageBase.Value;
			float perStack = Plugin.NetworkedSufferingDamageStack.Value;
			float totalPercent = ItemStacking.Linear(initial, perStack, itemCount);

			return totalPercent * 0.01f;
		}

		public void Dispose() {
			this.currentMaxHook?.Dispose();
			this.currentMaxHook = null;
		}
	}
}
