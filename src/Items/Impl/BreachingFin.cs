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
using System.Text.RegularExpressions;

namespace StackFix.Items.Impl {
	internal class BreachingFin: ItemFix {
		private const int VANILLA_AIRBORNE_LAUNCHES_BASE = 2;
		private const int VANILLA_AIRBORNE_LAUNCHES_STACK = 1;
		private const float VANILLA_DAMAGE_BASE = 0.1f;
		private const float VANILLA_DAMAGE_BASE_PERCENT = VANILLA_DAMAGE_BASE * 100f;
		private const float VANILLA_COOLDOWN_BASE = 15f;

		private ConfigEntry<int> airLaunchesBase;
		private ConfigEntry<int> airLaunchesStack;
		private ConfigEntry<float> damageBase;
		private ConfigEntry<float> damageStack;
		private ConfigEntry<float> cooldownBase;
		private ConfigEntry<float> cooldownStack;
		private ConfigEntry<ModSettings.BreachingFinCooldownStacking> cooldownStackType;
		private ConfigEntry<float> cooldownCap;

		public override void RegisterSettings(BaseUnityPlugin plugin) {
			this.fixEnabled        = ModSettings.BreachingFinFixEnabled.RegisterFor(plugin);
			this.airLaunchesBase   = ModSettings.BreachingFinAirLaunchesBase.RegisterFor(plugin);
			this.airLaunchesStack  = ModSettings.BreachingFinAirLaunchesStack.RegisterFor(plugin);
			this.damageBase        = ModSettings.BreachingFinDamageBase.RegisterFor(plugin);
			this.damageStack       = ModSettings.BreachingFinDamageStack.RegisterFor(plugin);
			this.cooldownBase      = ModSettings.BreachingFinCooldownBase.RegisterFor(plugin);
			this.cooldownStack     = ModSettings.BreachingFinCooldownStack.RegisterFor(plugin);
			this.cooldownStackType = ModSettings.BreachingFinCooldownStackType.RegisterFor(plugin);
			this.cooldownCap       = this.RegisterSetting(plugin, ModSettings.BreachingFinCooldownCap);
		}

		public override void RegisterRiskOfOptionsSettings() {
			RiskOfOptionsCompat.AddCheckboxOption(this.fixEnabled);
			RiskOfOptionsCompat.AddSliderOption(this.airLaunchesBase);
			RiskOfOptionsCompat.AddSliderOption(this.airLaunchesStack);
			RiskOfOptionsCompat.AddSliderOption(this.damageBase, new(Suffix: "%"));
			RiskOfOptionsCompat.AddSliderOption(this.damageStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownBase, new(FormatString: "{0:0.#}s", Step: 0.1f));
			RiskOfOptionsCompat.AddSliderOption(this.cooldownStack, new(Prefix: "+", Suffix: "%"));
			RiskOfOptionsCompat.AddChoiceOption(this.cooldownStackType);
			RiskOfOptionsCompat.AddSliderOption(this.cooldownCap, new(Suffix: "s"));
		}

		public override void RegisterLookingGlassStats() {
			int airLaunchesBase() => this.Enabled ? this.airLaunchesBase.Value : VANILLA_AIRBORNE_LAUNCHES_BASE;
			int airLaunchesStack() => this.Enabled ? this.airLaunchesStack.Value : VANILLA_AIRBORNE_LAUNCHES_STACK;

			float damageBase() => this.Enabled ? this.damageBase.Value * 0.01f : VANILLA_DAMAGE_BASE;
			float damageStack() => this.Enabled ? this.damageStack.Value * 0.01f : 0f;

			float cooldownBase() => this.Enabled ? this.cooldownBase.Value : VANILLA_COOLDOWN_BASE;
			float cooldownCap() => this.Enabled ? this.cooldownCap.Value : 0f;
			float cooldownStack() => this.Enabled ? this.cooldownStack.Value * 0.01f : 0f;
			ModSettings.BreachingFinCooldownStacking cooldownStackType()
				=> this.Enabled ? this.cooldownStackType.Value : ModSettings.BreachingFinCooldownStacking.Linear;

			LookingGlassCompat.AddItem(DLC2Content.Items.KnockBackHitEnemies, [
				new("Airborne Launches",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Number,
					itemCount => ItemStacking.Linear(airLaunchesBase(), airLaunchesStack(), itemCount)),
				new("Bonus Damage per Launch",
					LookingGlassCompat.StatType.Damage,
					LookingGlassCompat.StatUnits.Percentage,
					itemCount => ItemStacking.Linear(damageBase(), damageStack(), itemCount)),
				new("Cooldown",
					LookingGlassCompat.StatType.Utility,
					LookingGlassCompat.StatUnits.Seconds,
					itemCount => MathF.Max(cooldownCap(), cooldownBase() * cooldownStackType() switch {
						ModSettings.BreachingFinCooldownStacking.Linear
							=> ItemStacking.Linear(1f, -cooldownStack(), itemCount),

						ModSettings.BreachingFinCooldownStacking.Reciprocal
							=> ItemStacking.Reciprocal(0f, cooldownStack(), itemCount),

						ModSettings.BreachingFinCooldownStacking.Exponential
							=> ItemStacking.Exponential(1f - cooldownStack(), itemCount - 1),

						_ => 1f,
					})),
			]);
		}

		protected override void ApplyDescription() {
			bool enabled = this.Enabled;
			int airLaunchesBase                 = enabled ? this.airLaunchesBase.Value : VANILLA_AIRBORNE_LAUNCHES_BASE;
			int airLaunchesStack                = enabled ? this.airLaunchesStack.Value : VANILLA_AIRBORNE_LAUNCHES_STACK;
			float damageBase                    = enabled ? this.damageBase.Value : VANILLA_DAMAGE_BASE_PERCENT;
			float damageStack                   = enabled ? this.damageStack.Value : 0f;
			float cooldownBase                  = enabled ? this.cooldownBase.Value : VANILLA_DAMAGE_BASE_PERCENT;
			float cooldownStack                 = enabled ? this.cooldownStack.Value : 0f;
			ItemStacking.Type cooldownStackType = enabled ? ItemStacking.FromEnum(this.cooldownStackType.Value) : ItemStacking.Type.Linear;

			LanguageAPI.AddOverlay("ITEM_KNOCKBACKHITENEMIES_DESC",
				$"Grounded enemies hit with any {ItD.Utility("skill")} are {ItD.Utility("launched")} and {ItD.Utility("stunned")}. " +
				$"Enemies hit while airborne are {ItD.Utility("launched")} again up to {ItD.Utility($"{airLaunchesBase} times")}" +
				$"{ItD.Stack(airLaunchesStack)}. Launched enemies take increased damage by {ItD.Damage($"{damageBase}%")}{ItD.Stack(damageStack)} " +
				$"from all sources per launch. Recharges after {ItD.Utility($"{cooldownBase}s")}{ItD.Stack(cooldownStack, cooldownStackType, "{0:-0.##;-0.##}%")}.");

			if(ItemQualitiesCompat.Installed)
				ItemQualitiesCompat.ApplyQualityDescriptions("KNOCKBACKHITENEMIES", [
					new($"{ItD.Utility("2 times")} {ItD.Stack("(+1 per stack)")}",
						"{0}{1}",
						ItD.Utility($"{ItD.V(airLaunchesBase)} times"),
						ItD.Stack(airLaunchesStack)),
					new(new Regex(@"<style=cIsDamage>(?:\d{1,6})%</style> <style=cStack>\(\+(?:\d{1,6})% per stack\)</style>"),
						"{0}{1}",
						ItD.Damage($"{ItD.V(damageBase)}%"),
						ItD.Stack(damageStack, "+{0:0.##}%")),
					new(ItD.Utility("15 seconds"),
						ItD.Utility("{0} seconds"),
						ItD.V(cooldownBase),
						ItD.Stack(cooldownStack, cooldownStackType, "{0:-0.##;-0.##}%")),
				]);
		}

		protected override void ApplyHooks() {
			IL.RoR2.HealthComponent.TakeDamageProcess += this.HealthComponent_TakeDamageProcess;
			IL.RoR2.KnockbackFinUtil.ModifyDamageInfo += this.KnockbackFinUtil_ModifyDamageInfo;
			IL.RoR2.KnockbackFinUtil.GetCooldown += this.KnockbackFinUtil_GetCooldown;
		}

		private void HealthComponent_TakeDamageProcess(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld(typeof(KnockbackFinUtil), nameof(KnockbackFinUtil.baseJuggles))))
				throw new ILSeekException(il, "base juggles", nameof(HealthComponent.TakeDamageProcess));

			Instruction baseJugglesInstr = cursor.Prev;

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld(typeof(KnockbackFinUtil), nameof(KnockbackFinUtil.jugglesPerStackBeyond1))))
				throw new ILSeekException(il, "stack juggles", nameof(HealthComponent.TakeDamageProcess));

			cursor.EmitDelegate(this.ModifyStackJuggles);

			cursor.Goto(baseJugglesInstr, MoveType.After);
			cursor.EmitDelegate(this.ModifyBaseJuggles);
		}

		private int ModifyBaseJuggles(int baseJuggles) {
			if(!this.Enabled)
				return baseJuggles;

			return this.airLaunchesBase.Value;
		}

		private int ModifyStackJuggles(int stackJuggles) {
			if(!this.Enabled)
				return stackJuggles;

			return this.airLaunchesStack.Value;
		}

		private void KnockbackFinUtil_ModifyDamageInfo(ILContext il) {
			ILCursor cursor = new(il);

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld(typeof(KnockbackFinUtil), nameof(KnockbackFinUtil.damageAddPerJuggle))))
				throw new ILSeekException(il, "damageAddPerJuggle", nameof(KnockbackFinUtil.ModifyDamageInfo));

			cursor.Emit(OpCodes.Ldarg_1);
			cursor.EmitDelegate(this.ModifyDamage);
		}

		private float ModifyDamage(float baseDamage, CharacterBody attacker) {
			if(!this.Enabled || attacker?.inventory == null)
				return baseDamage;

			if(attacker == null) {
#if DEBUG
				Plugin.Logging.LogWarning("Attacker was null when trying to modify damage.");
#endif

				return baseDamage;
			}

			if(attacker.inventory == null) {
#if DEBUG
				Plugin.Logging.LogWarning("Attacker inventory was null when trying to modify damage.");
#endif

				return baseDamage;
			}

			float initial = this.damageBase.Value * 0.01f;
			float perStack = this.damageStack.Value * 0.01f;
			int itemCount = attacker.inventory.GetItemCountEffective(DLC2Content.Items.KnockBackHitEnemies);

			return ItemStacking.Linear(initial, perStack, itemCount);
		}

		private void KnockbackFinUtil_GetCooldown(ILContext il) {
			ILCursor cursor = new(il);

			//minCooldown and maxCooldown have the same value (15f) lerped w/hyperbolic 0.3 scaling for some reason.
			//It's probably a vestige of a previous tentative vanilla stacking.
			//Could probably remove the entire function but let's play it safer and just pass the two read-sites here through delegates.

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld(typeof(KnockbackFinUtil), nameof(KnockbackFinUtil.minCooldown))))
				throw new ILSeekException(il, "minCooldown", nameof(KnockbackFinUtil.GetCooldown));

			Instruction minCooldownIstr = cursor.Prev;

			if(!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdsfld(typeof(KnockbackFinUtil), nameof(KnockbackFinUtil.maxCooldown))))
				throw new ILSeekException(il, "maxCooldown", nameof(KnockbackFinUtil.GetCooldown));

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyCooldown);

			cursor.Goto(minCooldownIstr, MoveType.After);
			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(this.ModifyCooldown);
		}

		private float ModifyCooldown(float cooldown, int itemCount) {
			if(!this.Enabled || itemCount <= 0)
				return cooldown;

			float baseValue = this.cooldownBase.Value;
			float perStack = this.cooldownStack.Value * 0.01f;
			float minValue = this.cooldownCap.Value * 0.01f;

			float factor = this.cooldownStackType.Value switch {
				ModSettings.BreachingFinCooldownStacking.Linear
					=> ItemStacking.Linear(1f, -perStack, itemCount),

				ModSettings.BreachingFinCooldownStacking.Reciprocal
					=> ItemStacking.Reciprocal(0f, perStack, itemCount),

				ModSettings.BreachingFinCooldownStacking.Exponential
					=> ItemStacking.Exponential(1f - perStack, itemCount - 1),

				_ => 1f,
			};

			return MathF.Max(minValue, baseValue * factor);
		}
	}
}
