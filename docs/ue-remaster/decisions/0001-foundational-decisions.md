# Decision Record 0001 — Foundational Direction & Pillars

**Date:** 2026-06-23 (Session 1, Turn 3)
**Status:** Accepted (preplanning — revisable, but these are the working assumptions all
downstream planning builds on)
**Context:** After the multi-agent codebase analysis ([01](../01-codebase-analysis.md)) and the
strategy evaluation ([02](../02-vision-and-strategy.md)), the user confirmed the reframed vision and
answered the four parked questions. This record locks the foundational direction so we don't
re-litigate it after the conversation is compacted.

---

## D1 — Overall direction: **Position B, the "Idiomatic Remaster"**

**Decision:** Build the remaster on a **genuine Unreal Engine 5 substrate** — `AActor`-based
entities, real UE subsystems — rather than hosting a custom 2D engine inside UE ("engine-on-engine")
or reimagining the game from scratch.

**Rationale:** The user's goal is *effortless future expansion* (new modes, perspectives, eventually
3D/VR), with bounded/intentional behavior drift explicitly acceptable. Genuine UE adoption is the
only direction that serves that goal. The codebase is treated as a **behavioral spec** (mine for
intent), not an architectural blueprint (don't copy structure).

**Consequences accepted:** Bounded, intentional feel drift; a netcode rewrite; open-ended (but
charter-bounded) feel-tuning; routine 2D-on-3D engineering friction.

**Alternatives rejected:** *Position A (Faithful Port / engine-on-engine)* — would lock the project
into 2D forever and contradicts the vision. *Position C (Ground-up Reimagining)* — highest risk of
losing the game's soul.

**Guardrail:** A **Feel Charter** (see [02 §5](../02-vision-and-strategy.md)) — a short, testable
list of non-negotiable feel invariants — is the contract that prevents "generic UE floatiness." It
must be authored with the user (who holds authority over the game's soul) and playtested every
milestone.

---

## D2 — Dimensionality: **leave the door open** (no 3D work now)

**Decision:** Architect so that 3D/VR/alternate perspectives are *possible later*, but do **no**
3D-specific work now. Concretely: gameplay runs in a **2D-constrained** world; props use
**2D-constrained Chaos** (one translation axis + two rotation axes locked); the **sim/view
decoupling** (D4) keeps the rendering perspective swappable.

**Rationale:** User answered "more of a 'leave the door open' scenario." The 2D-constrained-Chaos +
view-abstraction approach keeps the third axis dormant-but-present at near-zero present cost.

**Consequences accepted:** Minor ongoing discipline to not hard-code 2D assumptions where cheap to
avoid (e.g., keep transforms full-3D under the hood, constrain via the physics/movement layer rather
than by collapsing to 2D data structures).

**Alternatives rejected:** *Committing to 3D now* (over-engineering for an uncommitted feature);
*ignoring 3D entirely* (would reintroduce the 2D one-way-door problem).

---

## D3 — Networking: **server-authoritative, with held items via attachment to a client-predicted holder**

**Decision:** Use **UE-native server-authoritative replication**. Target **both** local couch co-op
*and* online. Solve the "responsive held objects for non-host players" concern not by hacking the
transport layer, but by the idiomatic UE pattern:

- **While held:** the item is **kinematically attached** to the holder's hand socket. The holder's
  pawn is a **client-predicted autonomous proxy**, so the held item rides the locally-predicted pawn
  → **zero perceived latency for the holder.** Remote clients see it attached to that pawn and
  interpolate.
- **Grab:** client sends a request RPC → server validates (proximity / LOS / not already held) →
  server attaches the item and assigns net ownership (`SetOwner`) to that client → replicated to all.
  Client may predict the grab for immediacy and reconcile on denial.
- **Throw / drop:** client predicts the throw locally (immediate visual) → throw RPC with
  direction/force → server authoritatively activates the item as a 2D-constrained **Chaos** body with
  that impulse → replicated; remotes interpolate.
- DGR's contested-authority **"fondle"** model is **re-expressed as server-mediated
  ownership + attachment handoff**, not a custom NetDriver authority transfer.

**Rationale:** Server-authoritative is more robust, more extensible (new modes, spectators, rejoin,
anti-cheat), and far better documented. The attachment-to-predicted-pawn pattern delivers the user's
responsiveness requirement *within* UE's model — so the user's instinct is achievable and is **not**
wishful thinking, but the right mechanism is ownership+attachment+prediction, which UE is built for.

**Consequences accepted:** Multiplayer *feel* under latency differs from the original P2P (server
arbitrates); contested same-frame grabs resolve server-side (loser sees a rare small correction);
**no built-in host migration** for UE listen-servers — we either adopt a listen/dedicated-server
model and drop seamless migration, or invest later in a custom migration layer. Held items are
kinematic while held (no independent physics reaction mid-hold) — which matches Duck Game's actual
behavior, so this is faithful, not a regression.

**Alternatives rejected:** *Re-hosting DGR's P2P contested-authority netcode* (preserves exact feel
but fights UE and blocks extensibility); *extending the NetDriver for custom per-object authority
transfer* (unnecessary complexity — attachment+prediction already solves the felt problem).

**Open sub-question (parked):** listen-server vs dedicated-server as the primary online model, and
whether any host-migration investment is warranted. Decide before Phase 5.

---

## D4 — Art direction: **visual overhaul first, Classic (pixel) mode later — via sim/view decoupling**

**Decision:** Ship a **modern visual overhaul** as the first art target, and design for a later
**"Classic Mode"** that restores the original pixel-art presentation. Enable both with a
**decoupled architecture**: gameplay `AActor`s own simulation/collision/state and are agnostic about
rendering; a **swappable view/representation layer** draws them. Two implementations: **HD** (new
art) and **Classic** (original pixel atlas, which already exists). The view abstraction is built in
**from Phase 1**; Classic Mode itself ships later (Phase 6).

**Rationale:** Decoupling guarantees gameplay parity across visual modes (identical sim), makes
Classic Mode cheap (pixel assets already exist in `spriteatlas.png` + offsets), and reuses the *same
seam* where future 3D/alternate-perspective views would live (ties to D2). Cheap to build early,
very expensive to retrofit.

**Consequences accepted:** Must maintain (at least) two art sets; the HD set is the real new-art
investment. Slight added indirection between sim and rendering (a feature, not a cost, for this
project).

**Alternatives rejected:** *Hard-coding visuals onto gameplay actors* (would make Classic Mode and
future 3D a rewrite); *pixel-art-only on a modern engine* (forgoes the overhaul the user wants
first); *HD-only* (abandons the community's classic aesthetic).

---

## D5 — Level content: **offline converter preferred (reuse the existing C# loader); manual cleanup acceptable**

**Decision:** Preserve the community's level content by building an **offline batch converter** that
reads the original `.lev` files and emits a UE-native level format (DataAsset / structured data).
**Reuse the existing C# `BinaryClassChunk` / `BitBuffer` loader** to read `.lev` rather than
reimplementing it in C++. Accept that some **manual intermediate cleanup** per level is fine if it
materially reduces effort.

**Rationale:** User: importing original levels would be "amazing," but manual intermediate work is
OK if it significantly cuts development effort. An offline converter that reuses the proven C# reader
is far cheaper and lower-risk than a runtime C++ `.lev` parser, and a one-time conversion fits a
remaster better than perpetual runtime parsing.

**Consequences accepted:** Converted levels are a snapshot; live Steam Workshop `.lev` consumption
at runtime is *not* a day-one feature (could be revisited). Some per-level manual fixups expected
(geometry baking, edge-case objects, art remap to HD/Classic).

**Alternatives rejected:** *Runtime C++ `.lev` parser* (reimplements `BinaryClassChunk` in C++ —
expensive, higher risk); *fresh level set only* (discards 244 shipped levels + Workshop catalog and
the community goodwill the user explicitly values).

---

## Cross-cutting architectural principles established this session

1. **Code = behavioral spec, not architectural blueprint.** (D1)
2. **Split the physics substrate:** kinematic movement component for the *player duck*; 2D-constrained
   Chaos rigid bodies for *props / weapons / ragdolls*. (See [02 §3](../02-vision-and-strategy.md).)
3. **Decouple simulation from visual representation.** (D4) — also the future-3D and Classic-Mode seam.
4. **Feel Charter** as the enforced identity guardrail across all milestones. (D1)
5. **Server-authoritative netcode**, held-object responsiveness via attachment to a client-predicted
   holder. (D3)
