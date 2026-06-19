---
name: superduck-mod
description: The user's own SuperDuck content mod and its first item, Gandalf's Staff
metadata:
  type: project
---

The user is building a standalone DGR content mod called **SuperDuck** (compile-at-startup,
not an engine fork) — applying the guides in [[guns-equipment-guide]]. It lives in the repo
at `SuperDuck/` (copy/symlink into `<Documents>/DuckGame/Mods/SuperDuck/` to test).

Files: `mod.conf`, `SuperDuckMod.cs` (entry `Mod` + assembly metadata, namespace
`DuckGame.SuperDuck`), `GandalfsStaff.cs`, `GandalfFloat.cs`, `content/sprites/`.

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
or ghosts crash with a null target. Staff art is a placeholder atlas sprite (`"sword"`); swap
to `content/sprites/` art via `Mod.GetPath` when available.
