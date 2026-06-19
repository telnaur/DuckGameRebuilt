# Ostrich Mod V2

A maintained copy of OstrichMod (Workshop `2956579195`) with two classes of fixes:

1. **Zawarudo ("ZA WARUDO" time-stop hat)** — no longer freezes/crashes online.
   See `OstrichMod/src/AT/ATZawarudo.cs`, `OstrichMod/src/Equip/Zawarudo.cs`,
   `OstrichMod/src/Equip/ZawarudoThing.cs`.
2. **"Mod data mismatch" when joining online lobbies** — addressed by changing how the
   mod is packaged (see below).

---

## Player "force-fix": when you still see *"Mod data mismatch: OstrichMod"*

This error means your copy of the mod produced a different data hash than the host's.
With OstrichMod the usual cause is a stale or mismatched **compiled** copy of the mod on
one player's machine. To force every player back into the same state, have **everyone in
the lobby** do this together:

1. **Confirm you're all on the same DGR version.** Different game versions = guaranteed
   mismatch. (Main menu shows the version; it must match for everyone.)
2. **Re-download the mod cleanly:**
   - Unsubscribe from OstrichMod in the Steam Workshop.
   - Delete the folder `…/Steam/steamapps/workshop/content/312530/2956579195` if it remains.
   - Restart Steam, then resubscribe and let it finish downloading.
3. **Delete the local build cache** inside the mod folder (these regenerate automatically):
   - `OstrichMod_compiled.dll`
   - `OstrichMod_compiled.hash`
   - `OstrichMod_compiledRebuilt.dll`
   - `OstrichMod_compiledRebuilt.pdb`
   - `OstrichMod_compiledRebuiltData.txt`
   - `OstrichMod_build.log` (if present)
4. **Launch Duck Game once and wait** at the menu for ~10–20s so the mod recompiles from
   source (you'll briefly see "COMPILING MOD OstrichMod"). Then host/join.

Why this works: deleting the precompiled `*.dll` + `*.hash` forces *every* player to
recompile from the **same source files**, instead of some players using a prebuilt DLL and
others recompiling — which is what made the hashes diverge. The HDD→system-drive move that
once fixed it did the same thing by accident: it reset file timestamps and triggered a
re-download/recompile.

> Tip to tell which "camp" a machine is in: check the modified-time of
> `OstrichMod_compiled.dll`. Recent / near launch time = it recompiled locally; old /
> matching the download = it used the prebuilt DLL.

---

## What changed in V2 to fix this permanently (maintainer notes)

The original mod shipped **both** the C# source **and** a prebuilt `OstrichMod_compiled.dll`
+ `OstrichMod_compiled.hash`. At load, DGR (`ModLoader.GetOrLoad` →
`AttemptCompile`) does this:

- `assemblyPath` is `OstrichMod.dll` — which the mod never shipped — so DGR always entered
  the compile path.
- It CRC32s the local `.cs` files and compares to the shipped `_compiled.hash`. **Match →
  reuse the author's prebuilt DLL. Mismatch → recompile locally.**
- The author's prebuilt DLL and a local recompile emit `Thing` types in **different
  metadata order**, so the two groups computed different `thingHash` → *"Mod data
  mismatch."* A single invisible byte difference in the source (line-ending/encoding drift,
  a stray file, a partial download) flips a machine between the two groups, which is why it
  appeared and vanished seemingly at random.

**V2 removes the prebuilt binary and its hash gate** (`OstrichMod_compiled.dll`,
`OstrichMod_compiled.hash`, and the per-machine `*_compiledRebuilt.*` / `*RebuiltData.txt`
cache). Now there is no "reuse the prebuilt DLL" path: every client recompiles the same
sorted source set, so everyone lands on the same `thingHash`. (csc emits type definitions
in source-declaration order regardless of compiler version, and DGR feeds the source files
in sorted, drive-independent order — so the result is stable across machines.)

### Optional: bulletproof packaging (zero recompile variance)

Recompiling-everywhere fixes the dominant cause. To eliminate *all* remaining variance
(e.g. differing in-box C# compilers across Windows builds), ship a single canonical binary
so no machine ever recompiles:

1. Build the mod once from this fixed source by launching it in DGR and letting it compile
   — this produces `OstrichMod_compiled.dll` containing the Zawarudo fixes.
2. Rename that file to **`OstrichMod.dll`** (so `assemblyPath` exists and DGR skips
   compilation entirely).
3. Delete the leftover `OstrichMod_compiled.*` and `OstrichMod_compiledRebuilt.*` /
   `*RebuiltData.txt` cache files (never ship these — they're per-machine).
4. Add to `mod.conf` as a safety net so a missing binary fails loudly instead of silently
   recompiling-and-diverging:

   ```xml
   <Mod>
     <WorkshopID>2956579195</WorkshopID>
     <NoCompilation>true</NoCompilation>
   </Mod>
   ```

> Important: rebuild `OstrichMod.dll` from source **every time the source changes** — a
> stale binary is deterministic but wrong. Do not rename the pre-fix `_compiled.dll` that
> originally shipped; it predates the Zawarudo fixes.
