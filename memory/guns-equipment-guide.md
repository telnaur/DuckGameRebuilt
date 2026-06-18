---
name: guns-equipment-guide
description: Location and purpose of the user-authored weapons/equipment modding guide
metadata:
  type: project
---

The user is building their own guns/equipment for their DGR fork and asked for a
reusable coding guide. It lives at `docs/guns-and-equipment-guide.md` (repo root `docs/`,
created 2026-06-18, untracked at creation). It is reference-backed with file:line citations
to `Gun.cs`, `Grenade.cs`, `Sword.cs`, `AmmoType.cs`, `Equipment.cs`, `Helmet.cs`,
`EditorGroupAttribute.cs`, and `DuckGame.csproj`.

Key gotchas captured there and relevant to future work: (1) `DuckGame.csproj` is old-style
non-SDK, so every new `.cs` needs an explicit `<Compile Include>`; (2) AmmoType/Thing/
NetMessage network indices are assigned by discovery order, so all multiplayer participants
must run the byte-identical build. See [[private-multiplayer-testing]].
