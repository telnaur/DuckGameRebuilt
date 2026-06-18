# Duck Game Example Mods (source reference)

A curated set of **11 Steam Workshop mods (app 312530) that ship usable C# source**, copied here
to study how the community builds new equipment, weapons, projectiles, props, and netcode.

Each folder is named `ModName [WorkshopID]`. Build-cache noise (`obj/`, `.vs/`, `*.suo`,
`StyleCop.Cache`) was stripped; **source, content/media, level files, project files, and
referenced DLLs are intact**. Source for most mods lives in `src/`; for some it lives in
`build/src/` (noted below). Auto-generated `...AssemblyAttributes.cs` files can be ignored.

## Top picks (broad, well-organized)

| Mod | Source path | What to learn |
|---|---|---|
| **HaloWeapons** | `HaloWeapons/src` | Best-organized. Clean split: `Guns/`, `Equipment/` (Thruster), `AmmoTypes/`, `Bullets/` (HomingBullet), `NetMessages/` (multiplayer sync), `Skins/`, `Materials/`, plus **Harmony `*Patched.cs`** hooks into the base game. |
| **UFFMod** | `UFFMod/build/src` | Best for **equipment**: `equipment/` (FireShield, Reflector, TornadoBoots…) using an `IQuackOverrideEquipment` interface, `equipment/bits/` effect entities, plus `stuff/blocks`, `stuff/hazards` (SentryTurret), `stuff/props`. |
| **OstrichMod** | `OstrichMod/src` | Huge variety. `AT/` = AmmoTypes, `Equip/` = equipment (AirDash, StarPlatinum) with its own `IQuackOverrideEquipment`, plus dozens of guns. .NET 4.8. |
| **Weaponized** | `Weaponized/build/src` | Largest collection (~130 files, flat folder). Guns, ammo types (`AT*.cs`), thrown/mine items, hats, deathcrate effects. High volume reference. |

## Focused / smaller examples

| Mod | Source path | What to learn |
|---|---|---|
| **TarGun** | `TarGun/build/src` | One complete gun end-to-end: gun + projectile (`TarBlast`), particles, a custom flag/status effect (`DrenchedInTar`), deathcrate hook, sound wrapper, `ModMetadata`. Great single-feature read. |
| **Deathcrates++** | `Deathcrates++/src` | Custom death-crate events / crates and configurable mod settings. |
| **BrutalDG** | `BrutalDG/src` | Gore/particle systems, screen shake, ragdolls, custom blocks/particles, custom UI menu items, options data. Non-weapon example. |
| **MoreInstruments** | `MoreInstruments/src` | Small & tidy. Reusable base class (`PitchBendableInstrument`) subclassed by Guitar/PanFlute — good inheritance example. |
| **Cornfield** | `Cornfield/build/src` | Minimal mod skeleton (`Mod.cs` + one class). |
| **Ducktrocities2** | `Ducktrocities2/src` | Single `Mod.cs` entry-point example (no project file). |
| **PokemonHatPack** | `PokemonHatPack` | Mostly an art/hat pack; minimal code (`Mod.cs` + `AssemblyInfo.cs`). |

## Suggested study order

- **Equipment** → UFFMod `equipment/` → HaloWeapons `src/Equipment/Thruster.cs` → OstrichMod `src/Equip/`.
  Note: UFFMod and OstrichMod independently define `IQuackOverrideEquipment` — compare the two approaches.
- **Weapons + projectiles + netcode** → HaloWeapons (only one cleanly separating `NetMessages/` and using Harmony patches).
- **One complete feature in a single sitting** → TarGun.
