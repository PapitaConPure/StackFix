# Stack Fix
![masterpiece](https://github.com/PapitaConPure/StackFix/blob/master/masterpiece.png?raw=true)

Makes some items have a better initial effect + stacking behavior.

It's server-side, and everyone needs the mod. The default values are what I personally use and should be a good starting point for balancing.

## Features
* **Eclipse Lite**: reduced initial barrier gain but increased its stacking (configurable).
* **Faraday Spur**: reduced initial movement speed and jump height gains (configurable), as well as initial and per-stack explosion damage (configurable). Movement speed and jump height gains now scale linearly per stack (configurable).
* **Networked Suffering**: reduced initial maximum spread count and damage (configurable). 
* There's more planned, but that's it for now...

## Configuration
Some settings determine which formula is used to control how a particular effect scales per stack. These are available choices:
* **Linear**: effect scales by a fixed amount per stack.
* **Hyperbolic**: effect scales with diminishing returns per stack, approaching a limit.
* **Reciprocal**: effect decreases proportionally per stack.
* **Exponential**: effect compounds by a constant factor per stack.

### General
| Name                    | Type    | Default | Vanilla | Description |
|-------------------------|---------|---------|---------|-------------|
| Enable Mod              | Toggle  | On      | Off     | Turn this whole thing on or off (or maybe). |
| Fix Eclipse Lite        | Toggle  | On      | Off     | Enable Eclipse Lite fixes. |
| Fix Faraday Spur        | Toggle  | On      | Off     | Enable Faraday Spur fixes. |
| Fix Networked Suffering | Toggle  | On      | Off     | Enable Networked Suffering fixes. |

General settings act as umbrella toggles for their respective features. Turn off whatever you don't want.

### Item: Eclipse Lite
| Name                           | Type    | Default    | Vanilla  | Description |
|--------------------------------|---------|------------|----------|-------------|
| Base Cooldown Barrier          | Percent | `0.5%`     | `1%`     | Initial percentage of max health gained in barrier for each second of a skill's cooldown. |
| Cooldown Barrier per Stack     | Percent | `+0.5%`    | `+0.25%` | Percentage of max health gained in barrier per stack (*configurable formula*) for each second of a skill's cooldown. |
| Cooldown Barrier Stacking Type | Choice  | Hyperbolic | Linear   | Determines how "Cooldown Barrier per Stack" scales with each additional stack. |

With default settings, Eclipse Lite starts a bit weaker but catches up to vanilla at 3 stacks and becomes ~1.6x as effective as vanilla at ~20 stacks.

If you change "Cooldown Barrier Stacking Type" to the vanilla **Linear** stacking, it still catches up at 3 stacks but becomes ~1.75x as effective as vanilla at ~20 stacks.

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
| Name                   | Type    | Default | Vanilla | Description |
|------------------------|---------|---------|---------|-------------|
| Megumin                | Megumin | Megumin | Megumin | Megumin     |

Megumin.

## Contact
* Discord: `papitaconpure` (please be nice or I'll cry)

## Compatibility notes
I don't knowwwwwwwwwwwwwwwwwwwwwwwwwww but if any single item breaks because of another mod or something, you can disable the individual fix.

### Nerd
I've never really worked with IL Hooks nor overriden any item behavior before, and as of writing this, I haven't tested the thing in multiplayer, but I can at least confirm it works perfectly in singleplayer.

## Changelog
Read the thing on [Thunderstore](https://thunderstore.io/c/riskofrain2/p/PapitaConPure/StackFix/changelog) or [GitHub](https://github.com/PapitaConPure/StackFix/blob/master/CHANGELOG.md).

## Additional Info
Build info [here](https://github.com/PapitaConPure/StackFix/blob/master/wik/build.md).

in megumin i trust for she is the light ahead
