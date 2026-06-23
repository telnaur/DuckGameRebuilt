# UE Remaster — Vision & Strategy Evaluation

*Professional evaluation of the reframed vision (Session 1, Turn 2). This doc is allowed to
disagree with the codebase analysis's Turn-1 recommendation, because the governing constraint
changed.*

---

## 0. What changed, and why it changes the recommendation

My Turn-1 advice ("engine-on-engine, keep physics off Chaos, re-host the netcode") was **correct
for the constraint it was given**: *preserve the game's feel and behavior as faithfully as
possible.* Under that constraint, fighting Chaos and UE replication is the right call.

The reframing **relaxes that constraint deliberately**: bounded, intentional changes to behavior
and core mechanics are acceptable — even *preferred* — when they let the game be **genuinely built
on Unreal base classes**, in service of **future extensibility** (3D, VR, new perspectives, new
multiplayer modes).

That is not a small tweak to the brief. It inverts the optimization target:

| | Turn-1 brief | Turn-2 reframed brief |
|---|---|---|
| Optimize for | Fidelity of feel | Future extensibility & modernization |
| Acceptable behavior drift | ~None | Bounded, intentional |
| Substrate | Custom sim, UE as shell | Genuine UE actors + Chaos |
| 2D→3D/VR later | Effectively impossible | A primary goal |

**Verdict:** Under the reframed brief, **genuine UE adoption is the better-justified path.** The
single most decisive reason is below.

---

## 1. The argument that settles it: dimensionality is a one-way door

An engine-on-engine port (a custom 2D AABB sim hosted in UE) is **permanently 2D**. The simulation
*is* a 2D engine; there is no "third axis" to unlock. Adding 3D/VR later would mean writing a second
engine and re-porting all gameplay onto it — i.e., doing the project twice.

Building on UE actors + Chaos keeps the **third dimension dormant but present**. A 2D-constrained
Chaos game is "3D with one axis locked." Unlocking it later is a *constraint change and a camera
change*, not an engine rewrite. VR, alternate perspectives, 2.5D depth layers, full 3D modes — all
of these are *additive* on a UE-native substrate and *impossible* on a custom 2D substrate without
starting over.

If 3D/VR/new-perspectives are genuinely on the roadmap (even "someday"), this alone justifies
genuine UE adoption. **This is the strongest point in the user's favor, and I fully endorse it.**

---

## 2. The method is right: code is a *behavioral spec*, not an *architectural blueprint*

The user's instinct to "approach it port-first so the essence isn't lost" is wise — but the way to
honor it is not to copy the code's *structure*. It is to:

1. **Mine the code for intent:** *what* does the game do, and *why does it feel the way it does?*
   (The Turn-1 analysis is exactly this excavation.)
2. **Distil the intent into a spec** — especially the feel-defining behaviors (see §5, Feel Charter).
3. **Re-express that spec in idiomatic UE**, choosing UE's structures freely.

The DGR codebase is the **best existing documentation of Duck Game's behavior**. We read it like a
spec sheet, not like a porting target. This preserves essence *without* importing the decompiled
methodology that would handcuff us in UE. **Affirmed and adopted as the guiding principle.**

---

## 3. Where I push back: "genuinely use UE" ≠ "Chaos for everything"

This is the most important refinement in this document. The user said they're "very favorable of
building custom classes off `AActor` and the Chaos engine." Strongly agree on `AActor`. On Chaos,
the nuance matters a great deal:

**The idiomatic, professional UE architecture is a *split*, not "everything is a rigid body":**

- **The player duck → a custom kinematic movement component** (think `CharacterMovementComponent`,
  or the newer **Mover 2.0** plugin, or a bespoke `UMovementComponent`). The duck is *driven*, not
  *simulated*: instant air-direction changes, custom friction, precise jump arcs, deliberate
  "un-physical" control. **UE itself ships a kinematic character controller precisely because
  making the player a raw rigid body feels terrible in every genre.** This is not us inventing a
  weird custom base class to dodge UE — it *is* the UE-sanctioned pattern.
- **Props / weapons / crates / ragdolls → Chaos rigid bodies**, constrained to the 2D plane
  (lock one translation axis + two rotation axes). This is where Chaos *shines* and where Duck
  Game's "physics comedy" actually lives: thrown swords, bouncing grenades, flopping ragdolls,
  tumbling crates.

**Why this is also more faithful to the original design intent:** In DGR, ducks and items happen to
share one custom engine — but *in spirit* they were never the same thing. The duck is a
tightly-authored avatar; the items and ragdolls are the chaos. DGR blurred this only because it had
a single homemade physics system. UE lets us express the distinction that was always implicitly
there. So the split is simultaneously **more idiomatic UE** *and* **more honest to Duck Game**.

**Critical correction to a likely misconception:** A custom *kinematic movement component* for the
duck does **not** handcuff future 3D/VR. Movement components are dimension-agnostic — UE's own
character movement works in full 3D. The thing that would handcuff the future is a custom *2D
collision/physics engine*, which is exactly what we're avoiding. So the user's fear ("custom base
classes will handcuff future features") is correctly aimed at a *custom 2D sim* — and a kinematic
duck controller is not that.

> **Net:** Use `AActor` everywhere. Use **Chaos for dynamic props/ragdolls** (2D-constrained). Use a
> **custom kinematic movement component for the player duck**. This is the genuine-UE architecture.

---

## 4. The real risks of the genuine-UE path (so they're chosen with eyes open)

Going genuinely UE-native is the right call under the reframed brief, but it is **not free**. The
risks move from "fighting the engine" to "recapturing the feel and rebuilding netcode."

### 4.1 "Generic UE floatiness" — the identity risk
The biggest danger is *not* technical failure; it's **mushy success**: the remaster works, runs,
looks modern — and feels like every other UE physics platformer instead of like Duck Game.
Chaos's default behavior is *realistic* momentum/restitution/friction, which is **not** how Duck
Game moves. Without active guarding, behavior drifts toward "generic," and the crisp, snappy,
slightly-cartoon-physics identity erodes by a thousand small defaults. **This is the risk to
manage above all others.** Mitigation: the Feel Charter (§5) + early, obsessive feel playtesting.

### 4.2 Netcode becomes a real rewrite (and changes multiplayer feel)
Genuine UE → **server-authoritative replication** (clients no longer "own" arbitrary objects). This
is *more robust*, *easier to extend* (new modes, spectators, dedicated servers, rejoin), and
*better documented* than DGR's contested-authority P2P. **But** it changes the feel of held/thrown
objects under latency (the server arbitrates; clients predict/interpolate), and UE listen-servers
have **no built-in host migration** — DGR has it. Consequences to accept or design around:
- Either lean into **dedicated/listen-server** model (modern, robust) and drop seamless P2P host
  migration, **or** invest in a custom migration layer (expensive).
- Held/thrown-object latency feel will differ from the original. For a *remaster* this is likely
  acceptable and arguably an upgrade in fairness/anti-cheat, but it is a real change.
- Chaos networked physics (Network Physics / resimulation) is powerful but still maturing — budget
  for experimentation, not a turnkey solution.

### 4.3 Feel-tuning is open-ended work
A faithful custom-sim port has a clear "done" (matches the original numbers). A Chaos re-expression
has a *subjective* "done" (feels right). That is harder to schedule and easier to rabbit-hole.
Mitigation: the Feel Charter turns subjective feel into a finite checklist of testable invariants.

### 4.4 2D-on-3D friction (manageable, but real)
2D-constrained Chaos has well-known sharp edges: constraint drift, depth/Z-fighting in rendering
order, contact stability on stacked boxes, tunneling of fast objects (needs CCD), and pixel-perfect
expectations meeting floating-point physics. All solvable and well-trodden (many shipped 2.5D UE
games), but it's engineering, not magic.

---

## 5. The guardrail: a "Feel Charter"

To get the upside of genuine UE without losing the soul, we convert "the essence" from a vibe into a
**short, testable list of non-negotiable feel invariants**. Behavior may drift *anywhere not on this
list*; items on the list are protected and playtested every milestone. Draft seed (to be refined
with the user):

1. **Instant air control / direction reversal** — the duck changes horizontal direction in the air
   with near-zero momentum lag (un-physical on purpose).
2. **The jump arc** — height, hang-time, and fall acceleration match the original's read.
3. **Crisp ground friction / slide** — quick stops, the specific "skid" on landing/turning.
4. **Ragdoll comedy** — death/knockback ragdolls flop readably and hilariously, not limply or stiffly.
5. **Weapon knockback & recoil** — guns shove ducks and objects with the original's punchiness.
6. **Thrown-object chaos** — items tumble and bounce with lively, readable, slightly-exaggerated arcs.
7. **Grab/hold precision** — picking up and aiming items feels immediate and exact, not springy.
8. **Lethal speed / tempo** — rounds stay fast and twitchy; TTK and movement speed preserve the pace.

> Anything not on the charter is **fair game to modernize.** The charter is the contract between
> "remaster" and "Duck Game." It is the single most valuable artifact to get right early.

---

## 6. The strategic spectrum (for the user to choose from)

Three positions on a continuum from "preserve" to "reimagine." This is the decision to mull — not
being forced now.

### Position A — Faithful Port (engine-on-engine)
- **What:** Custom 2D AABB sim ported to C++, UE as renderer/input/audio/Steam shell. Netcode
  re-hosted. (My Turn-1 recommendation.)
- **Unlocks:** Maximum feel fidelity, lowest feel risk, preserves exact multiplayer behavior.
- **Costs:** Permanently 2D. 3D/VR/new-perspective modes ≈ impossible without a second engine.
  Maintains a bespoke engine forever. Least "modern." **Directly contradicts the reframed vision.**

### Position B — Idiomatic Remaster (recommended)  ✅
- **What:** Genuine UE. `AActor`-based things; **kinematic movement component for ducks + Chaos
  rigid bodies (2D-constrained) for props/ragdolls**; server-authoritative replication; UE asset
  pipeline; Feel Charter as the guardrail; `.lev` importer to preserve level content.
- **Unlocks:** 3D/VR/new-perspectives are *additive* later. Native UE tooling, modern netcode,
  Niagara, MetaSounds, Blueprint extensibility for rapid new content/modes. The reframed vision.
- **Costs:** Bounded, intentional feel drift (guarded by the charter). Netcode is a real rewrite and
  multiplayer feel under latency changes. Feel-tuning is open-ended (charter bounds it). Some 2D-on-3D
  engineering friction.

### Position C — Ground-up Reimagining
- **What:** Use DGR purely as inspiration; rebuild freely with no fidelity obligation.
- **Unlocks:** Maximum creative freedom and modernization speed.
- **Costs:** Highest risk of *losing the soul* — without the port-level analysis discipline and a
  charter, it becomes "a game like Duck Game" rather than Duck Game remastered. Most likely to
  squander the existing 240k-line behavioral spec. Probably *not* what the user wants given the
  explicit desire to honor the essence.

**Recommendation:** **Position B**, executed with two disciplines borrowed from A and the analysis
mindset of C: (1) the **Feel Charter** as a hard guardrail, and (2) **the codebase as behavioral
spec**. This delivers genuine UE extensibility (the user's actual goal) while systematically
protecting the identity (the user's stated worry).

---

## 7. Consequences matrix (what each direction buys and costs)

| Dimension | A: Faithful Port | B: Idiomatic Remaster ✅ | C: Reimagining |
|---|---|---|---|
| Feel fidelity | Highest | High *if charter enforced* | Variable / at risk |
| 3D / VR / new perspectives | ❌ Effectively impossible | ✅ Additive later | ✅ Native |
| New multiplayer modes | Hard (bespoke netcode) | ✅ UE-native, extensible | ✅ Native |
| Modding/extensibility | Low (custom engine) | ✅ High (Blueprints/UE) | ✅ High |
| Netcode robustness | Preserves P2P quirks | Server-auth (robust, modern) | Design freely |
| Multiplayer *feel* vs original | Identical | Changed (latency model) | Changed |
| Host migration | Preserved | Lost / custom (cost) | Design choice |
| Feel-tuning effort | Bounded (match numbers) | Open-ended (charter bounds) | Open-ended |
| Risk of losing identity | Lowest | Low *with charter* | Highest |
| Long-term maintenance | Bespoke engine forever | UE-standard | UE-standard |
| Alignment w/ stated vision | ❌ Contradicts | ✅ Matches | ⚠️ Overshoots |

---

## 8. What I'd want decided (parked — not forcing now)

These are the forks that most change the development path. Flagged for the user to mull during
continued discussion:

1. **Dimensionality intent.** Is 3D/VR an actual roadmap item, or "leave the door open"? Even the
   latter justifies Position B; the former makes it mandatory. *(Strongly shapes everything.)*
2. **Multiplayer priority.** Is couch co-op (local split-screen) the first-class target, online
   second? And is server-authoritative netcode acceptable (likely yes for a remaster)? Host
   migration: must-have or droppable?
3. **Feel Charter contents.** Which behaviors are truly non-negotiable? (Draft in §5 — needs the
   user's authority, since they know the game's soul.)
4. **Art direction.** Does "remaster" include a visual overhaul (HD/3D art), or preserve the pixel
   aesthetic on a modern engine? (Affects rendering strategy and asset pipeline enormously, and is
   somewhat independent of the physics-substrate decision.)
5. **Content compatibility.** Do we need to import the 244 shipped `.lev` levels (and the Steam
   Workshop catalog), or is a fresh level set acceptable? (Affects whether we build the `.lev`
   importer / `BinaryClassChunk` reader.)

---

## 9. Current standing recommendation (revisable)

> **Pursue Position B — the Idiomatic Remaster — on a genuine UE5 substrate:** `AActor`-based
> entities, a **custom kinematic movement component for the player duck**, **2D-constrained Chaos
> rigid bodies for props/weapons/ragdolls**, **server-authoritative replication**, the **UE asset
> pipeline** (with a `.lev` importer if content compatibility is wanted), and a **Feel Charter** as
> the enforced guardrail against identity drift. Treat the existing DGR code as the **behavioral
> spec** we mine for intent, not the architecture we copy.
>
> This honors the user's actual goal (effortless future expansion — 3D, VR, new modes) while
> systematically protecting the thing the user rightly fears losing (the essence/feel). The main
> accepted costs are: a netcode rewrite with changed multiplayer-latency feel, open-ended (but
> charter-bounded) feel-tuning, and routine 2D-on-3D engineering.

*This recommendation supersedes the Turn-1 "engine-on-engine" recommendation for as long as the
"extensibility over perfect fidelity" brief holds. If the brief flips back to "fidelity above all,"
Position A returns to the front.*

---

## 10. Resolutions (Turn 3) — the §8 questions are now answered

The user confirmed the vision and resolved the four parked questions. **Position B is accepted.**
Decisions are formally recorded in [decisions/0001-foundational-decisions.md](decisions/0001-foundational-decisions.md);
the build plan is in [03-roadmap.md](03-roadmap.md). Summary of resolutions:

1. **Dimensionality →** "leave the door open." 2D-constrained Chaos + sim/view decoupling keep 3D/VR
   possible at near-zero present cost; no 3D work scheduled. *(D2)*
2. **Multiplayer →** both local and online; **server-authoritative** netcode. Responsive held objects
   solved by **attaching the held item to a client-predicted holder** (zero perceived latency for the
   holder); "fondle" re-expressed as **server-mediated ownership + attachment handoff** — achieves the
   user's wish without hacking the transport layer. *(D3)*
3. **Art →** modern **visual overhaul first**, **Classic (pixel) Mode** later, both driven by one
   simulation via **sim/view decoupling**. Pixel atlas already exists → Classic is cheap; HD is the
   real new-art investment. *(D4)*
4. **Content →** **offline `.lev` converter reusing the existing C# loader**; manual per-level cleanup
   acceptable. Preserves the 244 shipped levels + Workshop catalog without a runtime C++ parser. *(D5)*

**Two architectural unlocks added this turn:**
- **Held-item responsiveness is not wishful thinking** — attachment-to-predicted-pawn is the idiomatic
  UE solution and is *simpler* than custom authority transfer.
- **Classic Mode + future 3D share one seam** — decoupling simulation from visual representation pays
  for both. Build the abstraction in Phase 1; it is cheap early and expensive to retrofit.
