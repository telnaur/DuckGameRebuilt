---
name: superduck-mod
description: The user's own SuperDuck content mod and its first item, Gandalf's Staff
metadata:
  type: project
---

The user is building a standalone DGR content mod called **SuperDuck** (compile-at-startup,
not an engine fork) — applying the guides in [[guns-equipment-guide]]. It lives in the repo
at `SuperDuck/`. The game loads it from `%AppData%/Roaming/DuckGame/Mods/SuperDuck/`, which is
a **manual separate copy** (NOT a symlink, verified 2026-06-19) — repo edits must be copied
there to take effect, and stale `SuperDuck_compiled.hash`/`.dll`/`_build.log` deleted to force
a clean recompile.

Files: `mod.conf`, `SuperDuckMod.cs` (entry `Mod` + assembly metadata, namespace
`DuckGame.SuperDuck`), `GandalfsStaff.cs`, `GandalfFloat.cs`, `content/sprites/`,
`content/SFX/`.

**Mod source is C# 5 only** (Windows CodeDom compiler) — see [[guns-equipment-guide]]. Hit
this 2026-06-19: `nameof` in `StateBinding`s broke the build; switched all to string literals.

**SuperDuck is a LOCAL mod** (`WorkshopID` 0), so maps containing the staff are flagged
`hasLocalMods` and hidden from online lobbies (shown in editor only). Confirmed via
`TEST STAFF.lev` (stores `DuckGame.SuperDuck.GandalfsStaff, SuperDuck_compiled`). Local-testing
bypass: Options → "Fast Level loading" (`IgnoreLevRestrictions`). See [[guns-equipment-guide]] /
`docs/modding-guide.md` §4.1. **Open issue (2026-06-19):** with both players on `-moddebug`,
placed staffs didn't appear online — most likely cause is mismatched local-mod copies (a stale/
non-compiling SuperDuck on one side → no GandalfsStaff type, or differing mod sets shifting
ghost-type IDs; §4.2). Next step: verify both machines have identical SuperDuck source that
compiled cleanly (check each `SuperDuck_build.log`) and the same mod set.

**Gandalf's Staff** (first item, created 2026-06-18): a `Gun` (`[EditorGroup("SuperDuck")]`)
whose `Fire()` is overridden to cast instead of shoot — spawns one `GandalfFloat` effect per
*other* duck. 3 casts, long cooldown. **GandalfFloat** is a short-lived `Thing` (one per
duck) modeled on the Ostrich mod's `Stun.cs`: keeps the target ragdolled, sets each
`RagdollPart.extraGravMultiplier = 0` (weightless — that knob, unlike `gravMultiplier`, is
NOT reset each frame by `RagdollPart.Update`), eases parts to a slow upward drift, then after
180 frames (~3s) restores gravity + `_makeActive=true` so the duck drops and recovers.

Key network-safety rule reused here (learned from Ostrich `ToyHammer.cs`): **only
`isServerForObject` spawns the networked effect Things** — spawning on every client floods
ghosts and crashes online. Effect Things need StateBindings for their `Duck` target + timer
or ghosts crash with a null target.

Staff now uses **custom art + sound** (wired 2026-06-19): `content/sprites/gandalfsstaff.png`
(11×48 vertical staff) and `content/SFX/magicstaff.wav`, both referenced via
`Mod.GetPath<SuperDuckMod>("sprites/gandalfsstaff")` / `("SFX/magicstaff")` and assigned to
`graphic` / `_fireSound`. Mod textures AND sounds register under their full GetPath key (path
minus extension), so the same `GetPath` string is the lookup key — a bare name only hits the
engine atlas.
