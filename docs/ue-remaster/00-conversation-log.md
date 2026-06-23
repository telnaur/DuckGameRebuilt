# UE Remaster — Conversation Log

> Append-only, verbose log of the preplanning discussion. Newest entries at the bottom.
> Captures ideas, reframings, findings, and the reasoning behind them — even the discarded ones.

---

## Session 1 — 2026-06-23

### Turn 1 — "Explore porting the core game to Unreal Engine"

**User intent:** Explore the (self-described "crazy") possibility of porting the core game —
*DuckGame* / *Duck Game Rebuilt (DGR)* — to Unreal Engine. Explicitly **not** interested in the
mods. Wants a realistic deep dive on the game code and how it maps to Unreal, and what a port
would take. Asked to leverage multiple agents to mitigate context bloat and search deeply.

**Method:** Fanned out six parallel investigation agents, one per subsystem:
1. Core object model & game loop
2. Rendering / graphics pipeline
3. Physics, collision, movement
4. Networking / replication
5. Input, audio, profiles, content pipeline
6. Levels, tiles, editor, game modes, scale metrics

**Key findings (full detail in [01-codebase-analysis.md](01-codebase-analysis.md)):**

- **Object model:** `Transform → Thing → MaterialThing → PhysicsObject → Holdable → Gun → concrete`.
  ~6 levels deep, 191 direct `:Thing` files, 400–600+ concrete types. Clean
  `Do*`-wrapper + overridable-hook lifecycle that maps 1:1 to UE `BeginPlay`/`Tick`/`EndPlay`.
  `Thing` is a 130-field god-class fusing transform + gameplay + networking + serialization + editor.
- **Game loop:** fixed-timestep ~60/61 Hz, update separated from draw, sub-tick render
  interpolation (`IntraTick` + `frameFlipFlop`). Update/draw *order* is non-deterministic
  (`HashSet` iteration); the game embraces this and reconciles via networked authority.
- **Physics:** fully custom AABB-only, axis-aligned, discrete-but-swept 2D engine. No Box2D/Chaos.
  Rotation is cosmetic — collision never rotated. The entire feel lives in
  `PhysicsObject.UpdatePhysics()` with many load-bearing magic constants. Bullets = hitscan via
  recursive subdivision + 1px raymarch. Grab/hold = kinematic position-parenting.
- **Rendering:** hand-rolled 2D sprite batcher on FNA (`MTSpriteBatch`/`MTSpriteBatcher`),
  prebuilt sprite atlas, fixed ordered layer set, `Depth` struct sort, ~80 SM2 `.fx` shaders,
  bitmap fonts, particles-as-Things. Total rewrite for UE.
- **Networking:** the #1 risk. Peer-to-peer over Steam, **contested per-object authority**
  ("fondle" — any peer can seize simulation ownership of any object), per-player-partitioned
  `NetIndex16` IDs, reflection-driven `StateBinding` delta bit-packing, peer hosting + host
  migration. **Architecturally opposed to UE's server-authoritative replication.**
- **Input/audio/content:** custom string-keyed input with local-multiplayer slot mapping
  (controller N ⇄ player N, keyboard split into two virtual devices); dual-stack audio
  (FAudio + NAudio/NVorbis); **loose-file runtime content loading** (opposite of UE's
  import-and-cook). `Profile` is a god-object (save + net slot + persona).
- **Levels:** runtime-reconstructed object lists (not UE maps), custom `.lev` binary chunk
  format (244 shipped levels + Workshop), geometry = 16px `Block` actors with runtime auto-tiling.
  Editor ~14.7k lines (out of scope) but its serializer (~2k lines) is load-bearing.
- **Scale:** ~1,750 files / ~240k lines, but a large fraction is FNA shim (disappears) +
  editor UI (out of scope). True surface ≈ 60k gameplay + 15–20k rendering + 19k networking +
  2k serializer salvage.

**Initial recommendation given (Turn 1):** "Engine-on-engine" — keep DGR's sim as a custom C++
module inside UE, UE as renderer/input/audio/asset/Steam shell, **keep physics off Chaos**,
re-host the bespoke netcode rather than fighting UE replication. This recommendation was
explicitly conditioned on *preserving game feel and behavior faithfully*.

### Turn 2 — Reframing: this is a REMASTER, not a 1:1 port

**User intent (verbatim essence, paraphrased for the log):**

- Does **not** want a 1:1 replica. This is a **remaster port**.
- **Minor changes to behavior and the core are acceptable — and may even be preferred** if they
  let the game systems be fundamentally built off **Unreal Engine base classes**.
- Goal: a much greater degree of **extendibility and modernization** as development proceeds.
- Reason for approaching it "port-first": much of the game's **essence and character** is a
  product of the *methodology conventionalized in the code*, and the user wants that essence
  genuinely evaluated and considered as it's adapted to UE structure — not lost.
- Strongly favors **building custom classes based off `AActor` and the Chaos engine**, leveraging
  those systems to **replicate current behavior as closely as possible while genuinely using UE
  base classes** — because that enables future reimagination: **3D, VR, new perspectives, new
  multiplayer modes**, and other things UE's native feature set makes possible.
- Does **not** want an "unhealthy pursuit of perfect replication" that handcuffs future features.
  The whole point of UE is **effortless expansion of the original vision**.
- Wants my **professional opinion** — evaluate, expand on, or **push back** as warranted.
  User makes final decisions; my job is to communicate consequences/costs of each direction and
  where development would go depending on the choice.
- Asked to keep documenting in this dedicated "UE Remaster" preplanning folder.
- Explicitly: **not entering an execute phase yet** — still in discussion/exploration.

**My response / professional evaluation (full detail in
[02-vision-and-strategy.md](02-vision-and-strategy.md)):**

Headline: the reframing *changes my recommendation*. My Turn-1 "keep off Chaos / engine-on-engine"
advice was conditional on "preserve feel exactly." By relaxing that to "bounded, intentional feel
drift in exchange for genuine UE extensibility," the user has changed the governing constraint, and
genuine UE adoption becomes the better-justified path — **especially** because the 3D/VR/new-modes
goal is *fatal* to an engine-on-engine 2D sim (it would box the project into 2D permanently).

Where I validated the user's instinct:
- The 3D/VR/extensibility argument decisively favors genuine UE adoption.
- "Code as behavioral spec, not architectural blueprint" is the right method; affirmed.
- Remaster framing correctly frees us from the determinism/feel-perfection straitjacket.

Where I pushed back / refined:
- **"Use Chaos for everything" is the wrong reading of "use UE genuinely."** The idiomatic UE
  architecture is a **split**: the *player duck* should be a **custom kinematic movement component**
  (UE ships `CharacterMovementComponent`/Mover precisely because rigid-body players feel bad),
  while *props / weapons / ragdolls* are **Chaos rigid bodies** constrained to 2D. This split is
  *more* idiomatic UE **and** closer to Duck Game's actual design intent (the duck was never a
  "physics object" in spirit; the comedy is in the items and ragdolls).
- A kinematic player controller is **not** the kind of "custom base class" the user fears —
  it's orthogonal to dimensionality and does not handcuff 3D/VR.
- Risk to name and guard against: **"generic UE floatiness"** — drifting into the feel of every
  other UE physics game and losing Duck Game's crisp identity. Proposed a **Feel Charter**: a
  short list of non-negotiable feel invariants to protect through the re-expression.
- Going genuine-UE implies **server-authoritative networking**, which is more robust and enables
  new multiplayer modes, but changes held/thrown-object latency feel vs. the original P2P. A
  trade-off to surface, not hide.

Strategic spectrum laid out for the user to choose from (see 02 doc):
1. **Faithful Port** (engine-on-engine) — max feel fidelity, 2D-locked, future-handcuffed.
2. **Idiomatic Remaster** (recommended) — UE actors + split kinematic/Chaos, Feel Charter guardrail,
   server-authoritative net. Unlocks 3D/VR/modes. Accepts bounded intentional feel drift.
3. **Ground-up Reimagining** — DGR as inspiration only. Max freedom, hardest to recapture "soul."

**My recommendation:** Position 2, leaning genuinely into UE, with the Feel Charter as the guardrail.

**Open questions parked for the user to mull (not yet decided):**
- Target dimensionality: is 3D/VR an actual roadmap item, or an "leave the door open" nicety?
- Netcode: accept server-authoritative (and possible dedicated-server implications), or is
  preserving couch-co-op-first / P2P feel a hard requirement?
- How much feel drift is acceptable — what goes in the Feel Charter as non-negotiable?
- Does "remaster" include an **art-direction overhaul** (3D/HD assets) or pixel-art preservation?

---

### Turn 3 — User confirms vision; answers the four parked questions; requests roadmap

**User intent:** Agreed entirely with the Turn-2 evaluation ("you have now captured my vision").
Answered the four parked questions, requested a roadmap, and asked that all pertinent information be
written to files because the conversation will be **compacted** next (context window is large).

**User's answers to the four parked questions:**
1. **Dimensionality:** "More of a 'leave the door open' scenario." → no committed 3D, but don't
   preclude it.
2. **Multiplayer:** Both local and online matter. Wondered aloud whether UE's native networking could
   be extended to facilitate custom **authority transfer for held items** to avoid laggy/jittery
   experience for non-host holders — "maybe wishful thinking." Fine with whatever prevents the laggy
   held-object experience.
3. **Art direction:** First implementation = **visual overhaul**; wants a later **Classic Mode**
   restoring traditional pixel art. Asked if that's too ambitious / can be in the plan.
4. **Content compatibility:** Importing original levels would be "amazing," but **manual intermediate
   work is acceptable** if it significantly reduces development effort.

**My response / additions this turn:**

- **Held-item netcode (answer to #2):** *Not* wishful thinking, and the right solution is easier than
  hacking the NetDriver. While held, the item is **kinematically attached** to the holder, whose pawn
  is **client-predicted** → the holder feels **zero latency**. DGR's "fondle" authority transfer is
  re-expressed as **server-mediated ownership + attachment handoff** (grab RPC → server validates +
  `SetOwner`; throw → client predicts, server spawns the Chaos body). Achieves the wish *within* UE.
  Cost: contested same-frame grabs arbitrated server-side (rare small correction); held items are
  kinematic mid-hold (which matches Duck Game — faithful, not a regression). Captured in
  [decisions/0001 §D3](decisions/0001-foundational-decisions.md).
- **Visual overhaul + Classic Mode (answer to #3):** Achievable if designed from day one via
  **decoupling simulation from visual representation** — gameplay actors are render-agnostic; a
  swappable view layer draws them (HD + Classic, and the same seam = future 3D). Classic is cheap
  because the **pixel atlas already exists**; HD is the real new-art cost. Build the abstraction from
  Phase 1; ship Classic Mode in Phase 6. Captured in [decisions/0001 §D4](decisions/0001-foundational-decisions.md).
- **Five foundational decisions recorded** in [decisions/0001-foundational-decisions.md](decisions/0001-foundational-decisions.md):
  D1 Position B (Idiomatic Remaster); D2 leave the 3D door open; D3 server-auth netcode + held-item
  attachment; D4 visual overhaul + Classic Mode via sim/view decoupling; D5 offline level converter
  reusing the C# loader.
- **Roadmap authored** in [03-roadmap.md](03-roadmap.md): Phase 0 Foundations & Feel Charter →
  1 Entity/movement → 2 Combat loop → 3 Content/level import → 4 Local co-op (first fun playable) →
  5 Online → 6 Visual overhaul + Classic Mode → 7 Meta/breadth. Phases 0 (feel) and 5 (netcode) are
  the make-or-break milestones; recommended to prove Phases 0→4 (local-MP vertical slice) before
  committing to the Phase 5 netcode build.

**State at end of Turn 3:** Foundational direction locked (revisably). Vision, strategy, decisions,
and roadmap all written to files in `docs/ue-remaster/` in anticipation of conversation compaction.
Still **no execute phase** — next sessions continue planning (likely: author the Feel Charter; pick
the Phase-0 module/abstraction shapes; resolve the listen-vs-dedicated host-model sub-question).

---

*(Append future turns below.)*
