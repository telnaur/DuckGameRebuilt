# Duck Game → Unreal Engine 5 Remaster — Preplanning Documentation

This folder is the working memory for the **UE Remaster** preplanning effort: a deliberate,
discussion-driven exploration of porting / remastering the core of *Duck Game Rebuilt (DGR)*
onto Unreal Engine 5. Nothing here is committed to execution yet — this is the
**think-before-we-build** archive.

> **Status:** Pre-planning. **Direction locked (revisably): Position B — "Idiomatic Remaster"** on a
> genuine UE5 substrate. Foundational decisions and the phased roadmap are recorded. **No execute
> phase has begun** — continued planning next (Feel Charter, Phase-0 shapes, host-model sub-question).

## Purpose of this folder

1. **Track every artifact** of our conversations, discovery, and decisions — verbosely.
2. Keep the *findings* (what the code does) separate from the *strategy* (how we'd re-express it).
3. Capture decisions as they harden, with their rationale and the alternatives rejected.

## Documents

| Doc | What it holds |
|---|---|
| [00-conversation-log.md](00-conversation-log.md) | Verbose running log of the discussion — every turn, every idea, every reframing. Append-only. |
| [01-codebase-analysis.md](01-codebase-analysis.md) | The deep multi-agent code investigation: object model, rendering, physics, networking, input/audio/content, levels/modes. Behavioral spec of the existing game. |
| [02-vision-and-strategy.md](02-vision-and-strategy.md) | The strategic evaluation: remaster vs. faithful port, the substrate spectrum (custom engine ↔ UE-native + Chaos), consequences of each direction, professional recommendation, and the Turn-3 resolutions. |
| [03-roadmap.md](03-roadmap.md) | The **Position B** phased build plan (Phase 0 Feel Charter → 7 Meta/breadth), milestones, deferrals, and critical-path risks. |
| [decisions/0001-foundational-decisions.md](decisions/0001-foundational-decisions.md) | The five locked foundational decisions (direction, dimensionality, netcode, art, level content) with rationale, consequences, and rejected alternatives. |

## How to use this

- **Before any execute phase**, the relevant strategy doc and decision record must exist.
- New discoveries → append to `00-conversation-log.md` and update the relevant topic doc.
- When a fork is decided, write a short record in `decisions/` (date, decision, rationale,
  alternatives rejected, consequences accepted).

## Guiding principle (working)

> **The existing DGR code is a *behavioral spec*, not an *architectural blueprint*.**
> We mine it to understand *what the game does and why it feels the way it does*, then decide
> independently *how Unreal should achieve that* — optimizing for future extensibility
> (3D, VR, new perspectives, new modes), not for line-by-line fidelity.
