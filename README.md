# Stack Fix
![masterpiece](https://github.com/PapitaConPure/StackFix/blob/master/masterpiece.png?raw=true)

This is an opinionated balance mod that makes a few problematic items have a better initial effect + stacking behavior. The default values are what I personally use and should be a good starting point.

The mod is server-side, and all players needs the same version.

Supports `Rune580-Risk Of Options` and `DropPod-LookingGlass`!

## Item Fixes
This is a summary of how the mod affects items with default settings.
* **Bolstering Lantern**: Lesser change. Very small reduction of initial attack speed per close character with slightly more scaling (configurable). Maximum character count for the buff is the same as vanilla, but was made configurable as well.
* **Chronic Expansion**: Initial nerf, stacking tweak. Very small reduction of initial combo damage gains (configurable). Slightly increased combo damage gains per stack and considerably reduced max combo per stack.
* **Eclipse Lite**: Lesser change. Reduced initial barrier gain (configurable), but increased barrier gain per stack (configurable).
* **Warped Echo**: Substantial change. Reduced initial damage absorption (configurable) and removed damage spread count stacking (configurable). Damage absorption and cooldown reduction now scale per stack (fully configurable).
* **Box of Dynamite**: Stacking buff. Stacks cooldown reduction instead of damage (configurable).
* **Chance Doll**: Nerf. Reduced initial improved reward chance (configurable). Increased additional chance per stack value but it now scales hyperbolically (configurable).
* **Faraday Spur**: Great initial nerf, great stacking buff, additional tweaks. Dramatically reduced initial movement speed and jump height gains (configurable), as well as initial discharge damage (configurable). Movement speed and jump height gains now scale linearly per stack (configurable). Discharge damage doesn't increase per stack (configurable), but discharge radius does (configurable). Moved from the 'Damage' category to the 'Utility' category (configurable), which better matches this stacking behavior.
* **Luminous Shot**: Great initial nerf, stacking buff. Dramatically reduced initial damage and max charges (configurable), but increased damage per stack (configurable). Cooldown reduction is half that of vanilla, but was made configurable and it may also be configured to stack.
* **Prayer Beads**: Initial nerf, stacking buff. Reduced initial bonus stats (configurable), but increased bonus stats per stack (configurable).
* **Networked Suffering**: Initial nerf, stacking buff. Reduced initial maximum spread count and damage (configurable). Percentage of damage sent to the global pool now scales linearly a bit per stack (configurable).
* **Sonorous Whispers**: Initial nerf, lesser stacking buff. Nullified initial elite item drop chance (configurable). Now increases said chance by +2.5% per stack instead of +1% (configurable).
* There's more planned, but that's it for now...

## Configuration
There's a section for each item fix. I invite you to carefully inspect the differences between the default and vanilla values for each fix, as it'll give you a good insight on what to change whenever you feel something's off. If you completely disagree with one or more fixes, you can turn them off individually.

Some settings determine which formula is used to control how a particular effect scales per stack. These are the available choices:
* **Linear**: effect scales by a fixed amount per stack.
* **Hyperbolic**: effect scales with diminishing returns per stack, approaching a limit.
* **Reciprocal**: effect decreases proportionally per stack.
* **Exponential**: effect compounds by a constant factor per stack.

### General
| Name                    | Type   | Default | Vanilla | Description |
|-------------------------|--------|---------|---------|-------------|
| Enable Mod              | Toggle | On      | Off     | Turn this whole thing on or off (or maybe). |
| Fix Bolstering Lantern  | Toggle | On      | Off     | Enable Bolstering Lantern fixes. |
| Fix Chronic Expansion   | Toggle | On      | Off     | Enable Chronic Expansion fixes. |
| Fix Eclipse Lite        | Toggle | On      | Off     | Enable Eclipse Lite fixes. |
| Fix Warped Echo         | Toggle | On      | Off     | Enable Warped Echo fixes. |
| Fix Box of Dynamite     | Toggle | On      | Off     | Enable Box of Dynamite fixes. |
| Fix Chance Doll         | Toggle | On      | Off     | Enable Chance Doll fixes. |
| Fix Faraday Spur        | Toggle | On      | Off     | Enable Faraday Spur fixes. |
| Fix Luminous Shot       | Toggle | On      | Off     | Enable Luminous Shot fixes. |
| Fix Prayer Beads        | Toggle | On      | Off     | Enable Prayer Beads fixes. |
| Fix Networked Suffering | Toggle | On      | Off     | Enable Networked Suffering fixes. |
| Fix Sonorous Whispers   | Toggle | On      | Off     | Enable Sonorous Whispers fixes. |

General settings act as umbrella toggles for their respective features. Turn off whatever you don't want.

### Item: Bolstering Lantern
| Name                          | Type    | Default | Vanilla | Description |
|-------------------------------|---------|---------|---------|-------------|
| Base Attack Speed             | Percent | `6%`    | `6.5%`  | Initial percentage of attack speed gained per character in close proximity. |
| Attack Speed per Stack        | Percent | `+4%`   | `+3.5%` | Percentage of attack speed gained per character in close proximity per stack (*linear*). |
| Base Max Character Count      | Integer | `3`     | `3`     | Initial maximum number of characters in close range that count towards the effect. |
| Max Character Count per Stack | Integer | `+1`    | `+1`    | Maximum number of characters in close range that count towards the effect per stack (*linear*). |

With default settings, Bolstering Lantern is almost exactly the same as vanilla but with a slightly worse initial attack speed and decently better attack speed stacking.

### Item: Chronic Expansion
| Name                | Type    | Default | Vanilla | Description |
|---------------------|---------|---------|---------|-------------|
| Base Damage         | Percent | `3%`    | `3.5%`  | Initial percentage of damage gained with multi-kill combos. |
| Damage per Stack    | Percent | `+1.5%` | `+1%`   | Percentage of damage gained with multi-kill combos per stack (*linear*). |
| Base Max Combo      | Integer | `10`    | `10`    | Initial maximum multi-kill combo number. |
| Max Combo per Stack | Integer | `+2`    | `+5`    | Maximum multi-kill combo number per stack (*linear*). |

With default settings, Chronic Expansion has a slightly weaker initial max combo gain (+30% damage), but stacks in a way that keeps the maximum combo per stack within reasonable numbers, balanced by the fact that each combo stack gives a bit more damage per stack. Having quadratic-type stacking on a white item is certainly a choice Gearbox has made. I'm weak and scared to turn off the stacking of one of the two stats, but you can set any of the two to +0 and the item's potential damage gains will stack linearly.

### Item: Eclipse Lite
| Name                           | Type    | Default    | Vanilla  | Description |
|--------------------------------|---------|------------|----------|-------------|
| Base Cooldown Barrier          | Percent | `0.5%`     | `1%`     | Initial percentage of max health gained in barrier for each second of a skill's cooldown. |
| Cooldown Barrier per Stack     | Percent | `+0.5%`    | `+0.25%` | Percentage of max health gained in barrier per stack (*configurable formula*) for each second of a skill's cooldown. |
| Cooldown Barrier Stacking Type | Choice  | Hyperbolic | Linear   | Determines how "Cooldown Barrier per Stack" scales with each additional stack. |

With default settings, Eclipse Lite starts a bit weaker but catches up to vanilla at 3 stacks and becomes ~1.6x as effective as vanilla at ~20 stacks.

If you change "Cooldown Barrier Stacking Type" to the vanilla **Linear** stacking, it still catches up at 3 stacks but becomes ~1.75x as effective as vanilla at ~20 stacks.

### Item: Warped Echo
| Name                             | Type    | Default     | Vanilla | Description |
|----------------------------------|---------|-------------|---------|-------------|
| Base Damage Reduction            | Percent | `5%`        | `10%`   | Initial damage absorption percentage on trigger. |
| Damage Reduction per Stack       | Percent | `+5%`       | `+0%`   | Damage absorption percentage on trigger per stack (*configurable formula*). |
| Damage Reduction Stacking Type   | Choice  | Reciprocal  | N/A     | Determines how "Damage Reduction per Stack" scales with each additional stack. |
| Base Damage Spread               | Integer | `3`         | `3`     | Initial number of instances in which the absorbed damage spreads on trigger. |
| Damage Spread per Stack          | Integer | `+0`        | `+1`    | Number of instances in which the absorbed damage spreads on trigger per stack (*linear*). |
| Base Cooldown                    | Seconds | `15s`       | `15s`   | Initial cooldown before damage absorption can trigger again. |
| Cooldown Reduction per Stack     | Percent | `+2.5%`     | `+0%`   | Damage absorption cooldown reduction per stack (*configurable formula*). |
| Cooldown Reduction Stacking Type | Choice  | Exponential | N/A     | Determines how "Cooldown Reduction per Stack" scales with each additional stack. |

With default settings, Warped Echo starts with a bit less damage absorption than vanilla and loses its spread count stacking in favor of stacking both damage absorption and cooldown reduction, which is quite the step up.

### Item: Box of Dynamite
| Name                             | Type    | Default     | Vanilla | Description |
|----------------------------------|---------|-------------|---------|-------------|
| Base Damage                      | Percent | `240%`      | `240%`  | Initial drone dynamite damage percentage. |
| Damage per Stack                 | Percent | `+0%`       | `+85%`  | Drone dynamite damage percentage per stack (*linear*). |
| Base Cooldown                    | Seconds | `10s`       | `10s`   | Initial cooldown between a drone's dynamites. |
| Cooldown Reduction per Stack     | Percent | `+10%`      | `+0%`   | Drone dynamite cooldown reduction per stack (*configurable formula*). |
| Cooldown Reduction Stacking Type | Choice  | Exponential | N/A     | Determines how "Cooldown Reduction per Stack" scales with each additional stack. |
| Minimum Cooldown                 | Seconds | `0.25s`     | N/A     | Minimum cooldown between a drone's dynamites. |

With default settings, Box of Dynamite just stacks better. No initial nerf.

### Item: Chance Doll
| Name                 | Type    | Default    | Vanilla | Description |
|----------------------|---------|------------|---------|-------------|
| Base Chance          | Percent | `25%`      | `40%`   | Initial improved Shrine of Chance reward chance percentage. |
| Chance per Stack     | Percent | `+25%`     | `+10%`  | Improved Shrine of Chance reward chance percentage per stack (*configurable formula*). |
| Chance Stacking Type | Choice  | Hyperbolic | Linear  | Determines how "Chance per Stack" scales with each additional stack. |

With default settings, Chance Doll starts out at 20% (because of how **Hyperbolic** scaling works), approaches the vanilla value stacking curve between 2 and 3 stacks, and doesn't keep up with vanilla scaling afterwards. At 7 stacks, vanilla reaches a 100% chance for an improved reward. Meanwhile, at 10 stacks, this default fix sits at ~71% chance (+1.9% next stack).

This is a decently sized nerf, justified by the fact that stacking more than 7 Chance Dolls is not useless and that the initial effect doesn't yield 4 times more value than every subsequent stack.

### Item: Faraday Spur
| Name                     | Type    | Default | Vanilla | Description |
|--------------------------|---------|---------|---------|-------------|
| Base Movement Speed      | Percent | `70%`   | `160%`  | Initial movement speed gains at max charge. |
| Movement Speed per Stack | Percent | `+30%`  | `+0%`   | Movement speed gains per stack (*linear*) at max charge. |
| Base Jump Height         | Percent | `80%`   | `200%`  | Initial jump height gains at max charge. |
| Jump Height per Stack    | Percent | `+40%`  | `+0%`   | Jump height gains per stack (*linear*) at max charge. |
| Base Damage              | Percent | `360%`  | `400%`  | Initial discharge explosion damage. |
| Damage per Stack         | Percent | `+0%`   | `+280%` | Discharge explosion damage per stack (*linear*). |
| Base Distance            | Meters  | `32.3m` | `32.3m` | Initial discharge explosion maximum distance. |
| Distance per Stack       | Meters  | `+7.5m` | `+7.5m` | Discharge explosion maximum distance increase per stack (*linear*). |

With default settings, Faraday Spur reaches the vanilla speed/jump gains at 4 stacks.

I have wondered for quite a bit if perhaps the first stack feels somewhat underwhelming now, but this should really incentivize printing and actually stacking the item instead of giving you a free win after picking it up once.

### Item: Luminous Shot
| Name                             | Type    | Default    | Vanilla | Description |
|----------------------------------|---------|------------|---------|-------------|
| Base Max Charges                 | Integer | `3`        | `5`     | Initial maximum number of lightning strike charges. |
| Max Charges per Stack            | Integer | `+1`       | `+1`    | Maximum number of lightning strike charges per stack (*linear*). |
| Base Damage                      | Percent | `140%`     | `175%`  | Initial lightning strike damage. |
| Damage per Stack                 | Percent | `+60%`     | `+50%`  | Lightning strike damage per stack (*linear*). |
| Base Cooldown Reduction          | Percent | `10%`      | `20%`   | Initial Secondary skill cooldown reduction. |
| Cooldown Reduction per Stack     | Percent | `+0%`      | `+0%`   | Secondary skill cooldown reduction per stack (*configurable formula*). |
| Cooldown Reduction Stacking Type | Choice  | Reciprocal | N/A     | Determines how "Cooldown Reduction per Stack" scales with each additional stack. |

With default settings, Luminous Shot's initial maximum potential TOTAL damage of 3x140%=420% is about half as effective than vanilla's 5x175%=875%. At 3 stacks, the initial vanilla potential is almost reached. At 4 stacks, the vanilla initial potential it is surpassed. At ~15 stacks, it fully catches up to vanilla stacking and surpasses it.

If you so desire, you can make Secondary cooldown reduction stack so that it also eventually reaches and surpasses the vanilla value. I wouldn't really recommend it, though.

### Item: Prayer Beads
| Name                  | Type    | Default | Vanilla | Description |
|-----------------------|---------|---------|---------|-------------|
| Base Extra Stats      | Percent | `10%`   | `20%`   | Initial percentage of extra experience stored by the item. |
| Extra Stats per Stack | Percent | `+10%`  | `+5%`   | Percentage of extra experience stored by the item per stack (*linear*). |

With default settings, Prayer Beads reach vanilla extra stat gains at 3 stacks and scale twice as much per stack. The initial effect is halved, but it incentivizes stacking the item instead of waiting to scrap it before picking up another stack so that the effectiveness isn't cut by a quarter.

### Item: Networked Suffering
| Name                   | Type    | Default | Vanilla | Description |
|------------------------|---------|---------|---------|-------------|
| Base Virus Spread      | Integer | `3`     | `4`     | Initial maximum number of infected enemies. |
| Virus Spread per Stack | Integer | `+2`    | `+2`    | Maximum number of infected enemies per stack (*linear*). |
| Base Damage            | Percent | `35%`   | `45%`   | Initial virus damage duplication. |
| Damage per Stack       | Percent | `+1%`   | `+0%`   | Virus damage duplication per stack (*linear*). |

The initial virus damage duplication needed a small nerf for a long time. I also went aheand and reduced the maximum spread by 1 on the first stack.

With default settings, you can actually stack the damage duplication, but need TEN STACKS of the thing to reach the vanilla value. It isn't really a small effect, though. Every 1% is impactful and the default spread per stack of 2 also contributes to the total damage. In fact, I'd say you should consider lowering the damage per stack to +0% (disable stacking). Still, I personally like +1~5% per stack as it's a red item.

### Item: Sonorous Whispers
| Name                            | Type    | Default | Vanilla | Description |
|---------------------------------|---------|---------|---------|-------------|
| Base Elite Drop Chance          | Percent | `0%`    | `5%`    | Initial chance percentage for elite monsters to drop an item. |
| Elite Drop Chance per Stack     | Percent | `+2.5%` | `+1%`   | Additional chance percentage for elite monsters to drop an item per stack (*configurable formula*). |
| Elite Drop Chance Stacking Type | Choice  | Linear  | Linear  | Determines how "Elite Drop Chance per Stack" scales with each additional stack. |

With default settings, Sonorous Whispers doesn't make elite monsters drop items initially. The elite item drop chance is equal to the vanilla initial 5% chance at 3 stacks, and surpasses that of vanilla by 1% at 5 stacks (remember this a red item, so in practice this is mostly just a nerf).

### Megumin
| Name    | Type    | Default | Vanilla | Description |
|---------|---------|---------|---------|-------------|
| Megumin | Megumin | Megumin | Megumin | Megumin     |

Megumin.

## Contact
If something breaks real bad, there's some incompatibility you think I should know about, you think I'm very bad at explaining shit (I am), or you want to communicate whatever doesn't fall into those categories, feel free to reach out (please be nice or I'll cry).
* Discord: `papitaconpure`

## Compatibility notes
I played one long run with this mod and some QoL mods in multiplayer and everything seemed to work.

If any single item breaks because of another mod or something, you can disable the individual fix. If that doesn't work either, please do let me know.

If you suspect there's a problem with this mod but you're playing with AI-generated mods, try disabling those first to see if the issue goes away, as AI-generated mods don't usually consider compatibility.

### Nerd
I've never really worked with IL Hooks nor overriden any item behavior before. I mess with as small a portion of IL code as possible, so I don't think networking/compat should be much of an issue, but I did make it so that everyone must have the mod with the same version just in case. You never know.

I assume shit may break if:
* Another mod removes an entire IL section this mod expects to match for an item fix.
* This mod removes an entire IL section another mod expects to match (I try not to remove instructions if possible).
* Another mod tries to match a very specific sequence of IL code this mod has already modified.
* This mod tries to match a very specific sequence of IL code another mod has already modified.

If any one item fix fails because of an IL mismatch, the other item fixes should still work. I don't yet communicate when an item fix fails outside of logs, though. I'm still figuring that out.

## Changelog
Read the thing on [Thunderstore](https://thunderstore.io/c/riskofrain2/p/PapitaConPure/StackFix/changelog) or [GitHub](https://github.com/PapitaConPure/StackFix/blob/master/CHANGELOG.md).

## Additional Info
Build info [here](https://github.com/PapitaConPure/StackFix/blob/master/wik/build.md).

in megumin i trust for she is the light ahead
