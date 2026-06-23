---
name: guns-equipment-guide
description: Location and purpose of the user-authored weapons/equipment modding guide
metadata:
  type: project
---

The user is building their own guns/equipment for their DGR fork. They pivoted from
forking the engine to making a standalone **mod** (DGR compiles mod `.cs` from source at
startup and has an in-game Steam Workshop uploader). Authoritative modding docs were
authored in `docs/` (created 2026-06-18, untracked at creation):

- `docs/modding-guide.md` — MASTER guide: mod system/compilation, folder layout, `mod.conf`,
  `AssemblyInfo.cs` metadata, entry `Mod` class, assets/`content/`, NetMessages, multiplayer
  compatibility (dataHash), local testing, in-game Workshop publishing, distributing to
  testers, troubleshooting. Reference-backed (ModLoader.cs, ModConfiguration.cs, Mod.cs,
  UIModManagement.cs, ContentPack.cs, DuckFile.cs, Network.cs). **§3.2 is now a full asset
  reference**: required file types (sprites=`.png`, SFX=`.wav`, music=`.ogg`; nothing else
  scanned), `GetPath` (omit extension → hits preload cache), `SpriteMap` animation (sheet
  layout/`AddAnimation`/playback), pink-transparency `(255,0,255)` gated on `<PinkTransparency>`,
  `SFX.Play` params, `Music.Play`, Workshop preview/screenshot PNGs.
- `docs/guns-and-equipment-guide.md` — item-code deep dive (Gun/Equipment/AmmoType/
  StateBinding), linked from the master. Now also covers **§12 status effects via spawned
  "effect Things"** (stun/freeze/levitate/DoT): one networked `Thing` per target, owner-only
  spawn (else ghost-flood crash online), bind target+timer (ghosts skip the real ctor),
  ragdoll manipulation (`GoRagdoll`/`_makeActive`, `extraGravMultiplier` vs the every-frame-
  reset `gravMultiplier`). Added after building [[superduck-mod]] (Gandalf's Staff).

**C# 5 limit on mod source (Windows):** DGR compiles mod `.cs` at startup with the legacy
CodeDom `CSharpCodeProvider` (`ModLoader.cs:752`), which maxes at **C# 5** — so `nameof`,
`$"..."` interpolation, `?.`, expression-bodied members, etc. fail (e.g. *"The name 'nameof'
does not exist"*). Use string literals for `StateBinding("field")`. Linux uses Roslyn so it's
a platform split. Engine code is unaffected (MSBuild `LangVersion 9.0`). Documented in
`docs/modding-guide.md` §1.1 (⚠️ callout) + §9 troubleshooting. Also: the running game loads
mods from `%AppData%/DuckGame/Mods/`, a **separate copy** from the repo `SuperDuck/` folder —
edits must be synced there (no symlink) and stale `_compiled.hash`/`.dll` cleared to force a
clean recompile.

**Modded maps & online (documented in `docs/modding-guide.md` §4.1/§4.2, learned 2026-06-19):**
A custom level that contains items from a **local** (non-Workshop) mod is flagged
`modData.hasLocalMods` on save (`Editor.cs:4642`) and filtered OUT of the online lobby list
(`LSFilterMods.cs:36`) — it still shows in the editor/offline, which is the confusing symptom.
Workshop-mod maps need their `workshopIDs` accessible (`:46`). Bypass for local testing:
`DGRSettings.IgnoreLevRestrictions` (Options → "Fast Level loading"); `-moddebug` only bypasses
the hasLocalMods branch, not the workshop-subset branch. Real fix: publish to Workshop.
Separately, networked mod Things get a **ghost-type ID by FullName-sorted position over the
combined engine+mod type list** (`Editor.cs:5381-5398`), and the join-gate `thingTypesHash`
covers ONLY engine types — so players can connect yet have misaligned mod ghost IDs if their
mod sets differ at all (extra mod, different version, or a local mod that compiled differently/
failed). Result: a placed mod object silently missing online while the rest of the map loads.
Every player needs byte-identical local mods that each compiled cleanly.

Key gotchas captured: (1) `DuckGame.csproj` is old-style non-SDK, so engine `.cs` needs an
explicit `<Compile Include>` (not relevant inside a mod, which compiles all `.cs`);
(2) AmmoType/Thing/NetMessage network indices come from discovery order + type-name hashes,
so all multiplayer participants must run the same content/version; (3) mod metadata comes
from `AssemblyInfo.cs`, not `mod.conf`; (4) mod assets load by path via `Mod.GetPath`, not
the global atlas name.
