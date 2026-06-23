# Duck Game Rebuilt → Unreal Engine 5: Port Feasibility Analysis

*Deep-dive assessment of the core game (no mods). Scope: what DGR's architecture is, how each subsystem maps to UE5, the critical risks, and a realistic strategy + roadmap.*

---

## 0. TL;DR verdict

**A port is feasible and the codebase is unusually well-suited for it** — DGR is a clean, fixed-timestep (60/61 Hz), object-oriented 2D engine with a single root entity (`Thing`) and clearly separated subsystems. But it is **not** a "drop it into UE and wire up actors" job. The realistic shape of the work is:

- **Keep & port faithfully (C++):** the fixed-step simulation core — `Thing` lifecycle, the custom AABB physics, gameplay logic. *This is where the game feel lives; it must be preserved line-for-line and kept OFF Chaos.*
- **Rewrite against UE (no incremental path):** rendering, the content/asset pipeline, input, audio.
- **The one genuinely hard architectural decision:** networking. DGR's model (contested per-object "authority", peer hosting, host migration) is **fundamentally opposed** to UE's server-authoritative actor replication. This single subsystem determines the whole project's shape.

Headline recommendation: **"engine-on-engine."** Treat UE5 as a *renderer + input + audio + asset host + window/platform shell*, and run DGR's simulation as a custom deterministic C++ module inside it. Do **not** rebuild the sim on `AActor` replication / `CharacterMovementComponent` / Chaos — that path silently destroys both game feel and the existing netcode.

---

## 1. Scale & shape of the codebase

| Scope | Files (.cs) | Lines | Disposition in a UE port |
|---|---:|---:|---|
| **TOTAL `DuckGame/src/`** | **~1,750** | **~240,700** | — |
| `MonoTime/`, `XnaToFna/`, `SystemDrawing/` shims | (large) | (tens of k) | **Deleted** — UE provides these |
| `Network/` | 229 | 19,100 | **Rewrite or re-host** (the key decision) |
| `Weapons/` | 145 | 19,864 | Mechanical C#→C++ translation (long tail) |
| `Levels/` | 140 | 32,089 | Mixed: ~half is menu/screen UI (out of scope), half is level runtime |
| `LevelEditor/` (`MonoTime/`) | 61 | 14,757 | **~12k editor UI = out of scope**; ~2k serializer = **must salvage** |
| `Profile/` | 15 | 5,859 | Save/unlock data → `USaveGame` + OSS |
| `Tiles/` | 105 | 4,933 | Geometry + decorative; bake to merged mesh |
| `Particles/` | 52 | 3,684 | → Niagara or pooled lightweight instances |
| `Equipment/` | 21 | 3,113 | Mechanical translation |
| `Rules/` (game modes) | 17 | 2,818 | Clean → `AGameModeBase` subclasses |
| `Spawners/` | 16 | 2,333 | Placeable actors (preserve seeded RNG!) |
| `Events/` | 10 | 131 | Trivial event bus |

**Key takeaway:** the headline "240k lines" is misleading. A large fraction is FNA/XNA shim code that *disappears* (UE replaces it) plus editor UI that's out of scope. The true porting surface is roughly: **~60k lines of gameplay/entity/physics to translate**, **~15–20k lines of rendering to rewrite**, **~19k lines of networking to re-architect**, and **~2k lines of serializer to salvage from the editor**.

---

## 2. Subsystem-by-subsystem mapping

### 2.1 Core object model & game loop — *ports cleanly*

- **`Thing`** (`src/DuckGame/Thing.cs`, ~1,920 lines) is the root of everything: `Transform → Thing → MaterialThing → PhysicsObject → Holdable → Gun → <concrete>`. ~6 levels deep, 191 direct `: Thing` files, **400–600+ concrete types** transitively.
- Lifecycle uses a clean **`Do*`-wrapper + overridable-hook** pattern (`DoInitialize`/`Initialize`, `DoUpdate`/`Update`, `DoDraw`/`Draw`, `DoTerminate`/`Terminate`). This is *exactly* UE's `BeginPlay`/`Tick`/`EndPlay` idiom → **maps 1:1**.
- The loop is **fixed-timestep ~60/61 Hz** (`TargetElapsedTime` = 61 UPS in DGR), with **update separated from draw** and **sub-tick render interpolation** (`IntraTick` + `frameFlipFlop` double-buffer). UE doesn't natively separate fixed-update from variable-draw — you implement a **fixed accumulator subsystem** and interpolate at render time (this is what UE's movement smoothing already does internally).

> **Mapping:** `Thing` → `AActor` (decomposed — see warning below). Loop → custom fixed-step subsystem. `Level` + its `QuadTreeObjectList` multi-index container → `UWorld` + a `UWorldSubsystem` holding type indices and the spatial hash. Deferred add/remove (`RefreshState`) → already how UE defers `DestroyActor`.

> ⚠️ **`Thing` is a 130-field god-class** fusing transform + gameplay + networking authority + serialization + editor UI. Do **not** port it 1:1 onto one `AActor`. Decompose: transform/render → components, networking → replication layer, editor metadata → data assets.

### 2.2 Physics, collision, movement — *port faithfully, keep OFF Chaos* ⚠️

This is the **single highest feel-risk area** and the most important thing to get right.

- DGR has a **fully custom, AABB-only, axis-aligned, discrete-but-swept, single-threaded 2D physics engine**. No Box2D, no Farseer. Rotation (`_angle`) is **cosmetic only** — collision is never rotated.
- The entire integrator is `PhysicsObject.UpdatePhysics()` (`PhysicsObject.cs:307–653`): axis-separated sweeps, `ceil(|v|/4)` 4px sub-steps, **three different friction regimes** (grounded/crouch/air-at-0.7×), gravity `0.2/frame`, `vMax`/`hMax` clamps, penetration push-out + `bouncy` reflection with `<0.1` snap-to-zero, one-way platforms (`IPlatform`), and an elaborate sleeping heuristic. **Every magic constant is load-bearing for game feel.**
- Collision math is pure (`Collision.cs` — `Rect`/`Line`/`Point`/`Circle` over `Thing.left/right/top/bottom`). Broadphase = **custom QuadTree (static geometry) + uniform spatial-hash grid (dynamic), filtered by C# Type**.
- Bullets are **hitscan via recursive segment subdivision → 1px raymarch** (`Bullet.cs`), producing pixel-quantized hits and order-dependent penetration — gameplay, not physics.
- Grab/hold is **kinematic position-parenting** (item physics disabled, transform hard-set from the duck's hand each frame), not joints.
- **No `deltaTime` multiply anywhere** — one `Update()` = one fixed physics step. The integer sub-step count + deterministic collision sort order are **relied on by networking**.

> **Mapping:** Reimplement as a bespoke `UDGPhysicsSubsystem` + `UDGPhysicsComponent`, ported **line-for-line** from `UpdatePhysics()`. Custom AABB broadphase (`TMap<int64,TArray<>>` spatial hash), not Chaos overlaps. Bullets → custom subdivide+raymarch trace, not `LineTraceSingleByChannel`. Grab → kinematic transform-set, not physics constraints.

> 🚫 **Do NOT** use `UCharacterMovementComponent`, Chaos rigid bodies, or Chaos substepping. Chaos is continuous, rotational, impulse-based, `dt`-scaled, and non-deterministic across machines — adopting it would (a) silently change air control/friction/bounce/one-way-platforms, and (b) **break multiplayer**, which assumes identical integer-step simulation from identical inputs. The biggest project risk is anyone "simplifying" the physics onto stock UE movement.

### 2.3 Rendering — *full rewrite, no incremental path* ⚠️

The renderer is a hand-rolled 2D sprite batcher built directly on **FNA's `Microsoft.Xna.Framework.Graphics`**. Orthographic, depth-sorted, layered, no 3D. It is the most XNA-coupled subsystem.

- **Sprite path:** `Sprite`/`SpriteMap` → `Graphics.Draw(...)` → `MTSpriteBatch : SpriteBatch` → `MTSpriteBatcher`, which builds `VertexPositionColorTexture` quads and calls FNA `DrawUserIndexedPrimitives`. Manual depth-bucket sort (`Dictionary<float, List<item>>`), 16-bit index cap forces 5,461-quad batch splits.
- **Sprite atlas:** `spriteatlas.png` + `spriteatlas_offsets.txt` (prebuilt offline), swapped in at draw time as a draw-call-merging optimization. At the *gameplay API* level, sprites are still one loose PNG per name.
- **Layers:** a fixed ordered set (`PARALLAX → BACKGROUND → BLOCKS → GAME → GLOW → LIGHTING → FOREGROUND → HUD → CONSOLE`), each its own `MTSpriteBatch` + camera + effect + optional `RenderTarget2D`. Within-layer order = the `Depth` struct (float + auto-increment span), resolved by SpriteBatch sort.
- **Camera:** pure 2D ortho, native virtual resolution **320px wide**, "zoom = change size". Auto-zoom-to-fit-players, split-screen via per-viewport re-draw.
- **Shaders:** ~80 HLSL `.fx` (SM 2.0) compiled offline to XNB `Effect`s, plus ~27 `Material` wrappers (gold, recolor, plasma, glitch…). Fullscreen post via `Level.fullscreenShaders`.
- **Fonts:** bitmap fonts (`BitmapFont`) with inline color/button-glyph tag parsing. **Particles:** ~40 ordinary `Thing`/`Sprite` subclasses drawn through the same path (no GPU particle system), manually pooled.
- **Render targets:** lighting composite (additive), pause capture, recolor-bake-to-target-then-readback, fullscreen-shader chaining.

> **Mapping:** Keep the *abstractions* (`Sprite`, `SpriteMap`, `Layer`, `Camera`, `Depth`, `Material`, `Tex2D`) as the seam, reimplement their bodies on UE. Best fit for the batcher: a **custom RHI/Slate quad batcher** (one draw call per texture/material) to preserve the batched, thousands-of-quads model — Paper2D's per-component sprite overhead won't match it. `Depth`/layers → translucency sort priority or world-Z bands. The ~80 `.fx` shaders → **UE Materials / post-process materials** (mechanical rewrite; SM2 HLSL is trivial for UE). Camera/auto-zoom math ports as-is. Render targets → `UTextureRenderTarget2D`/`SceneCapture`; the recolor-bake becomes unnecessary (do it in a material). Recoloring/atlas/index-tables largely vanish (UE handles batching/streaming).

### 2.4 Networking — *re-architect; the project-defining decision* 🚩

**This is the #1 risk.** DGR's model is architecturally hostile to UE's built-in replication.

- **Model:** host-authoritative-*ish*, **peer-to-peer over Steam P2P datagrams**, with snapshot/state-delta replication and **per-object client-side interpolation** (buffered state timeline, ~360 states, adaptive "constipation" catch-up). **Not** lockstep, **not** rollback, **not** dedicated-server client-server.
- **Ghosts:** a "ghost" (`GhostObject`/`GhostManager`) is the network shadow of a `Thing`. IDs use a **per-player-partitioned wrapping `NetIndex16`** space (`localIndex + profileIndex*2500`) — the *creator* is encoded in the object's network ID, avoiding a central allocator.
- **Authority is contested, not server-fixed** ⚠️: `Thing._authority` is a numeric counter; any peer can **"fondle"** (bump authority +1/+8/+25/…) to seize simulation ownership of *any* object mid-game. Ties resolve deterministically via `(networkIndex + levelIndex) % NumDucks`. **Whoever owns an object simulates it; everyone else interpolates.** On disconnect, the host "super-fondles" the departed peer's objects.
- **Serialization:** reflection-driven `StateBinding` fields → custom bit-packed **delta** encoding (`BitBuffer`) with a per-connection dirty bitmask. Hard cap of **64 replicated fields per Thing** (the `long` mask).
- **Transport:** Steam P2P + a **hand-rolled reliability/ordering/fragmentation layer** (`StreamManager`), with per-message priorities (`ReliableOrdered`/`Urgent`/`Volatile`/`UnreliableUnordered`) and drop-notification-driven re-dirtying.
- **Not deterministic:** owner-simulates + remotes-interpolate. RNG isn't lockstep — the host generates a per-Thing `networkSeed`, replicates it, and clients seed local `Random` for *cosmetic* effects only.
- **Host migration** exists (peer reassignment by `networkIndex`) but is fragile and explicitly disabled in some modes.

> **The mismatch:** UE authority is **binary and server-fixed** (`ROLE_Authority` on the server; clients never own replicated-actor simulation except predicted movement of their own pawn). DGR's "any duck can grab the bazooka and become its simulating authority" is a **distributed-ownership model UE actively forbids**. Likewise, UE listen-servers have **no built-in host migration**.

> **Two real options:**
> - **(A) Re-host DGR's netcode over Steam Sockets, bypassing `UNetDriver` entirely.** Keep the ghost/authority/fondle model and host migration intact; UE only renders the result. *Lower gameplay risk, preserves feel and existing behavior, but you maintain a bespoke net stack and get none of UE's networking tooling.* **Recommended for a faithful port.**
> - **(B) Re-architect to server-authoritative UE replication.** Centralize all authority on the server (fondles become ownership-transfer RPCs), re-author every gameplay class's replicated-field contract, accept dedicated-server or a heavy custom-migration product compromise. *This is essentially a from-scratch netcode rewrite and changes held/thrown-object latency feel.*

> Either way, **transport + matchmaking** (the `StreamManager`/Steam-lobby layer) can be replaced by UE's `OnlineSubsystemSteam` sessions with moderate effort — but the *replication semantics* do not transfer.

### 2.5 Input, profiles, audio, content — *rewrite onto UE subsystems*

- **Input:** custom string-keyed logical triggers (`JUMP`/`SHOOT`/`GRAB`/…) over an `InputDevice` abstraction, with **local multiplayer as a load-bearing requirement** (controller-slot N ⇄ player N; one keyboard split into two virtual P1/P2 devices). Ducks read `inputProfile.Down(Triggers.Shoot)` directly at hundreds of call sites. → **Enhanced Input** (`UInputAction`/`UInputMappingContext`, per-`ULocalPlayer` subsystem). A thin string-keyed facade over Enhanced Input minimizes gameplay churn, but every call site re-points. **Keyboard-split between two local players needs custom slot logic** (Enhanced Input doesn't do it out of the box). Note: `InputProfile` is *also* the network-replicated input carrier (`VirtualInput`, `ushort _state`) and the replay-injection point — untangle local input from networked/replay input first.
- **Profiles:** `Profile` is a **god-object** mixing (a) persistent save, (b) live network slot, (c) cosmetic persona, with a `linkedProfile` indirection between runtime and persistent. Stored as per-profile `.pro` DuckXML in `%AppData%/DuckGame/<SteamID>/`. → split into `USaveGame` (persistent), `APlayerState`-style runtime slot, and a persona data asset; Steam stats/cloud → **Online Subsystem Steam**.
- **Audio:** dual-stack — FNA/FAudio `SoundEffect` + a custom NAudio/NVorbis decoder (streaming, resampling, `.vgz`/`.dgm`/`.ogg`). 32-voice pool + per-frame de-dup. → **MetaSounds/USoundBase** + `USoundConcurrency` (pool/de-dup handled natively). Exotic formats (Ogg/chiptune) need conversion or a custom `USoundWave` decoder.
- **Content pipeline:** **loose files discovered by directory-walk at runtime** (`.png`/`.wav`/`.lev`/`.xnb`/`.ogg`), custom `Content` manager with integer-indexed tables. This is the **opposite** of UE's import-and-cook model. Two strategies: **(1) import everything at editor time** (cleanest, gives streaming/cooking, loses runtime drop-in extensibility), or **(2) runtime PNG/WAV import** (preserves the loose-file/mod model, more work, no cook optimization). The `.lev` format needs a custom importer regardless.

### 2.6 Levels, tiles, game modes — *clean, but format work required*

- **Levels are runtime-reconstructed object lists, not UE maps.** A `Level` (`MonoTime/Level.cs`, 2,444 lines) is a runtime scene rebuilt each round from a flat list of serialized `Thing`s; `GameLevel : XMLLevel : Level` is the in-match type. There is **no tilemap array** — every piece of geometry is a `Thing`.
- **On-disk `.lev` format:** custom **versioned binary chunks** (`BinaryClassChunk`, magic number + checksum + reflection-driven members over a `BitBuffer`, GZip for network transfer). **244 shipped levels** + the entire Steam Workshop catalog use it. → needs a **custom UE importer/`UFactory`** reimplementing `BinaryClassChunk` + `BitBuffer` to preserve compatibility, else re-export levels to a new format.
- **Geometry = 16px `Block`/`AutoBlock` actors** with runtime neighbor-based auto-tiling and runtime-computed concave collision hulls (Hopcroft–Karp matching in `Block`/`BlockGroup`). → either one lightweight actor per block, or (better) **bake grouped blocks into a merged static mesh + single collision body at load** (the existing grouping logic is the blueprint; perf-critical for big levels).
- **Game modes:** small and clean — `GameMode` (1,024 lines) round/match state machine + only **two real competitive modes** (`DM`, `CTF`); variety comes from perks/rules. → straightforward `AGameModeBase` subclasses. Main risk is dependency drag into `Teams`/`Party`/`Scoreboard`/meta.
- **Spawners** (`ItemSpawner`/`ItemBox`/spawn points): placeable actors that spawn payloads. **Critical:** item randomization uses a **deterministic per-level seed** (`Rando.generator = new Random(seed)`) so all networked clients spawn identical items — the UE port must replicate this seeding exactly or multiplayer desyncs.
- **Editor** (`LevelEditor/`, ~14,757 lines, `Editor.cs` = 5,572): a full bespoke immediate-mode UI toolkit. **Out of scope for v1** (replace with UE's editor). **But** ~2k lines of serializer/reflection (`BinaryClassChunk`, `LevelData`, `Editor.GetMembers/CreateThing/GetType`) physically live here and are **load-bearing for level loading** — extract them into a clean serialization module early.

### 2.7 Determinism — the cross-cutting subtlety

- Per-frame **update/draw order is non-deterministic** (the thing list and draw lists are `HashSet<Thing>`; there's even a `RandomizeObjectOrder()`). The game *embraces* this and reconciles via networked authority/ghost state rather than lockstep.
- RNG (`Rando`) is `System.Random` — not portable/reproducible to C++.
- **Implication:** you don't need full determinism *if* you keep the authority/ghost net model (Option A above). If you ever want rollback/lockstep, you must replace every `HashSet` iteration with a stably-ordered container (sort by `globalIndex`/`NetIndex`) and `System.Random` with a seeded `FRandomStream` — a substantial rework. **However**, the physics sub-step *collision resolution* order *is* deterministically sorted today, and spawners *do* rely on seeded RNG — so "deterministic where it matters for sync, non-deterministic for cosmetics" is the existing contract to preserve.

---

## 3. The three decisions that define the project

1. **Networking model (§2.4).** Re-host DGR's bespoke netcode over Steam Sockets (faithful, recommended) **vs.** rewrite onto UE server-authoritative replication (UE-native, bigger, product compromises on held-object latency + host migration). *Everything else flexes around this.*
2. **Physics ownership (§2.2).** Faithful custom C++ sim kept off Chaos (mandatory for feel + sync) **vs.** stock UE movement (do not — it breaks both feel and netcode).
3. **Content strategy (§2.5/§2.6).** Cook-time import (clean, optimized, drops loose-file mod extensibility) **vs.** runtime loose-file loading (preserves the mod/reskin/Workshop model the codebase is built around). A custom `.lev` importer is required either way.

---

## 4. Recommended architecture: "engine-on-engine"

Run DGR's simulation as a **self-contained, headless-capable C++ module ("DuckSim")** hosted inside UE5. UE owns the platform; DuckSim owns the game.

```
┌───────────────────────────────────────────────────────────────┐
│ UE5 host shell                                                  │
│  • Window / platform / GameInstance / fixed-step driver         │
│  • Enhanced Input  → feeds DuckSim input each fixed tick         │
│  • Renderer: custom 2D quad batcher + Materials  ← reads DuckSim │
│  • MetaSounds / USoundConcurrency                ← SFX events    │
│  • Asset host (imported or runtime-loaded content)              │
│  • OnlineSubsystem Steam (sessions/stats/cloud/workshop)        │
├───────────────────────────────────────────────────────────────┤
│ DuckSim (custom C++, ported from DGR)                           │
│  • Fixed 60/61 Hz step, Thing lifecycle, Level/world container  │
│  • Custom AABB physics (UpdatePhysics ported verbatim)          │
│  • Gameplay: Weapons, Equipment, Tiles, Particles, Rules        │
│  • Ghost/authority networking (re-hosted on Steam Sockets)      │
│  • FDGVec2 math shim (mirrors Vec2 API; .x/.y lowercase)        │
└───────────────────────────────────────────────────────────────┘
```

Why this shape:
- **Preserves game feel and the existing netcode** (the two highest-risk areas) by porting them faithfully rather than re-deriving them on UE's opinionated systems.
- **Clean seam for the renderer:** DuckSim produces a list of "draw this sprite at this transform/depth/material" each frame; the UE renderer consumes it. This lets you build & test the sim **headless** first, then attach rendering — major de-risking.
- **`FDGVec2` shim** (mirroring `Vec2`'s lowercase `.x/.y` and helpers) lets the ~240k-line gameplay translation be near-mechanical, converting to `FVector2D` only at the render boundary.

---

## 5. Phased roadmap

**Phase 0 — Spike / de-risk (weeks).** Stand up a UE5 project with a fixed-step subsystem. Port `Vec2`→`FDGVec2`, `Collision.cs`, and a minimal `Thing`/`PhysicsObject` + one `Block`. Get a single duck-sized AABB falling and colliding at 61 Hz, rendered with a placeholder quad. *Goal: prove the fixed-step + custom-physics + quad-render seam.*

**Phase 1 — Headless sim core.** Port `Thing` lifecycle, `Level`/`QuadTreeObjectList`, the full `UpdatePhysics` integrator, broadphase, and the `.lev` loader (salvage `BinaryClassChunk`/`BitBuffer`). Load a real shipped level headless; validate collision/movement against the C# original numerically. *Goal: the sim runs correctly with no renderer.*

**Phase 2 — Rendering.** Build the custom 2D quad batcher, layer/depth system, camera (auto-zoom), sprite atlas import, bitmap fonts. Port the core `.fx` shaders to Materials. *Goal: a level renders and a duck moves on screen, single-player.*

**Phase 3 — Input, audio, one duck playable.** Enhanced Input with local-MP slot mapping + keyboard split; MetaSounds SFX; grab/hold; one or two weapons end-to-end. *Goal: local single-player is fun and feels right.*

**Phase 4 — Gameplay content long tail.** Translate Weapons (145 files), Equipment, Tiles, Particles→Niagara, Spawners (preserve seeded RNG), Rules (`DM`/`CTF` → `AGameModeBase`). *Goal: full local multiplayer feature parity.*

**Phase 5 — Networking.** Re-host the ghost/authority stack on Steam Sockets (Option A) or build the UE-native replication (Option B). Sessions/lobbies via OSS Steam. *Goal: online multiplayer.*

**Phase 6 — Meta & polish.** Profiles→`USaveGame`, unlocks, stats/cloud, Workshop, menus/screens (much of `Levels/` UI), split-screen. *Goal: shippable.*

*(The editor is deliberately deferred indefinitely — replace with UE's editor or a future in-game tool.)*

---

## 6. Effort & risk summary

| Subsystem | Disposition | Effort | Risk |
|---|---|---|---|
| Object model / game loop | Faithful port | Medium | Low |
| Physics / collision / movement | **Faithful port, off Chaos** | Medium | **High (feel)** |
| Rendering | Full rewrite on UE | High | Medium |
| Networking | Re-host or rewrite | High | **Very High** |
| Input | Rewrite (Enhanced Input) | Medium | Medium (local-MP/kbd-split) |
| Audio | Rewrite (MetaSounds) | Low–Medium | Low |
| Content pipeline + `.lev` importer | Rewrite + custom importer | Medium–High | Medium |
| Levels / tiles / geometry | Port + bake-to-mesh | Medium | Medium |
| Game modes / spawners | Clean port | Low–Medium | Low (preserve seed) |
| Profiles / saves / Steam | Rewrite on USaveGame + OSS | Medium | Low |
| Editor | **Out of scope** (salvage serializer) | — (low salvage) | Low |

**Overall:** a realistic, faithful, online-capable port is a **multi-month-to-multi-year effort for a small experienced UE+gameplay team**, with the *gameplay content translation* (400–600 Thing types) as the long tail and *networking* as the top schedule/architecture risk. A **single-player local-multiplayer vertical slice** (Phases 0–4) is a much more tractable, high-confidence milestone and is the right way to prove the concept before committing to the networking rewrite.

---

## 7. The "do not do this" list (feel & sync killers)

1. ❌ Don't rebuild physics on `UCharacterMovementComponent` or Chaos rigid bodies. Port `UpdatePhysics` verbatim.
2. ❌ Don't use Chaos/UE traces for bullets. Port the subdivide+raymarch.
3. ❌ Don't use physics constraints for held items. Kinematic transform-set.
4. ❌ Don't map "fondle"/contested authority onto UE `ROLE_Authority` naively — decide the net model deliberately first.
5. ❌ Don't drop the per-level seeded RNG for spawners — multiplayer desyncs.
6. ❌ Don't author levels as static `.umap`s — they're runtime object lists; spawn from data.
7. ❌ Don't assume "240k lines" of work — a large fraction is FNA shim/editor that disappears or is out of scope.
8. ⚠️ Untangle `InputProfile` (local input) from `VirtualInput` (network/replay input) and split the `Profile` god-object before rebuilding either.

---

*Generated from a parallel multi-agent code investigation of the DGR codebase (core object model, rendering, physics, networking, input/audio/content, levels/modes). All file:line references point at the current `master` checkout.*
