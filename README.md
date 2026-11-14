# Stack Fix
![masterpiece](https://github.com/PapitaConPure/StackFix/blob/master/masterpiece.png?raw=true)

This is an opinionated balance mod that makes a few problematic items have a better initial effect + stacking behavior. The default values are what I personally use and should be a good starting point.

The mod is server-side, and all players needs the same version.

Supports `Rune580-Risk Of Options` and `DropPod-LookingGlass`!

## Item Fixes
This is a summary of how the mod affects items with default settings.
* **Bolstering Lantern**: Lesser change. Very small reduction of initial attack speed per close character with slightly more scaling (configurable). Maximum character count for the buff is the same as vanilla, but was made configurable as well.
* **Eclipse Lite**: Lesser change. Reduced initial barrier gain (configurable), but increased barrier gain per stack (configurable).
* **Warped Echo**: Substantial change. Reduced initial damage absorption (configurable) and removed damage spread count stacking (configurable). Damage absorption and cooldown reduction now scale per stack (fully configurable).
* **Chance Doll**: Nerf. Reduced initial improved reward chance (configurable). Increased additional chance per stack value but it now scales hyperbolically (configurable).
* **Faraday Spur**: Great initial nerf, great stacking buff. Reduced initial movement speed and jump height gains (configurable), as well as initial and per-stack explosion damage (configurable). Movement speed and jump height gains now scale linearly per stack (configurable).
* **Prayer Beads**: Initial nerf, stacking buff. Reduced initial bonus stats (configurable), but increased bonus stats per stack (configurable).
* **Networked Suffering**: Initial nerf, stacking buff. Reduced initial maximum spread count and damage (configurable). Percentage of damage sent to the global pool now scales linearly a bit per stack (configurable).
* There's more planned, but that's it for now...

## Configuration
There's a section for each item fix. I invite you to carefully inspect the differences between the default and vanilla values for each fix, as it'll give you a good insight on what to change whenever you feel something's off. If you completely disagree with one or more fixes, you can turn them off individually.

Some settings determine which formula is used to control how a particular effect scales per stack. These are the available choices:
* **Linear**: effect scales by a fixed amount per stack.
* **Hyperbolic**: effect scales with diminishing returns per stack, approaching a limit.
* **Reciprocal**: effect decreases proportionally per stack.
* **Exponential**: effect compounds by a constant factor per stack.

### General
| Name                    | Type    | Default | Vanilla | Description |
|-------------------------|---------|---------|---------|-------------|
| Enable Mod              | Toggle  | On      | Off     | Turn this whole thing on or off (or maybe). |
| Fix Bolstering Lantern  | Toggle  | On      | Off     | Enable Bolstering Lantern fixes. |
| Fix Eclipse Lite        | Toggle  | On      | Off     | Enable Eclipse Lite fixes. |
| Fix Warped Echo         | Toggle  | On      | Off     | Enable Warped Echo fixes. |
| Fix Chance Doll         | Toggle  | On      | Off     | Enable Chance Doll fixes. |
| Fix Faraday Spur        | Toggle  | On      | Off     | Enable Faraday Spur fixes. |
| Fix Prayer Beads        | Toggle  | On      | Off     | Enable Prayer Beads fixes. |
| Fix Networked Suffering | Toggle  | On      | Off     | Enable Networked Suffering fixes. |

General settings act as umbrella toggles for their respective features. Turn off whatever you don't want.

### Item: Bolstering Lantern
| Name                          | Type    | Default | Vanilla | Description |
|-------------------------------|---------|---------|---------|-------------|
| Base Attack Speed             | Percent | `6%`    | `6.5%`  | Initial percentage of attack speed gained per character in close proximity. |
| Attack Speed per Stack        | Percent | `+4%`   | `+3.5%` | Percentage of attack speed gained per character in close proximity per stack (*linear*). |
| Base Max Character Count      | Integer | `3`     | `3`     | Initial maximum number of characters in close range that count towards the effect. |
| Max Character Count per Stack | Integer | `+1`    | `+1`    | Maximum number of characters in close range that count towards the effect per stack (*linear*). |

With default settings, Bolstering Lantern is almost exactly the same as vanilla but with a slightly worse initial attack speed and decently better attack speed stacking.

### Item: Eclipse Lite
| Name                           | Type    | Default    | Vanilla  | Description |
|--------------------------------|---------|------------|----------|-------------|
| Base Cooldown Barrier          | Percent | `0.5%`     | `1%`     | Initial percentage of max health gained in barrier for each second of a skill's cooldown. |
| Cooldown Barrier per Stack     | Percent | `+0.5%`    | `+0.25%` | Percentage of max health gained in barrier per stack (*configurable formula*) for each second of a skill's cooldown. |
| Cooldown Barrier Stacking Type | Choice  | Hyperbolic | Linear   | Determines how "Cooldown Barrier per Stack" scales with each additional stack. |

With default settings, Eclipse Lite starts a bit weaker but catches up to vanilla at 3 stacks and becomes ~1.6x as effective as vanilla at ~20 stacks.

If you change "Cooldown Barrier Stacking Type" to the vanilla **Linear** stacking, it still catches up at 3 stacks but becomes ~1.75x as effective as vanilla at ~20 stacks.

### Item: Warped Echo
| Name                             | Type       | Default     | Vanilla | Description |
|----------------------------------|------------|-------------|---------|-------------|
| Base Damage Reduction            | Percent    | `5%`        | `10%`   | Initial damage absorption percentage on trigger. |
| Damage Reduction per Stack       | Percent    | `+5%`       | `+0%`   | Damage absorption percentage on trigger per stack (*configurable formula*). |
| Damage Reduction Stacking Type   | Choice     | Reciprocal  | N/A     | Determines how "Damage Reduction per Stack" scales with each additional stack. |
| Base Damage Spread               | Integer    | `3`         | `3`     | Initial number of instances in which the absorbed damage spreads on trigger. |
| Damage Spread per Stack          | Integer    | `+0`        | `+1`    | Number of instances in which the absorbed damage spreads on trigger per stack (*linear*). |
| Base Cooldown                    | Fractional | `15.0`      | `15.0`  | Initial cooldown before damage absorption can trigger again. |
| Cooldown Reduction per Stack     | Percent    | `+2.5%`     | `+0%`   | Damage absorption cooldown reduction per stack (*configurable formula*). |
| Cooldown Reduction Stacking Type | Choice     | Exponential | N/A     | Determines how "Cooldown Reduction per Stack" scales with each additional stack. |

With default settings, Warped Echo starts with a bit less damage absorption than vanilla and loses its spread count stacking in favor of stacking both damage absorption and cooldown reduction, which is quite the step up.

### Item: Chance Doll
| Name                 | Type    | Default    | Vanilla | Description |
|----------------------|---------|------------|---------|-------------|
| Base Chance          | Percent | `25%`      | `40%`   | Initial improved Shrine of Chance reward chance. |
| Chance per Stack     | Percent | `+25%`     | `+10%`  | Improved Shrine of Chance reward chance per stack (*configurable formula*). |
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
| Base Damage              | Percent | `300%`  | `400%`  | Initial discharge explosion damage. |
| Damage per Stack         | Percent | `+200%` | `+280%` | Discharge explosion damage per stack (*linear*). |

With default settings, Faraday Spur reaches the vanilla speed/jump gains at 4 stacks.

I have wondered for quite a bit if perhaps the first stack feels somewhat underwhelming now, but this should really incentivize printing and actually stacking the item instead of giving you a free win after picking it up once.

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

### Megumin
| Name    | Type    | Default | Vanilla | Description |
|---------|---------|---------|---------|-------------|
| Megumin | Megumin | Megumin | Megumin | Megumin     |

Megumin.

## Contact
If something breaks real bad, there's some incompatibility you think I should know about, you think I'm very bad at explaining shit (I am), or you want to communicate whatever doesn't fall into those categories, feel free to reach out (please be nice or I'll cry).
* Discord: `papitaconpure`

## Compatibility notes
From what I've tested: `Goorakh-Quality` seems to work fine. I haven't tested the thing in multiplayer yet, though. I'd appreciate it if you let me know if it works! I'll eventually test it myself.

If any single item breaks because of another mod or something, you can disable the individual fix. If that doesn't work either, please do let me know.

### Nerd
I've never really worked with IL Hooks nor overriden any item behavior before, but I can at least confirm it works perfectly in singleplayer. I also mess with as small a portion of IL code as possible, so I don't think networking/compat should be much of an issue. I did make it so that everyone must have the mod with the same version just in case, though. You never know.

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
