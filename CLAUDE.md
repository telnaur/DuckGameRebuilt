# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Duck Game Rebuilt (DGR) is a decompilation of the game *Duck Game* with heavy performance, compatibility, and quality-of-life modifications. It is a C# game targeting **.NET Framework 4.8** (`LangVersion` 9.0), built on **FNA** (an XNA reimplementation). Because it is decompiled, the code is messy by design — expect 200+ build warnings; that is normal and not something to "fix."

## Prerequisites & setup

- Uses git submodules. After cloning, run `git submodule update --init --recursive` (the `FNA` submodule under `FNA/` will be empty otherwise).
- **Steam must be running before the game launches**, or it crashes on startup.
- Windows: Visual Studio 2022 with the ".NET desktop development" workload (MSBuild, NuGet, .NET Framework 4.8 targeting pack).
- Linux: `mono-complete` and the `nuget` CLI.

## Build & run

There is no test suite in this repo — verification is done by building and running the game.

**Windows (Visual Studio):** open `DuckGame.sln`, set **DuckGame** as the startup project (not CrashWindow, FNA, Rebuilder, or Steam), restore NuGet, build with `Ctrl+Shift+B`, run with `F5`.

**Linux (mono):**
```bash
nuget restore                  # if the IDE hasn't
mkdir ./bin/
cp ./DuckGame/lib/* ./bin/     # copy DLL deps into the output dir
msbuild -m -p:Configuration=Debug
mono ./bin/DuckGame.exe
```

Build output goes to `./bin/` (shared output path for all configs). On Linux, runtime deps also come from `./deps/` (native FNA3D/FAudio/SDL libs, Steamworks, Harmony, MonoMod, etc.).

### Configurations

- `Debug` — `DEBUG;TRACE` defined; `Program.IS_DEV_BUILD` is `true`, enabling dev-only features.
- `Release` — normal end-user build.
- `ReleaseAutoUpdater` — Release plus the in-app auto-updater; this is what the GitHub release workflow ships as the standalone download.
- `pHost` — profiling/host variant.

CI (`.github/workflows/autobuild_for_release.yml`) builds on a published GitHub release: `ReleaseAutoUpdater` is zipped as the standalone download, and `Release` is packaged into the `Rebuilder/` directory and pushed to the Steam Workshop.

## Solution layout (4 projects)

- **DuckGame** (`DuckGame/DuckGame.csproj`) — the game itself (`WinExe`). The only project you normally run. Startup object is `DGWindows.WindowsPlatformStartup`; `Program`/`Main` (`DuckGame/src/`) bootstrap the engine.
- **Steam** (`Steam/`) — Steamworks integration, built as a `Library`.
- **CrashWindow** (`CrashWindow/`) — standalone crash-reporter UI (`WinExe`) shown when the game crashes.
- **Rebuilder** (`Rebuilder/build/Rebuilder.csproj`) — produces `Rebuilder.dll`, the small loader mod that lets DGR run as a Steam Workshop mod of the original Duck Game. `Rebuilder/mod.conf` is the Workshop mod manifest. (`Rebuilder.sln` and `Steam.sln` are separate solutions; `DuckGame.sln` is the main one.)

## Code architecture

All game code lives under `DuckGame/`. The bulk is in `DuckGame/src/DuckGame/` (~2000 `.cs` files). Most game logic sits in the global `DuckGame` namespace.

Key engine concepts:

- **`Thing`** (`src/DuckGame/Thing.cs`) — abstract base for everything in the world. Things are added via `Level.Add` and are then drawn/updated automatically each frame. `MaterialThing` extends it for physical objects. Things carry extensive networking state (`_ghostType`, `_authority`, `NetIndex` fields) since multiplayer sync is pervasive — touching a `Thing` subclass usually means thinking about network replication.
- **`MonoMain` / `Main`** (`src/Main.cs`) — the game loop / match-state owner (e.g. `ResetMatchStuff`).
- **`Level`** subsystem (`src/DuckGame/Levels/`) — world/level container and editor.
- **`Network`** (`src/DuckGame/Network/`) — multiplayer; ghosts, authority, replication.

Feature areas are grouped into folders under `src/DuckGame/`: `Weapons/`, `Equipment/`, `Particles/`, `Profile/`, `Rules/` (game modes), `Tiles/`, `Spawners/`, `Events/`, `Highlights/`, `Scripting/`, etc.

`src/MonoTime/`, `src/SystemDrawing/`, `src/XnaToFna/` are compatibility/shim layers bridging the original XNA code onto FNA.

### Added content & dev tooling (`DuckGame/AddedContent/`)

This is where DGR's own additions live (as opposed to decompiled original code). Notable subsystems:

- **Firebreak** (`AddedContent/Firebreak/`) — DGR's developer framework:
  - **DuckShell** — an in-game developer console ("MallardManager"). Commands are `static` methods on the `partial class Commands` (namespace `DuckGame.ConsoleEngine`), one file per command under `Firebreak/DuckShell/Console/Commands/`, each annotated with `[Marker.DevConsoleCommand(Description = ..., To = ImplementTo.DuckShell)]`. To add a command, add a new file following that pattern — registration is attribute-driven.
  - **AutoConfig** — attribute/reflection-driven config system with its own serializer (`FireSerializer` + pluggable `IFireSerializerModule` modules).
- **Recorderator** (`DuckGame/Recorderator/`) — gameplay recording/replay system.

### Patching libraries

The game uses **Harmony** (`0Harmony.dll`) and **MonoMod** / **Mono.Cecil** for runtime IL patching (present in `deps/`). Expect `[HarmonyPatch]`-style patches when behavior is injected into decompiled methods.

## Assets

- `spriteatlas.png` + `spriteatlas_offsets.txt` at the repo root are the packed sprite atlas.
- `shaders_source/` holds shader sources.
- Versioning: `Program.CURRENT_VERSION_ID` (`src/Program.cs`) is the canonical game version string.

## Conventions

- `.editorconfig` is authoritative: 4-space indent, CRLF, no final newline, no `this.` qualification, `var` discouraged for built-in types. Follow the surrounding (often messy) decompiled style rather than aggressively reformatting.
- Branch naming: `fix/<topic>`, `feat/<topic>`, `docs/<topic>`. Commit subjects prefixed `Fix:` / `Feat:` / `Docs:`. PRs target `master`.
