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

Key gotchas captured: (1) `DuckGame.csproj` is old-style non-SDK, so engine `.cs` needs an
explicit `<Compile Include>` (not relevant inside a mod, which compiles all `.cs`);
(2) AmmoType/Thing/NetMessage network indices come from discovery order + type-name hashes,
so all multiplayer participants must run the same content/version; (3) mod metadata comes
from `AssemblyInfo.cs`, not `mod.conf`; (4) mod assets load by path via `Mod.GetPath`, not
the global atlas name.
