# SuperDuck

A magical guns & equipment mod for Duck Game Rebuilt.

## Items

### Gandalf's Staff (`GandalfsStaff`)
A staff that casts instead of shooting. On press, **every other duck** is thrown into a
ragdoll and **floats slowly upward for ~3 seconds**, then drops and recovers. Holds 3 casts
with a long cooldown between them. Found in the level editor under the **SuperDuck** group.

How it works:
- `GandalfsStaff.cs` — the `Gun`. `Fire()` is overridden to spawn one `GandalfFloat` effect
  per other duck (authority-gated so it's network-safe — only the owner spawns the effects).
- `GandalfFloat.cs` — a short-lived `Thing`, one per affected duck. It keeps the duck
  ragdolled, zeroes the ragdoll parts' `extraGravMultiplier` (weightless), and eases them
  into a slow upward drift. After 180 frames it restores gravity and lets the duck recover.

## Layout
```
SuperDuck/
├─ mod.conf            manifest (loading flags)
├─ SuperDuckMod.cs     entry Mod class + assembly metadata
├─ GandalfsStaff.cs    the staff (Gun)
├─ GandalfFloat.cs     the levitation status effect (Thing)
└─ content/sprites/    drop custom art here (staff uses a placeholder atlas sprite for now)
```

## Install / test locally
Copy (or symlink) this `SuperDuck/` folder into your Duck Game mods directory:
```
<Documents>/DuckGame/Mods/SuperDuck/
```
Launch DGR (Steam running). The mod compiles on startup; compile errors land in
`SuperDuck_build.log`. Place the staff from the editor's **SuperDuck** group and test.

For the full lifecycle (multiplayer compatibility, Workshop publishing) see
`docs/modding-guide.md` and `docs/guns-and-equipment-guide.md`.
