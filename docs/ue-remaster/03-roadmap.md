# UE Remaster — Suggested Roadmap

*Derived from the codebase analysis ([01](01-codebase-analysis.md)), the strategy evaluation
([02](02-vision-and-strategy.md)), and the foundational decisions
([decisions/0001](decisions/0001-foundational-decisions.md)). This is the **Position B — Idiomatic
Remaster** roadmap. It supersedes the Turn-1 "engine-on-engine" roadmap in
[01 §5](01-codebase-analysis.md).*

> **Not an execution commitment.** This is the planning target. No phase starts until the user calls
> for an execute phase. Phases are sequenced by *de-risking value*, not just dependency order.

---

## Guiding principles (the spine of every phase)

1. **Code = behavioral spec.** Mine DGR for *what it does and why it feels right*; choose UE
   structures freely.
2. **Split substrate:** kinematic **movement component** for the duck; **2D-constrained Chaos** for
   props/weapons/ragdolls.
3. **Decouple sim from view:** gameplay actors are render-agnostic; a swappable view layer draws them
   (enables HD + Classic + future 3D).
4. **Server-authoritative netcode:** held items ride a client-predicted holder for zero-latency feel.
5. **Feel Charter is law:** every milestone is playtested against the non-negotiable feel invariants.
6. **Leave the 3D door open:** keep transforms full-3D under the hood; constrain via physics/movement,
   not by collapsing data to 2D.

---

## Phase 0 — Foundations & Feel Charter *(de-risk the core bet)*

**Goal:** Prove the substrate *split* feels right, and turn "the essence" into a testable contract.

- **Author the Feel Charter** with the user — the non-negotiable feel invariants (draft seed in
  [02 §5](02-vision-and-strategy.md)). This is the highest-value early artifact.
- UE5 project + module structure + source control + coding standards.
- Decide the **sim/view abstraction** shape (interface between gameplay actor and its renderer).
- **Spike:** a kinematic duck capsule + a handful of 2D-constrained Chaos crates, placeholder boxes,
  in a single test room. Tune against Feel Charter items **1–3** (air control / jump arc / friction).
- **Exit criteria:** "a placeholder duck moving among placeholder crates *already feels like Duck
  Game*." If it doesn't, stop and reconsider before building breadth.

**Risk addressed:** the #1 risk (generic floatiness) is confronted *first*, on placeholders, before
any sunk cost.

---

## Phase 1 — Core entity & movement vertical slice

**Goal:** The real entity architecture and a duck that moves correctly, drawn through the view layer.

- **Base actor architecture** — the decomposed `AActor` answer to DGR's `Thing` (transform/state on
  the actor; gameplay logic in components; *not* the 130-field god-class). Lifecycle mapped to
  `BeginPlay`/`Tick`/`EndPlay`.
- **`UDuckMovementComponent`** — custom kinematic controller porting the *intent* of
  `PhysicsObject.UpdatePhysics()` (air control, jump arc, friction regimes, ground/slide), expressed
  kinematically, not as a rigid body.
- **2D-constrained Chaos** for one prop type + a **ragdoll spike**.
- **Camera** — 2D ortho, auto-zoom-to-fit-players (port the math from DGR's camera).
- **View abstraction in place** — duck rendered via placeholder *through the swappable view layer*
  (even if it's just a capsule mesh for now).
- **Fixed-step driver** — decide tick model (UE tick vs. a fixed-accumulator subsystem) to honor the
  60/61 Hz heritage where it matters for feel.
- **Exit criteria:** single-player duck satisfies Feel Charter **1–4**; view layer demonstrably
  swappable (swap placeholder A ↔ B with no sim change).

---

## Phase 2 — Combat & interaction loop

**Goal:** The core verbs — grab, hold, shoot, throw, die — feel right single-player.

- **Grab/hold** — kinematic attach to hand socket (the local half of the D3 netcode design; networking
  comes in Phase 5).
- **1–2 weapons end-to-end** — a gun (bullets via trace, porting the subdivide/raymarch *intent*) and
  a thrown melee (Chaos body on release).
- **Knockback, hurt, ragdoll death.**
- **SFX** via MetaSounds (core hit/shoot/jump sounds).
- **Exit criteria:** Feel Charter **5–8** (knockback punch, thrown-object chaos, grab precision,
  tempo) hold single-player.

---

## Phase 3 — Content pipeline & level import

**Goal:** Play real Duck Game levels.

- **Offline `.lev` → UE converter** — reuse the existing C# `BinaryClassChunk`/`BitBuffer` loader to
  read `.lev`; emit a UE-native level format (DataAsset / structured data). (Per
  [D5](decisions/0001-foundational-decisions.md).)
- **Block geometry → baked merged collision** (reuse DGR's BlockGroup/auto-tile *logic* as the recipe;
  bake to merged static mesh + single collision body at load).
- Import a handful of real levels; play them with the Phase 1–2 duck.
- **Asset pipeline groundwork** — import path for HD art; register the original pixel atlas for the
  future Classic view.
- **Exit criteria:** several shipped levels playable; converter handles the common object set with
  bounded manual cleanup.

---

## Phase 4 — Local multiplayer (couch co-op) *(first "fun with friends" milestone)*

**Goal:** Local split-screen multiplayer with a real game mode.

- **Enhanced Input** — per-player slot mapping (controller N ⇄ player N), keyboard split into two
  virtual players (custom slot logic), the string-keyed action facade over `UInputAction`s.
- **Split-screen** rendering.
- **Deathmatch (`DM`)** as an `AGameModeBase` subclass — round flow, scoring, win conditions.
- **Spawners** (`ItemSpawner`/`ItemBox`/spawn points) — *preserve the deterministic per-level seed* so
  item spawns are reproducible (matters even more once online).
- **Exit criteria:** 2–4 players, one keyboard + controllers, full DM round on real levels. This is the
  first milestone that is *actually fun*, and the natural first playable to share.

---

## Phase 5 — Online multiplayer

**Goal:** Networked play with responsive held objects.

- **Server-authoritative replication** of ducks, items, projectiles.
- **Held-item responsiveness** via attachment to the client-predicted holder; **grab/throw as
  server-mediated ownership+attachment handoff** (per [D3](decisions/0001-foundational-decisions.md)).
- **OSS Steam** sessions/lobbies/matchmaking.
- **Decide host model** — listen vs dedicated server; host-migration stance (the parked D3
  sub-question).
- **Exit criteria:** online DM that holds up under realistic latency; held objects feel responsive to
  their holder; no desync on seeded spawns.

---

## Phase 6 — Visual overhaul + Classic Mode *(validate the decoupling)*

**Goal:** The modern look, plus the nostalgia toggle.

- **HD art-direction pass** — the main new-art investment (the modern visual overhaul).
- **Classic Mode** — swap the view layer to the original pixel atlas; verify *identical gameplay*
  across modes (proves the D4 decoupling).
- **Exit criteria:** both view modes ship from one simulation; toggling changes only presentation.

---

## Phase 7 — Meta, polish & content breadth

**Goal:** From "a fun slice" to "a game."

- Profiles/saves/unlocks → `USaveGame` + OSS stats/cloud.
- Full weapon roster (the 145-file long tail) + equipment.
- Particles → Niagara (or pooled lightweight instances where gameplay-affecting).
- Remaining modes (`CTF`, perks/party rules), menus/screens, scoreboard.
- More converted levels + cleanup pass on the catalog.

---

## Explicitly deferred / out of scope (for now)

- **In-game level editor** (DGR's ~14.7k-line editor) — replaced by UE's editor; revisit a
  player-facing editor much later. *(But salvage the serializer for the converter — Phase 3.)*
- **Live Steam Workshop runtime `.lev` consumption** — converter is offline/snapshot first.
- **Actual 3D/VR features** — architecture leaves the door open; no 3D work is scheduled.
- **Host migration** — pending the Phase 5 host-model decision.

---

## Recurring across all phases

- **Feel Charter playtest** at every milestone exit (non-negotiable invariants).
- **Append discoveries/decisions** to [00-conversation-log.md](00-conversation-log.md) and
  `decisions/`.
- **Keep transforms 3D-capable** even while constrained to 2D (the cheap insurance for D2).

---

## One-screen summary

| Phase | Milestone | Proves / de-risks |
|---|---|---|
| 0 | Placeholder duck + crates feel right | The substrate split + Feel Charter (the core bet) |
| 1 | Entity arch + movement + view layer | Decomposed actor, kinematic duck, swappable view |
| 2 | Grab/shoot/throw/die | The core verbs feel right |
| 3 | Real levels playable | `.lev` converter + baked geometry |
| 4 | Couch co-op DM | First *fun* playable; input/split-screen/modes |
| 5 | Online DM | Server-auth netcode + responsive held items |
| 6 | HD look + Classic Mode | The sim/view decoupling pays off |
| 7 | Meta + breadth | Slice → shippable game |

**Critical path & top risks:** Phase 0 (feel) and Phase 5 (netcode) are the two make-or-break
milestones. Everything else is execution. The recommended proving sequence is Phases 0→4 (a
local-multiplayer vertical slice) *before* committing to the Phase 5 networking build.
