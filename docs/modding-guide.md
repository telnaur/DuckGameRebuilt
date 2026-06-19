# Duck Game Rebuilt — Authoritative Modding Guide

This is the master reference for building, testing, and publishing mods for **Duck Game
Rebuilt (DGR)**. It covers the whole lifecycle: how the mod system works, how to lay a mod
out, how to add content, how multiplayer compatibility is enforced, and how to ship to the
Steam Workshop — all backed by `file:line` references into this repo so you can verify and
go deeper.

For the **code** of guns, equipment, and projectiles specifically, see the companion deep
dive: [`guns-and-equipment-guide.md`](./guns-and-equipment-guide.md). That document and
this one are meant to be read together — this one is the container, that one is the
contents.

> **The one-sentence model:** a DGR mod is a *folder of C# source files plus a manifest*
> that the game **compiles at startup** and loads into the running game, where your
> `[EditorGroup]`-tagged `Thing` subclasses are discovered automatically — exactly as if
> they were built into the engine.

---

## 0. Two things called "publishing" — don't confuse them

| | **DGR itself** (the engine) | **Your content mod** (what this guide is about) |
|---|---|---|
| What ships | The whole rebuilt game | Your guns/equipment/levels |
| How | GitHub Release → CI (`autobuild_for_release.yml`) → SteamCMD | **In-game** Mod Management → UPLOAD |
| Steam item | The "Rebuilder" workshop item | A new workshop item you own |
| You touch it? | No | Yes — this is your path |

Everything below is about **your content mod**. You never run the CI or SteamCMD.

---

## 1. How the mod system works

### 1.1 Compilation at startup
DGR compiles each mod from source when the game launches:

- `ModLoader.AttemptCompile` (Windows, CodeDom) / `LinuxAttemptCompile` (Linux, Roslyn)
  compiles every `*.cs` in the mod folder (`ModLoader.cs:709`, `783`).
- The compile references **all currently-loaded assemblies** — including the running
  `DuckGame.exe` (DGR) — so your code can call any public DGR/Duck Game type
  (`ModLoader.cs:753`).
- Compiler symbols `DGR` and `WINDOWS` are defined (`ModLoader.cs:754`). Use `#if DGR` to
  branch DGR-specific code from vanilla-Duck-Game code.
- Output is cached next to the mod as `<name>_compiled.dll` + `<name>_compiled.hash`
  (`ModConfiguration.cs:232,234`). The cache is keyed to `DG.version` via a `CompiledFor`
  record and **automatically recompiled when the game version changes**
  (`ModLoader.cs:1004-1031`).
- Exactly **one** `AssemblyInfo.cs` is allowed; extra ones are filtered out during compile
  (`ModLoader.cs:839-853`).

> **Alternative — ship a prebuilt DLL.** Set `<NoCompilation>true</NoCompilation>` in
> `mod.conf` (`ModConfiguration.cs:278`) and place a compiled `<name>.dll` in the folder.
> The game then loads the DLL directly and never compiles source. Use this if you want to
> keep source closed or build in Visual Studio with full tooling. (`Rebuilder/mod.conf`
> does exactly this — it's a `NoCompilation` loader mod.)

### 1.2 Loading & lifecycle
- Mods are discovered from the mods directories (§2.1) and Steam-subscribed workshop items
  (`ModLoader.PreLoadMods`, `ModLoader.cs:1189-1254`).
- The loader finds your mod's entry class by reflection:
  `assembly.GetExportedTypes().Where(t => t.IsSubclassOf(typeof(Mod)) && !t.IsAbstract)`
  (`ModLoader.cs:631`). **Every code mod needs exactly one non-abstract `Mod` subclass.**
- Dependencies are resolved and load order sorted before init (`ModLoader.cs:601-617`).
- Your `Mod` subclass gets three lifecycle hooks, in order (`Mod.cs:269-292`):
  1. `OnPreInitialize()` — set up properties; not all mods have registered content yet.
  2. `OnPostInitialize()` — all mods have contributed content; safe to cross-reference.
  3. `OnStart()` — everything loaded and the first level is set.

---

## 2. Anatomy of a mod

### 2.1 Folder location
Mods live under your Duck Game save directory (`DuckFile.cs:225,281,133,139,157`):

```
<Documents>/DuckGame/Mods/<YourMod>/              ← global mods   (DuckFile.globalModsDirectory)
<Documents>/DuckGame/<steamID>/Mods/<YourMod>/    ← per-user mods (DuckFile.modsDirectory)
```

Either works for development; drop your folder there and launch.

### 2.2 Folder structure
```
MyWeapons/
├─ mod.conf                  ← XML manifest (loading flags + WorkshopID)
├─ AssemblyInfo.cs           ← mod display metadata (name/version/author/description)
├─ MyWeaponsMod.cs           ← entry class : Mod
├─ BurstPistol.cs            ← [EditorGroup] gun (see guns-and-equipment-guide.md)
├─ ATBurst.cs                ← custom AmmoType (optional)
├─ NMBurstFx.cs              ← custom NetMessage (optional)
└─ content/                  ← assets, scanned by ContentPack (ModLoader.cs:941)
   ├─ preview.png            ← Workshop thumbnail (Mod.cs:208-234)
   ├─ screenshot.png         ← Workshop screenshot (Mod.cs:238-260)
   ├─ sprites/...            ← your textures (.png)
   ├─ sounds/...             ← your sounds (.wav/.ogg)
   └─ Levels/...             ← optional bundled levels (ContentPack.cs:90)
```

### 2.3 `mod.conf` (the manifest)
XML, parsed in `ModConfiguration.LoadConfiguration` (`ModConfiguration.cs:255-283`).
**Loading behavior and IDs only** — display metadata comes from `AssemblyInfo.cs` (§2.4),
not here.

```xml
<Mod>
  <MajorSupportedRevision>1</MajorSupportedRevision>   <!-- 1 = post-2020 DG; set this or DG may auto-disable -->
  <PreloadContent>true</PreloadContent>                <!-- load assets at startup; avoids in-game stutter -->
  <PinkTransparency>false</PinkTransparency>           <!-- treat (255,0,255) as transparent; off = faster load -->
  <NoCompilation>false</NoCompilation>                 <!-- true = load a prebuilt DLL instead of compiling source -->
  <NoRecompilation>false</NoRecompilation>             <!-- true = never recompile even on version change -->
  <NoPreloadAssets>false</NoPreloadAssets>
  <SoftDependencies>OtherModName</SoftDependencies>    <!-- pipe-separated; may interact, not required -->
  <HardDependencies>RequiredModName</HardDependencies> <!-- pipe-separated; must be loaded first -->
  <WorkshopID>0</WorkshopID>                            <!-- 0 until first upload; the game fills this in -->
</Mod>
```

Recognized tags and their fields: `SoftDependencies`/`HardDependencies`
(`ModConfiguration.cs:260-275`), `NoRecompilation` (`:276`), `NoPreloadAssets` (`:277`),
`NoCompilation` (`:278`), `PreloadContent` (`:279`), `PinkTransparency` (`:280`),
`WorkshopID` (`:281`), `MajorSupportedRevision` (`:282`). Dependencies are matched by mod
`name` or `uniqueID` (`ModConfiguration.cs:60-74`).

### 2.4 `AssemblyInfo.cs` (the display metadata)
The mod's **display name, version, author, and description come from assembly attributes**,
read off the compiled assembly (`ModConfiguration.cs:214-224`):

```csharp
using System.Reflection;

[assembly: AssemblyTitle("My Weapons Pack")]        // → displayName
[assembly: AssemblyDescription("Adds 5 new guns.")] // → description
[assembly: AssemblyCompany("YourName")]             // → author
[assembly: AssemblyVersion("1.0.0.0")]              // → version
```

If you omit `AssemblyTitle`, the folder name is used; missing `AssemblyCompany` yields
author `"Unknown"` (`ModConfiguration.cs:215,224`).

> **The filename doesn't matter — the attributes do.** These `[assembly: …]` attributes are
> plain C# and can live at the top of **any one** source file; a file literally named
> `AssemblyInfo.cs` is just convention. Putting them atop your entry `Mod` class works fine
> and keeps a tiny mod to a single file. The "exactly one `AssemblyInfo.cs`" rule from §1.1 is
> about not having the **same assembly attribute declared twice** (which is a compile error) —
> so pick one home for them. If you generate an `AssemblyInfo.cs`, don't also declare them
> elsewhere.

### 2.5 The entry `Mod` class
Minimal content mod — the class only needs to exist; logic is optional:

```csharp
using DuckGame;

namespace MyWeapons
{
    public class MyWeaponsMod : Mod
    {
        // Optional. Your [EditorGroup] Things are discovered automatically;
        // you only need hooks if you do cross-mod setup or runtime registration.
        protected override void OnPostInitialize()
        {
            Mod.Debug.Log("MyWeapons loaded");
        }
    }
}
```

Useful members on `Mod` (`Mod.cs`): `GetPath(asset)` to resolve a content path (§3),
`configuration` for your manifest data, `priority`/`SetPriority` for load order
(`Mod.cs:184-186`), `properties`/`_properties` for a cross-mod property bag (`Mod.cs:25,177`),
`previewTexture`/`screenshot` for Workshop art (`Mod.cs:208-260`).

---

## 3. Adding content

### 3.1 Things (guns, equipment, anything in the world)
This is the heart of it, and it's identical to engine code — **see
[`guns-and-equipment-guide.md`](./guns-and-equipment-guide.md)** for the full treatment of
`Gun`, `Equipment`, `AmmoType`, `StateBinding`, the firing lifecycle, and authority rules.

The only mod-specific note: registration is still purely the **`[EditorGroup("…")]`
attribute** — the editor/spawn system reflects over it across all loaded assemblies
(`EditorGroupAttribute.cs`), so a `[EditorGroup]` class in your mod DLL shows up in the
editor automatically. No manual registration call, no central list.

### 3.2 Assets — sprites, animation & sound

Mod assets live in the mod's `content/` folder, which the engine sets as `contentDirectory`
(`ModLoader.cs:941`). `ContentPack` recursively **scans that folder by file extension** and
preloads what it recognizes (`ContentPack.cs:67-100`, extension map `Content.cs:47-64`):

| Asset | **Required file type** | Loaded as | Reference it with |
|---|---|---|---|
| Sprite / texture | **`.png` only** | `Texture2D` | `new Sprite(path)` / `new SpriteMap(path, w, h)` |
| Sound effect | **`.wav` only** | `SoundEffect` | `SFX.Play(path)` |
| Music track | **`.ogg` only** | `Song` | `Music.Play(path)` |
| Levels (optional) | `.lev` | `Level` | bundled playlist (`content/Levels/`) |

There is **no other supported format** — a `.jpg`/`.gif` sprite, `.mp3`/`.ogg` *sound
effect*, or `.wav` *music track* is silently ignored by the scanner. (`.xnb` content-pipeline
files are an engine thing, not for mods.)

#### Reference everything by `GetPath` (not by bare name)

Engine-built items name the packed atlas directly (`new Sprite("ak47")`). **Mod assets are
referenced by their path**, resolved through `GetPath`, which prepends your
`contentDirectory` (`Mod.cs:160-172`):

```csharp
// From inside a Thing in your mod:
graphic = new Sprite(Mod.GetPath<MyWeaponsMod>("sprites/burstpistol"));
// or, from within the Mod instance itself:
graphic = new Sprite(this.GetPath("sprites/burstpistol"));
```

- **Omit the extension.** The loaders append `.png`/`.wav`/`.ogg` themselves
  (`ContentPack.cs:145,183,212`), and the preload cache is keyed on the *extensionless* path
  (`ContentPack.cs:74,85`) — so `GetPath("sprites/foo")` resolves to the **preloaded** copy,
  while `GetPath("sprites/foo.png")` works too but re-reads from disk.
- A bare `new Sprite("foo")` will **not** find mod content — it looks in the engine atlas.
- You can use any subfolder layout under `content/` (`sprites/`, `sounds/`, `SFX/`, or the
  root); `GetPath` just mirrors wherever you put the file. Subfolders are scanned recursively.

#### Sprites (static)
A PNG can be any size. Two ways to get transparency:
1. **A real alpha channel** (the normal, recommended way — author a 32-bit RGBA PNG).
2. **Pink/magenta key**: pixels of exactly `(255, 0, 255)` are made transparent, but **only
   if** the mod sets `<PinkTransparency>true</PinkTransparency>` in `mod.conf`
   (`TextureConverter.cs:166,325`; flag at `ContentPack.cs:230`). This is the legacy
   XNA/Duck-Game convention for art drawn without an alpha channel. It's **off by default**
   (faster load), so if you rely on a magenta background, you must turn it on.

Either way the loader **premultiplies alpha** (`TextureConverter.cs:20`) — relevant only if
you author custom shaders; for normal sprites it just works. Duck Game art is pixel-art at
small sizes (e.g. the AK47 sprite is 32×16); match that scale and the game's `Vec2 center` /
`collisionOffset` / `collisionSize` fields position and hit-box it.

#### Animation (`SpriteMap`)
For multi-frame art, use a **sprite sheet**: a single PNG laid out in a grid of equal cells,
sliced by `SpriteMap(path, frameWidth, frameHeight)` (`SpriteMap.cs:131`). Frames are
**numbered left-to-right, top-to-bottom** starting at 0 (`SpriteMap.cs:147-155`), so a 64×32
sheet with 16×16 cells is frames 0-3 on the top row, 4-7 on the bottom.

```csharp
var sm = new SpriteMap(this.GetPath("sprites/wand"), 16, 16);
// AddAnimation(name, speed, looping, params int[] frames)
sm.AddAnimation("idle", 0.2f, true,  0, 1, 2, 1);   // speed = frames advanced per tick
sm.AddAnimation("cast", 0.6f, false, 3, 4, 5, 6);
sm.SetAnimation("idle");        // (or set .currentAnimation)
graphic = sm;
```

- `speed` is the advance rate (frames per update tick); `0f` freezes on the current frame.
- Drive playback from your Thing: `sm.SetAnimation("cast")` to switch, read `sm.finished`
  for non-looping anims (`SpriteMap.cs:55`), set `sm.frame` to scrub, `sm.flipH` to mirror
  by `offDir`. See `Pistol.cs:18-21` and the Ostrich mod's `ToyHammer.cs` (a `swordSwipe`
  swing sheet) for working examples.
- A `SpriteMap` with no explicit `AddAnimation` gets a "default" animation spanning every
  cell (`SpriteMap.cs:147`), handy for a sheet you index by hand via `.frame`.

#### Sound effects (`.wav`)
Drop `.wav` files anywhere in `content/`; they're auto-registered under their extensionless
`GetPath` key (`ContentPack.cs:48,87`). Play with:

```csharp
// SFX.Play(string sound, float vol = 1, float pitch = 0, float pan = 0, bool looped = false)
SFX.Play(this.GetPath("sounds/zap"), 0.9f, Rando.Float(-0.1f, 0.1f));
```

`vol` is 0-1, `pitch` is an **offset** (0 = normal, ±1 ≈ an octave), `pan` is -1 (left) to
1 (right) (`SFX.cs:214`). A missing/unloaded sound name simply doesn't play (no crash), which
is why a typo'd name fails silently. Files ≤5 MB are loaded fully into memory, larger ones
stream from disk (`ContentPack.cs:166`) — keep SFX short.

#### Music (`.ogg`)
Background tracks must be **Ogg Vorbis** (`OggSong.Load`, `ContentPack.cs:195`). Play with
`Music.Play(this.GetPath("music/theme"))` (`Music.cs:197`, loops by default).

#### Workshop art (preview & screenshot)
Publishing requires images in `content/`: `preview.png` (thumbnail) and `screenshot.png`
(`Mod.cs:208-260`) — both **PNG**. See §6.

> **Preload vs. on-demand.** With `<PreloadContent>true</PreloadContent>` everything above is
> loaded at startup so there's no mid-match hitch when an item first appears
> (`ModLoader.cs:637`). Without it, assets load lazily on first use. Preload is the safe
> default for a gameplay mod.

### 3.3 NetMessages (custom networked events)
If your content sends custom network events (e.g. a special effect on fire), subclass
`NetMessage`. Rules enforced by `Network.InitializeMessageTypes` (`Network.cs:411-474`):

- **Every `NetMessage` must have a parameterless constructor** (`new MyMsg()`), or the game
  errors/crashes in mod-debug (`Network.cs:445-455`).
- IDs are assigned automatically by discovery order; for a stable ID across versions, apply
  `[FixedNetworkID(n)]` (`FixedNetworkID`, used at `Network.cs:419-436`).
- Client-only messages (no server authority) can be marked `[ClientOnly]` so they're
  excluded from the network compatibility hash (`Network.cs:471`).

---

## 4. Multiplayer compatibility (read this before testing online)

DGR enforces that connected players agree on content, because wire formats are derived from
**discovery order and type-name hashes**, not explicit versioning:

- **Per-mod data hash.** Each `Mod` computes `thingHash` (CRC of all its `Thing` type
  names), `netMessageHash` (CRC of its `NetMessage` type names), and `dataHash =
  thingHash + netMessageHash` (`Mod.cs:44-84`). This is how the game decides whether two
  players' copies of a mod match.
- **Global ordering.** `AmmoType` indices (`AmmoType.cs:42-63`), `NetMessage` IDs
  (`Network.cs:411-474`), and `Thing` type hashes feed the overall network identity. Adding
  or renaming a networked type **changes the wire format**.
- **`StateBinding` is mandatory** for any runtime state other players must see — covered in
  depth in the guns guide (§5 there). Guard authoritative actions with `isServerForObject`.

**Practical rule:** every player in a session must run the **same version of the same
mod**. Once published, Steam Workshop subscription handles this automatically (and the
loader can require server mods on clients, `ModLoader.cs:1203-1213`). During development,
re-share your folder after any change to a `Thing`/`AmmoType`/`NetMessage`.

---

## 5. Build & test locally

1. Put the mod folder in `…/DuckGame/Mods/` (§2.1).
2. Launch DGR (Steam running). The mod compiles on startup; compile errors land in
   `<name>_build.log` (`ModConfiguration.cs:236`) and the dev console.
3. Enable mod debugging for verbose logs: launch with `-moddebug` (gates `Mod.Debug.Log`
   and turns NetMessage misconfig into hard errors — `Mod.cs:321-326`, `Network.cs:449`).
4. Iterate: editing a `.cs` triggers a recompile on next launch (cache auto-invalidates).
   If a stale cache ever bites you, delete `<name>_compiled.dll` / `.hash`.
5. Find your items in the **level editor** under their `[EditorGroup]` path, place them,
   and play-test. For networked behavior, test in a hosted match with a second client
   running the **identical** mod folder.

---

## 6. Publish to the Steam Workshop (in-game)

Publishing is done entirely from inside the game via `UIModManagement` — no CI, no SteamCMD,
no git. Flow (`UIModManagement.cs:152-185`, `440-529`):

1. **Main menu → Mods / Mod Management.** Select your loaded mod, choose **UPLOAD**
   (`UIModManagement.cs:339`). (Requires Steam running and you to own Duck Game.)
2. **First upload:**
   - `Steam.CreateItem()` creates a new Workshop item and returns its ID
     (`UIModManagement.cs:168`).
   - The game writes that ID back into your `mod.conf` (`SetWorkshopID`,
     `UIModManagement.cs:457` → `ModConfiguration.cs:324-332`) so future uploads update the
     same item.
   - Name/description are taken from your metadata; it's tagged `"Mod"` (or `Map Pack` /
     `Hat Pack` / `Texture Pack` by mod type) and set to **`Private` visibility by default**
     (`UIModManagement.cs:459-470`).
   - Your mod folder is copied to a staging area with **`build/`, `.vs/`, and the cached
     `_compiled.dll`/`.hash` stripped** (`UIModManagement.cs:486-510`) — so by default your
     **`.cs` source is uploaded** and each subscriber's game recompiles it locally. (Use
     `NoCompilation` + a prebuilt DLL if you don't want to ship source.)
   - Requires a preview/screenshot image (`generateAndGetPathToScreenshot`,
     `Mod.cs:123-155`).
   - It uploads via `ApplyWorkshopData`, shows the Workshop legal agreement if needed, and
     opens the Steam overlay to your new item page (`UIModManagement.cs:512-525`).
3. **Subsequent updates:** because `WorkshopID` is now set, it reuses the existing item and
   prompts for change notes (`UIModManagement.cs:171,473`).
4. **Set visibility on the Steam page** when ready: Private (only you) → Friends Only /
   Unlisted (testers) → Public.

---

## 7. Distributing to remote testers without going public

Because mods are lightweight folders, you have three options, easiest first:

1. **Share the folder.** Zip the mod folder; testers drop it in their `…/DuckGame/Mods/`.
   No Workshop involved. Best for fast iteration.
2. **Private/Unlisted Workshop item.** Upload (defaults to Private), then set the Steam item
   to **Friends Only** or **Unlisted** so invited/link-having testers can subscribe. Flip to
   Public when done.
3. **Public release.** Set the item Public once it's ready.

In all cases the multiplayer rule from §4 holds: testers must have the **same version** of
the mod. Workshop subscription enforces this automatically; folder-sharing does not, so
re-distribute after each change.

---

## 8. Advanced notes

- **Client-only content.** A mod can be a `ClientMod` / set `clientMod` (`Mod.cs:27`), and
  types marked `[ClientOnly]` are excluded from the network hash (`Network.cs:471`) — for
  purely visual content that doesn't need server agreement.
- **Dev/parent mod facades.** For maintaining a separate DEV build of a published mod,
  `Mod` exposes `workshopIDFacade`, `namespaceFacade`, and `assemblyNameFacade`
  (`Mod.cs:188-204`) so a dev build can serialize/deserialize as the released mod and share
  its levels.
- **Type sort order.** `configuration.SortedTypeNames` + `Mod.SortTypeOrder` let a rebuilt
  mod preserve the original Duck Game type order for cross-compatibility
  (`Mod.cs:93-117`, `ModConfiguration.cs:22`). Most new mods don't need this.
- **Mod types.** Besides code `Mod`, there are `Reskin` (texture packs), `MapPack`, and
  `HatPack` (`ModConfiguration.Type`, `ModConfiguration.cs:334-340`) — different content,
  same publishing path.

---

## 9. Troubleshooting / common gotchas

| Symptom | Likely cause | Fix |
|---|---|---|
| Mod doesn't appear | No non-abstract `Mod` subclass | Add `public class X : Mod` (`ModLoader.cs:631`) |
| Compile fails silently | Source error | Read `<name>_build.log`; launch with `-moddebug` |
| Item not in editor | Missing `[EditorGroup]` | Tag the class (`EditorGroupAttribute.cs`) |
| Sprite is blank | Bare name instead of path | Use `GetPath("…")` for mod assets (§3.2) |
| Mod auto-disabled in DG | Missing revision tag | `<MajorSupportedRevision>1</MajorSupportedRevision>` |
| Multiplayer desync / version kick | Mismatched mod versions | All players on identical mod (§4) |
| NetMessage crash on connect | No empty constructor | Add `public MyMsg() {}` (`Network.cs:445`) |
| Stale behavior after edit | Cached compiled DLL | Delete `<name>_compiled.dll`/`.hash` |

---

## 10. Reference index

| Topic | File |
|---|---|
| Mod base class & lifecycle | `src/MonoTime/Modding/ModLoader/Mod.cs` |
| Manifest parsing & metadata | `src/MonoTime/Modding/ModLoader/ModConfiguration.cs` |
| Discovery, compilation, loading | `src/MonoTime/Modding/ModLoader/ModLoader.cs` |
| In-game Workshop upload | `src/MonoTime/UI/UIModManagement.cs` |
| Mod asset loading | `src/MonoTime/Content/ContentPack.cs` |
| Mods/Workshop directories | `src/MonoTime/File/DuckFile.cs` |
| NetMessage registration | `src/DuckGame/Network/Network.cs` |
| Editor registration attribute | `src/MonoTime/LevelEditor/EditorGroupAttribute.cs` |
| Example manifest (NoCompilation) | `Rebuilder/mod.conf` |
| **Item code (guns/equipment/ammo)** | [`guns-and-equipment-guide.md`](./guns-and-equipment-guide.md) |
