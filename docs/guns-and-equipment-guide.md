# Coding Guns & Equipment in Duck Game Rebuilt

A practical, reference-backed guide to adding new weapons and equipment. All file/line
references point at the actual source in this repo so you can jump straight to a working
example. Paths are relative to the repo root.

> **Mental model:** every item is a `Thing` subclass that configures itself in its
> constructor and overrides a handful of virtual hooks. There is **no central registry**
> to edit — items are discovered by reflection over the `[EditorGroup]` attribute. The two
> things you must get right beyond "it compiles" are **adding the file to the `.csproj`**
> and **network state via `StateBinding`**.

---

## 1. The class hierarchy

```
Thing                         src/DuckGame/Thing.cs        (abstract base for everything in the world)
└─ MaterialThing                                            (physical objects: collision, physics)
   └─ Holdable                                              (can be picked up / held / thrown)
      ├─ Gun            src/DuckGame/Weapons/Gun.cs         (abstract; ALL weapons extend this, even melee)
      │  ├─ Pistol, AK47, Shotgun, ...                      (hitscan / projectile guns)
      │  ├─ Grenade     src/DuckGame/Weapons/Grenade.cs     (thrown explosive)
      │  └─ Sword       src/DuckGame/Weapons/Sword.cs       (melee — overrides Fire() to do nothing)
      └─ Equipment      src/DuckGame/Equipment/Equipment.cs (abstract; wearable gear)
         ├─ Hat ─ Helmet, KnightHelmet, TinfoilHat, ...     (head slot; armor via _isArmor)
         ├─ ChestPlate, Boots, Holster, Jetpack, ...
         └─ ...
```

Note that **`Gun` is the base for melee weapons too** — `Sword` extends `Gun` and simply
overrides `Fire()` to be empty (`src/DuckGame/Weapons/Sword.cs:985`), doing all its damage
in `Update()` via line checks. Don't assume "Gun == fires bullets."

---

## 2. Minimum viable weapon

The smallest complete weapon in the codebase is `AK47` (`src/DuckGame/Weapons/AK47.cs`).
The entire item is constructor configuration:

```csharp
namespace DuckGame
{
    [EditorGroup("Guns|Machine Guns")]          // editor category (root|sub|sub...)
    [BaggedProperty("isSuperWeapon", true)]     // optional metadata flags
    public class AK47 : Gun
    {
        public AK47(float xval, float yval) : base(xval, yval)
        {
            ammo = 30;
            _ammoType = new ATHighCalMachinegun();   // bullet behavior lives in an AmmoType
            _type = "gun";
            graphic = new Sprite("ak47");            // sprite name in the atlas
            center = new Vec2(16f, 15f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize  = new Vec2(18f, 10f);
            _barrelOffsetTL = new Vec2(32f, 14f);    // top-left-relative muzzle position
            _fireSound = "deepMachineGun2";
            _fullAuto = true;
            _fireWait = 1.2f;                        // cooldown between shots
            _kickForce = 3.5f;
            _fireRumble = RumbleIntensity.Kick;
            loseAccuracy = 0.2f;                     // accuracy lost per shot
            maxAccuracyLost = 0.8f;
            editorTooltip = "Go-to weapon of all your favorite Duck Action Heroes.";
        }
    }
}
```

That is a fully working, multiplayer-safe gun. `Gun` already provides firing, reloading,
kick, ammo, smoke, accuracy, and all the network bindings (see §5).

### Key constructor fields (defined on `Gun`, `src/DuckGame/Weapons/Gun.cs:8-72`)

| Field | Purpose | Ref |
|---|---|---|
| `ammo` | starting ammo count | `Gun.cs:20` |
| `_ammoType` | the `AmmoType` that defines the projectile | `Gun.cs:8` |
| `_barrelOffsetTL` | muzzle position (top-left relative; converted via `barrelOffset`) | `Gun.cs:36,92` |
| `_fireSound` / `_clickSound` | SFX names | `Gun.cs:40-41` |
| `_fireWait` | frames-ish cooldown; set into `_wait` after each shot | `Gun.cs:43,523` |
| `_fullAuto` | hold-to-fire vs press-to-fire | `Gun.cs:48` |
| `_numBulletsPerFire` | pellets per shot (shotguns) | `Gun.cs:49` |
| `_kickForce` / `_fireRumble` | recoil + controller rumble | `Gun.cs:18-19` |
| `loseAccuracy` / `maxAccuracyLost` | spread accumulation | `Gun.cs:24,26` |
| `_manualLoad` | if true, you call `Reload()` yourself (pump/bolt action) | `Gun.cs:50,521` |
| `graphic` / `center` / `collisionOffset` / `collisionSize` | visuals & hitbox | inherited |
| `editorTooltip` / `_editorName` / `_bio` | editor & UI text | `Gun.cs:35,88` |

---

## 3. The firing lifecycle (where to put your logic)

Input → fire flows through these virtuals on `Gun`. Override the ones you need:

1. **`OnPressAction()`** — fired once on trigger press; for semi-auto guns it calls
   `Fire()` (`Gun.cs:403-408`). Override to add muzzle effects (see `Pistol.OnPressAction`,
   `src/DuckGame/Weapons/Pistol.cs:46-61`).
2. **`OnHoldAction()`** — fired every frame the trigger is held; for full-auto it calls
   `Fire()` (`Gun.cs:410-414`).
3. **`Fire()`** — the core shot: checks `loaded`/`ammo`/`_wait`, applies kick, spawns
   `_numBulletsPerFire` bullets via `_ammoType.FireBullet(...)`, plays sound, reloads,
   sets `_wait = _fireWait` (`Gun.cs:451-543`). Override for fully custom behavior; melee
   weapons override it to **do nothing** (`Sword.cs:985`).
4. **`Reload(bool shell)`** — decrements ammo, pops a shell (`Gun.cs:560-568`).
5. **`PlayFireSound()`** — override to customize pitch/randomization (`Gun.cs:545`).
6. **`Update()`** — per-frame logic (animation, timers). **Always call `base.Update()`**
   — the base does a lot (smoke, accuracy decay, spin physics when dropped, etc.,
   `Gun.cs:244-349`).
7. **`Draw()` / `DrawGlow()`** — rendering; call `base.Draw()` (`Gun.cs:570`).

### Example patterns by weapon type

- **Semi-auto with muzzle sparks:** `Pistol` overrides `OnPressAction` to spawn `Spark`
  particles, sets a fire animation, then calls `Fire()` (`Pistol.cs:46-61`). Note the use
  of `DGRSettings.ActualParticleMultiplier` to scale particle counts — **always gate
  particle loops on this** so users with effects turned down aren't hammered
  (`Pistol.cs:51`).
- **Thrown explosive:** `Grenade` (`src/DuckGame/Weapons/Grenade.cs`). `OnPressAction`
  pulls the pin (`Grenade.cs:172-190`); `Update` runs the fuse timer and spawns the
  explosion + shrapnel bullets on the server (`Grenade.cs:94-163`). Note
  `OnNetworkBulletsFired` (`Grenade.cs:56-63`) — the network hook that makes remote clients
  show the explosion.
- **Melee:** `Sword` (`src/DuckGame/Weapons/Sword.cs`). Empty `Fire()`; damage done in
  `Update()` via `Level.CheckLineAll<IAmADuck>(...)` between `barrelStartPos` and
  `barrelPosition`, gated on `isServerForObject` (`Sword.cs:517-534`, `758-813`). Swing
  state (`_swing`, `_hold`, stances) is all network-bound (`Sword.cs:9-15`).

---

## 4. Projectiles: `AmmoType` and `Bullet`

A gun's projectile behavior is **not** in the gun — it's in an `AmmoType`
(`src/DuckGame/Weapons/Bullets/AmmoTypes/AmmoType.cs`). The gun just holds an instance in
`_ammoType` and calls `_ammoType.FireBullet(...)` from `Gun.Fire()` (`Gun.cs:493`).

`AmmoType` (abstract, `AmmoType.cs:7-164`) exposes tuning fields: `accuracy`, `range`,
`penetration`, `bulletSpeed`, `bulletThickness`, `affectedByGravity`, `rebound`,
`bulletColor`, etc. (`AmmoType.cs:10-33`). To make a new projectile:

- **Reuse** an existing `AT*` type (e.g. `ATHighCalMachinegun`, `AT9mm`, `ATShrapnel`)
  configured inline — see `Grenade.cs:32-35` setting `penetration` on an `ATShrapnel`.
- **New AmmoType:** subclass `AmmoType`, set fields in the constructor, and optionally set
  `bulletType` to a custom `Bullet` subclass (`AmmoType.cs:38`). Existing `AT*` files live
  in `src/DuckGame/Weapons/Bullets/AmmoTypes/`; bullet visuals/physics in
  `src/DuckGame/Weapons/Bullets/`.

> **⚠️ Network ordering gotcha:** `AmmoType.InitializeTypes()` assigns each type a `byte`
> index by enumerating subclasses **in discovery order** (`AmmoType.cs:42-63`). That index
> is what's sent over the wire. Two game builds only agree on which bullet is which if they
> discover the **same set of AmmoTypes in the same order**. In practice this means: adding
> or removing an `AmmoType` changes indices, so **every player must run the identical
> build** (see §8). The same principle applies to networked `Thing` and `NetMessage` types.

---

## 5. Networking: `StateBinding` (the part people get wrong)

Multiplayer sync is pervasive — most `Thing` subclasses are "ghosts" replicated from the
authoritative owner to everyone else. Any runtime state that other players must see needs
a **`StateBinding`** field, or it will desync.

`Gun` already binds the common stuff (`Gun.cs:9-13`):

```csharp
public StateBinding _ammoBinding   = new StateBinding(nameof(netAmmo));
public StateBinding _waitBinding    = new StateBinding(nameof(_wait));
public StateBinding _loadedBinding  = new StateBinding(nameof(loaded));
public StateBinding _bulletFireIndexBinding = new StateBinding(nameof(bulletFireIndex));
public StateBinding _infiniteAmmoValBinding = new StateBinding(nameof(infiniteAmmoVal));
```

When you add **new** networked state, declare a matching binding. Examples:

- `Grenade` binds its fuse timer and pin state (`Grenade.cs:9-10`):
  ```csharp
  public StateBinding _timerBinding = new StateBinding(nameof(_timer));
  public StateBinding _pinBinding   = new StateBinding(nameof(_pin));
  ```
- `Helmet` binds a single `crushed` bool (`src/DuckGame/Equipment/Helmet.cs:8-9`).
- `Sword` binds swing/stance floats and a custom `SwordFlagBinding` for packed booleans
  (`Sword.cs:9-15`).

### Authority rules

- Guard authoritative actions (spawning bullets, dealing damage, removing things) with
  **`isServerForObject`** so only the owner runs them; the result is replicated. See
  `Gun.Fire()` (`Gun.cs:494,499`), `Grenade.cs:123`, `Sword.cs:517`.
- To make a remote client react to a server event, send/handle a `NMxxx` net message (e.g.
  `Grenade` sends `NMFireGun`, `Gun.cs:397` sends `NMExplodingProp`). Net message classes
  live in `src/DuckGame/Network/NM*.cs`.
- `OnNetworkBulletsFired(Vec2 pos)` (`Gun.cs:116`, overridden `Grenade.cs:56`) is the hook
  the network layer calls on clients when the owner fires.

If your item only needs *position/velocity/angle* synced (a plain physics object), the base
classes already handle that — you only add bindings for **extra** state you introduce.

---

## 6. Equipment specifics

Equipment extends `Holdable` via `Equipment` (`src/DuckGame/Equipment/Equipment.cs`). It
adds equip/unequip, wearing on the duck's skeleton, and armor behavior.

Minimal example — `Helmet` (`src/DuckGame/Equipment/Helmet.cs`):

```csharp
[EditorGroup("Equipment")]
public class Helmet : Hat
{
    public StateBinding _crushedBinding = new StateBinding(nameof(crushed));
    public bool crushed;

    public Helmet(float xpos, float ypos) : base(xpos, ypos)
    {
        _pickupSprite = new Sprite("helmetPickup");
        _sprite = new SpriteMap("helmet", 32, 32);
        graphic = _pickupSprite;
        center = new Vec2(8f, 8f);
        collisionOffset = new Vec2(-5f, -2f);
        collisionSize  = new Vec2(12f, 8f);
        _sprite.CenterOrigin();
        _isArmor = true;            // makes it block a bullet & get knocked off
        _equippedThickness = 3f;
        editorTooltip = "Protects your precious, precious brain from impacts.";
    }
}
```

Useful `Equipment` members (`Equipment.cs`):

- `_isArmor` — armor blocks one hit and is knocked off; see `Equipment.Hit(...)`
  (`Equipment.cs:246-267`).
- `_equippedBinding` / `equipIndex` — sync who's wearing it (`Equipment.cs:5-7`).
- `_hasEquippedCollision`, `_equippedCollisionOffset/Size` — different hitbox while worn
  (`Equipment.cs:159-180`).
- `PositionOnOwner()` — snaps the item to the duck's bone each frame (head/torso/feet
  selected by type in `Equipment.cs:90-116`). Override `Update()`/`Draw()` and call base.
- `wearOffset` — fine-tune the worn position (`Equipment.cs:54-58`).

Hats specifically extend `Hat`; head-slot positioning is automatic.

---

## 7. Attributes & registration (no central list to edit)

- **`[EditorGroup("root|sub|sub")]`** — the *only* registration step. The level editor and
  spawn system discover items by reflecting over this attribute
  (`src/MonoTime/LevelEditor/EditorGroupAttribute.cs`). Format is pipe-delimited nesting,
  e.g. `"Guns|Machine Guns"`, `"Guns|Explosives"`, `"Guns|Melee"`, `"Equipment"`.
- **`[BaggedProperty("key", value)]`** — optional metadata flags read elsewhere. Common
  ones seen in-tree: `isInDemo`, `previewPriority` (`Pistol.cs:4-5`), `isSuperWeapon`
  (`AK47.cs:4`). Grep existing weapons for more.
- **`EditorItemType`** — an overload of `[EditorGroup]` takes an `EditorItemType` for
  special handling (`EditorGroupAttribute.cs:22`).

### ⚠️ You MUST add new files to the `.csproj`

`DuckGame/DuckGame.csproj` is an **old-style (non-SDK) project**: every source file is
listed explicitly with `<Compile Include="..." />` (see the huge `<ItemGroup>` starting at
`DuckGame.csproj:305`). Consequences:

- **In Visual Studio**, adding a file to the project does this automatically — just make
  sure "Show All Files" didn't trick you into leaving it merely on disk but excluded.
- **If you create the file by hand or on Linux**, you must add a matching
  `<Compile Include="src\DuckGame\Weapons\MyGun.cs" />` line yourself or it won't compile.
- Files under `Weapons/` and `Equipment/` *appear* auto-included in some listings, but
  confirm your new file has a `<Compile>` entry before reporting a "mysterious" missing
  type at runtime.

---

## 8. Assets (sprites & sounds)

- Sprites are referenced by **name** (`new Sprite("ak47")`, `new SpriteMap("pistol", 18, 10)`).
  Names resolve against the packed atlas: `spriteatlas.png` + `spriteatlas_offsets.txt` at
  the repo root. A `SpriteMap(name, w, h)` is a sheet sliced into `w×h` frames; add named
  animations with `AddAnimation("idle", speed, loop, frames...)` (`Pistol.cs:18-21`).
- Sounds are referenced by name through `SFX.Play("pistolFire")` / the `_fireSound` field.
- Shaders live in `shaders_source/` and are compiled by the post-build step (see the build
  pipeline notes).

When adding new art, you generally need to get it into the atlas (or load via `Content`) —
study how existing items reference their sprites and mirror that. If a sprite name doesn't
resolve, you get a blank/placeholder rather than a crash.

---

## 9. Step-by-step: add a new gun

1. Copy the closest existing weapon file in `src/DuckGame/Weapons/` and rename the class +
   file (e.g. `BurstPistol.cs`).
2. Set `[EditorGroup("Guns|Pistols")]` (and any `[BaggedProperty]` flags you want).
3. In the constructor: set `ammo`, `_ammoType`, `graphic`, `center`,
   `collisionOffset/Size`, `_barrelOffsetTL`, `_fireSound`, `_fireWait`, recoil, accuracy,
   and `editorTooltip`.
4. Override only the hooks you need (`OnPressAction`, `Fire`, `Update`, `Draw`). Call
   `base.*` in `Update`/`Draw`.
5. For any **new runtime state** other players must see, add a `StateBinding` and guard
   authoritative logic with `isServerForObject`.
6. Scale all particle loops by `DGRSettings.ActualParticleMultiplier`.
7. **Confirm a `<Compile Include>` entry exists** in `DuckGame/DuckGame.csproj`.
8. Build & run; the item shows up in the level editor under its `[EditorGroup]` path. Test
   it in a hosted match (and in multiplayer if it has networked state — see below).

## 10. Equipment checklist

Same as above but extend `Equipment` (or `Hat`/`ChestPlate`/etc.), set `_isArmor` if it
should block hits, configure worn collision via `_hasEquippedCollision`, and rely on
`PositionOnOwner()` for placement.

---

## 11. Testing your item in multiplayer (the safe rule)

Because AmmoType/Thing/NetMessage indices and the version handshake depend on the build,
**every participant must run the byte-identical binary** when testing networked content.
For how to host a private session for remote testers without touching Steam Workshop or
upstream git, see the build-pipeline notes / project README. Short version: build locally,
zip `bin/`, distribute the same zip to everyone, and host a Private Steam lobby (or use a
VPN + LAN host). Mixing builds → version-mismatch kick or silent desync.

---

## Reference index

| Topic | File |
|---|---|
| Gun base (firing, reload, kick, bindings) | `src/DuckGame/Weapons/Gun.cs` |
| Simple gun example | `src/DuckGame/Weapons/AK47.cs`, `Pistol.cs` |
| Thrown explosive | `src/DuckGame/Weapons/Grenade.cs` |
| Melee (empty Fire, line-check damage) | `src/DuckGame/Weapons/Sword.cs` |
| AmmoType / projectile tuning | `src/DuckGame/Weapons/Bullets/AmmoTypes/AmmoType.cs` |
| Bullet implementations | `src/DuckGame/Weapons/Bullets/` |
| Equipment base | `src/DuckGame/Equipment/Equipment.cs` |
| Equipment example (armor hat) | `src/DuckGame/Equipment/Helmet.cs` |
| Editor registration attribute | `src/MonoTime/LevelEditor/EditorGroupAttribute.cs` |
| Project file (must list new .cs) | `DuckGame/DuckGame.csproj` |
| Networking (ghosts/authority/messages) | `src/DuckGame/Network/` |
