using System;
using DuckGame;

namespace DuckGame.SuperDuck
{
    // "The One Ring" — worn equipment that grants a quack-toggled cloak.
    //
    // DESIGN NOTE ON INVISIBILITY (the load-bearing decision):
    // Duck Game does NOT have per-recipient visibility. Thing.visible is a plain local bool
    // (Thing.cs:71), not a StateBinding, and Draw() runs independently on every client from
    // replicated state. So we do NOT replicate "invisibility" — we replicate ONE bool
    // (_cloaked) and then every client decides locally whether to draw the wearer:
    //
    //     show the wearer  <=>  this client is the wearer's own machine
    //                           (wearer.profile.connection == DuckNetwork.localConnection)
    //
    // The owner keeps seeing themselves (ghosted); everyone else stops drawing them. This is
    // balanced for ONLINE play. In local splitscreen, localConnection is shared by the whole
    // couch, so a same-machine opponent would still see the cloaked duck — an accepted
    // limitation of this scope.
    //
    // WRAITH VISION uses Layer.colorMul (the same trick the Ostrich mod's Zawarudo uses to tint
    // the world): while WE are the cloaked wearer we multiply the Blocks/Game layers toward a
    // cold blue and outline enemies in fiery orange. colorMul is a per-machine global, so online
    // it only tints the cloaked player's own screen. It MUST be restored to Vec3.One on every
    // exit path (Zawarudo learned this the hard way — see its Removed()).
    //
    // DEFENSIVE HARDENING ("My Precious", §5) uses the BUILT-IN armor path, not Harmony: with
    // _isArmor + an equipped collision box, Equipment.Hit can intercept a lethal shot, knock the
    // ring off, and spare the duck (exactly like Helmet/ChestPlate). But the ring defends ONLY
    // while cloaked — our Hit override returns false (inert, shot passes to the duck) when
    // uncloaked, and only falls through to the armor path while cloaked, where it also adds the
    // panic flourish (force-uncloak + fling into a ragdoll).
    //
    // Base class: Equipment (not Hat) — Equipment.PositionOnOwner anchors non-Hat gear at the
    // upper torso, which is where the band should sit, and it avoids fighting Hat.Update().
    //
    // C# 5 (CodeDom) constraints apply to mod source on Windows: string-literal StateBindings,
    // no nameof/interpolation/null-conditional, explicit double->float casts.
    [EditorGroup("SuperDuck")]
    public class TheOneRing : Equipment
    {
        // The only networked gameplay state: are we cloaked? Set on the authority (the wearer's
        // machine), replicated so every client runs the same per-client visibility rule below.
        public StateBinding _cloakedBinding = new StateBinding("_cloaked");
        public bool _cloaked;

        private bool _prevCloaked;      // edge-detect on every client (SFX + restore on transition)
        private Duck _cloakedDuck;      // who we hid — kept so we can RELIABLY restore them
        private float _quackCooldown;   // server: debounce so one quack toggles once
        private SauronEyeGhost _eye;    // server: the pursuer; persists while the ring is WORN

        private float _glow;            // shimmer phase
        private bool _wasLocalTinted;   // local client: did WE push the wraith colorMul last frame?

        // Fire-rate boost (every client: holdObject is synced, _fireWait is deterministic, so each
        // client scales its own copy identically). We cache the held gun + its base cooldown so we
        // can restore it exactly when the bearer switches/drops the weapon.
        private Gun _boostedGun;
        private float _boostedBaseWait;

        // The cloak's continuous ambient hum (a looped Sound, NOT a one-shot sting). It is scoped to
        // the LOCAL wearer's own machine — we deliberately do NOT broadcast it to enemies, because a
        // constant audible beacon on the cloaked duck would undo the very stealth the cloak grants.
        // Created lazily + defensively (a throw must never break the ring); reused via Play/Stop.
        private Sound _cloakLoop;
        private bool _cloakLoopPlaying;

        private const float QuackDebounce = 0.25f;  // seconds
        private const float FireRateMultiplier = 2.0f; // held weapons fire this many times faster
        private const float SelfAlpha     = 0.4f;   // how see-through you look to yourself while cloaked
        private const float FlickerChance = 0.985f; // Rando.Float threshold to reveal for one frame to enemies

        // Cold, desaturated-blue world tint for wraith vision. A colorMul can't truly desaturate
        // (that needs a matrix), but dimming red/green and keeping blue reads as "the shadow realm".
        private static readonly Vec3 WraithTint = new Vec3(0.5f, 0.62f, 1f);

        public TheOneRing(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _editorName = "The One Ring";

            // Art hook: SuperDuck/content/sprites/onering.png (22x18). Extension omitted → uses
            // the preloaded texture (docs/modding-guide.md §3.2). Single sprite for both the
            // ground pickup and the worn band; PositionOnOwner places it on the torso when worn.
            graphic = new Sprite(GetPath("sprites/onering"));
            center          = new Vec2(11f, 9f);
            collisionOffset = new Vec2(-6f, -6f);
            collisionSize   = new Vec2(12f, 12f);

            // Armor hook (§5): mirrors ChestPlate so the equipped hitbox + armor path exist.
            // It only actually absorbs while cloaked — see the Hit() override, which is inert
            // (returns false) when uncloaked so the ring offers no protection in normal play.
            _isArmor               = true;
            _hasEquippedCollision  = true;
            _equippedCollisionOffset = new Vec2(-7f, -6f);
            _equippedCollisionSize   = new Vec2(14f, 14f);
            _equippedThickness     = 3f;
            _equippedDepth         = 4;
            _wearOffset            = new Vec2(-1f, 1f);
            wearable               = true;
            weight                 = 1f;
            thickness              = 0.1f;

            editorTooltip = "One quack to rule them all. QUACK to vanish; the Eye of Sauron hunts you while you hide, firing reveals you, and a hit makes the ring betray its bearer.";
        }

        // ── Cloak lifecycle (authority only) ─────────────────────────────────────

        private void Cloak(Duck d)
        {
            _cloaked = true;
            // The eye PERSISTS while the ring is worn — we only spawn it once, the first time the
            // bearer cloaks. Re-cloaking after an uncloak just resumes the existing eye's hunt from
            // wherever its clock had receded to (it is never reset by uncloaking).
            if (_eye == null || _eye.removeFromLevel)
            {
                _eye = new SauronEyeGhost(d);   // spawns on the authority, replicates itself
                _eye.ringRef = this;
                _eye.hunting = true;
                Level.Add(_eye);
            }
            else
            {
                _eye.targetDuck = d;            // in case the ring changed wearer
            }
        }

        private void Uncloak()
        {
            // Just drop the cloak. The eye is deliberately NOT removed here: while uncloaked it
            // recedes (its hunt clock counts back down) but keeps stalking. Only the ring leaving
            // the bearer resets the hunt — DespawnEye(), called from UnEquip()/Removed().
            _cloaked = false;
            // Visibility/tint restore happens uniformly in ApplyCloakVisuals() on the falling
            // edge of _cloaked, so it works on every client — not just the authority.
        }

        // Despawns the eye and forgets it — used when the ring is voluntarily dropped or removed,
        // which is the ONLY thing that resets the hunt. A betrayal does NOT go through here: it
        // hands the eye off (sets _eye = null) so the autonomous final pursuit survives the drop.
        private void DespawnEye()
        {
            if (_eye != null)
            {
                if (!_eye.removeFromLevel)
                    Level.Remove(_eye);
                _eye = null;
            }
        }

        // True while the ring is held or worn by some duck. The betrayed eye polls this on the
        // dropped ring: as long as nobody is holding it, the eye's 3-second give-up clock drains.
        public bool IsHeldOrWorn()
        {
            return owner != null || _equippedDuck != null;
        }

        // Called by the eye (co-authoritative with the bearer) the instant it reaches the worn
        // bearer. Reveals + knocks the bearer back and makes the ring betray them: it unequips
        // itself, dropping intact to the ground (pickable). _eye is nulled first so our own
        // UnEquip() does NOT despawn the eye — the eye is handed off to autonomous final pursuit.
        public void BeginBetrayal(Vec2 eyePos)
        {
            Duck d = _equippedDuck;
            _cloaked = false;
            _eye = null;            // hand off: the eye now self-manages its final pursuit
            if (d != null)
            {
                if (d.isServerForObject)
                {
                    // Shove away from the eye — a knockback, NOT a ragdoll, so the betrayed bearer
                    // keeps control and has a real chance to flee in the 3-second window.
                    float dirx = d.x < eyePos.x ? -1f : 1f;
                    d.hSpeed += dirx * Rando.Float(3f, 5f);
                    d.vSpeed -= Rando.Float(2f, 4f);
                }
                d.Unequip(this);    // drop the ring intact onto the ground (no Destroy)
            }
        }

        // ── Update ────────────────────────────────────────────────────────────────

        public override void Update()
        {
            base.Update();   // Equipment.Update positions us on the torso + handles equip plumbing.

            Duck d = duck;

            // Safety: if the ring was knocked off / dropped while cloaked, the wearer reference
            // is gone — drop the cloak from the authority so the binding restores everyone.
            if (isServerForObject && _cloaked && d == null)
                Uncloak();

            // --- authority: input → cloak, interlocks, safety cap, catch resolution ---
            if (d != null && isServerForObject)
            {
                if (_quackCooldown > 0f)
                    _quackCooldown -= Maths.IncFrameTimer();

                InputProfile ip = d.inputProfile;
                if (ip != null)
                {
                    if (ip.Pressed(Triggers.Quack) && _quackCooldown <= 0f)
                    {
                        if (_cloaked) Uncloak();
                        else          Cloak(d);
                        _quackCooldown = QuackDebounce;
                    }

                    // Firing breaks the cloak: pressing SHOOT while cloaked reveals you on the
                    // SAME frame (so the shot is never fired from true invisibility), but the shot
                    // itself still goes off normally. We deliberately do NOT stall the weapon — an
                    // earlier _wait interlock here ate the press (the gun advanced to its post-fire
                    // "needs reload" state without ever spawning a bullet), forcing a useless
                    // reload+refire. Just dropping the cloak lets Gun.Fire run on the same press.
                    if (_cloaked && ip.Pressed(Triggers.Shoot))
                        Uncloak();
                }

                // The cloak has NO duration of its own — it lasts indefinitely until a PRACTICAL
                // effect ends it: quacking again, firing, taking a hit (Hit override), or the Eye
                // of Sauron reaching the bearer (which triggers the betrayal, not a timeout).
                //
                // Drive the eye's hunt clock from the cloak state: it advances while cloaked and
                // recedes while uncloaked. Once the eye is gone (despawned, or handed off by a
                // betrayal) clear our reference so a fresh cloak can spawn a new one.
                if (_eye != null && !_eye.removeFromLevel)
                    _eye.hunting = _cloaked;
                else
                    _eye = null;
            }

            // --- every client: fire-rate boost on whatever the bearer is holding ---
            ApplyFireRateBoost(d);

            // --- every client: apply the per-client visibility + tells from _cloaked ---
            ApplyCloakVisuals(d);

            _glow += 0.1f;
            _prevCloaked = _cloaked;
        }

        // Runs on ALL clients. Translates the single replicated _cloaked bool into local render
        // state: hide the wearer (and what they hold) from everyone but their own machine, throw
        // an audio/visual tell on transitions, and reliably restore everything on uncloak.
        private void ApplyCloakVisuals(Duck d)
        {
            Duck target = d != null ? d : _cloakedDuck;

            // Edge transitions. The cloak "engage" sound is no longer a one-shot here — it is now a
            // continuous loop started/stopped below, and only for the local wearer (see field note).
            // The uncloak "reappear" sting stays a one-shot, heard on every client as the tell that
            // the duck has come back.
            if (!_cloaked && _prevCloaked)
            {
                SFX.Play(GetPath("SFX/onering_uncloak"), 0.8f);
                RestoreAll();
            }

            if (_cloaked && target != null && !target.dead)
            {
                _cloakedDuck = target;
                // Offline (no active network session) there is only the local couch, so the wearer
                // must always count as the local viewer — otherwise the cloak reads as TOTAL
                // invisibility in local testing (the connection compare below fails offline and
                // hid the wearer from their own screen). Online, only the wearer's own machine
                // shows them (ghosted); everyone else stops drawing them.
                bool localViewer = !Network.isActive
                                   || (target.profile != null && target.profile.connection == DuckNetwork.localConnection);
                bool reveal = localViewer || Rando.Float(0f, 1f) > FlickerChance;

                target.visible = reveal;
                if (target.holdObject != null)
                    target.holdObject.visible = reveal;

                // If the bearer ragdolls (electively or otherwise) their body is drawn by the
                // ragdoll PARTS, not target.visible — so without this the cloak visibly breaks the
                // instant they ragdoll. Ragdoll.visible cascades to all three parts (Ragdoll.cs:136).
                if (target.ragdoll != null)
                    target.ragdoll.visible = reveal;

                // ONLINE belt-and-suspenders: setting visible=false alone did NOT reliably hide the
                // duck on remote machines (it worked offline). The duck's own networked ghost re-runs
                // its update after ours and can re-assert visible=true before the draw, so enemies
                // kept seeing the "invisible" bearer. alpha is NOT ghost-restored, so we also drive it
                // to 0 when hidden — a fully-transparent duck renders nothing even if visible flips
                // back on. (The local wearer keeps SelfAlpha, set further below, so this only touches
                // the enemy/remote view.)
                if (!localViewer)
                {
                    float a = reveal ? 1f : 0f;
                    target.alpha = a;
                    if (target.holdObject != null)
                        target.holdObject.alpha = a;

                    // Enemy machine: never run the wearer's private cloak hum here.
                    StopCloakLoop();
                }

                // The ring's own sprite: shown to the local wearer (ghosted), hidden from enemies.
                visible = reveal;
                alpha   = localViewer ? SelfAlpha : 1f;

                if (localViewer)
                {
                    // The wearer (and only the wearer) hears the continuous cloak hum while hidden.
                    StartCloakLoop();

                    // The wearer sees themselves as a faint, shimmering wraith: ghosted alpha, a
                    // cold blue world tint, and an occasional wisp of "cloak smoke" so the cloaked
                    // state is unmistakable while they can still tell exactly where they are.
                    target.alpha = SelfAlpha;
                    Layer.Blocks.colorMul = WraithTint;
                    Layer.Game.colorMul   = WraithTint;
                    _wasLocalTinted = true;

                    if (Rando.Float(1f) < 0.1f)
                        SmallSmoke.New(target.x + Rando.Float(-4f, 4f), target.y + Rando.Float(-6f, 4f));
                }
            }
            else
            {
                visible = true;
                alpha   = 1f;
                StopCloakLoop();
                if (_cloakedDuck != null)
                    RestoreAll();
                else
                    RestoreTint();
            }
        }

        // ── Cloak ambient loop (local wearer only) ───────────────────────────────────
        private void StartCloakLoop()
        {
            try
            {
                if (_cloakLoop == null)
                    _cloakLoop = SFX.Get(GetPath("SFX/onering_cloak"), 0.85f, looped: true);
                if (_cloakLoop != null && !_cloakLoopPlaying)
                {
                    _cloakLoop.Volume = 0.85f;
                    _cloakLoop.Play();
                    _cloakLoopPlaying = true;
                }
            }
            catch
            {
                _cloakLoop = null;
            }
        }

        private void StopCloakLoop()
        {
            if (_cloakLoop != null && _cloakLoopPlaying)
                _cloakLoop.Stop();
            _cloakLoopPlaying = false;
        }

        private void RestoreAll()
        {
            if (_cloakedDuck != null)
            {
                if (!_cloakedDuck.dead)
                {
                    _cloakedDuck.visible = true;
                    _cloakedDuck.alpha   = 1f;
                    if (_cloakedDuck.holdObject != null)
                    {
                        _cloakedDuck.holdObject.visible = true;
                        _cloakedDuck.holdObject.alpha   = 1f;
                    }
                    if (_cloakedDuck.ragdoll != null)
                        _cloakedDuck.ragdoll.visible = true;
                }
                _cloakedDuck = null;
            }
            RestoreTint();
        }

        private void RestoreTint()
        {
            if (_wasLocalTinted)
            {
                Layer.Blocks.colorMul = Vec3.One;
                Layer.Game.colorMul   = Vec3.One;
                _wasLocalTinted = false;
            }
        }

        // ── §5 "My Precious": the ring defends ONLY while cloaked ────────────────────
        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            // Uncloaked, the ring is defensively inert: return false so the shot passes through
            // to the duck. We must NOT fall through to the armor base.Hit here, because with
            // _isArmor set it would absorb the shot and knock the ring off even uncloaked
            // (MaterialThing.Hit would otherwise block it via thickness > penetration).
            if (!_cloaked)
                return false;

            // Cloaked: the ring betrays its bearer. The built-in armor path (base.Hit) knocks the
            // ring off and spares the duck; we add the panic — force-uncloak + fling into ragdoll.
            if (_equippedDuck != null && bullet != null && bullet.owner != _equippedDuck)
            {
                Duck d = _equippedDuck;
                if (isServerForObject)
                    Uncloak();
                // Physics belongs to the duck's authority; the knock-off itself (KnockOffEquipment
                // inside base.Hit) is network-safe, this is the extra flourish on top.
                if (d.isServerForObject)
                {
                    d.hSpeed = (hitPos.x < d.x ? 1f : -1f) * Rando.Float(3f, 5f);
                    d.vSpeed = -Rando.Float(3f, 5f);
                    d.GoRagdoll();
                }
            }
            return base.Hit(bullet, hitPos);
        }

        public override void UnEquip()
        {
            if (isServerForObject)
            {
                Uncloak();
                // Voluntarily dropping the ring resets the hunt. A betrayal already handed the eye
                // off (_eye == null), so this only fires for a genuine player drop.
                DespawnEye();
            }
            RestoreFireRate();
            RestoreAll();
            StopCloakLoop();
            base.UnEquip();
        }

        public override void Removed()
        {
            RestoreFireRate();
            RestoreAll();
            StopCloakLoop();
            DespawnEye();
            base.Removed();
        }

        // ── Fire-rate boost ──────────────────────────────────────────────────────────
        // Scale down the held gun's _fireWait (its between-shots cooldown) so the bearer fires
        // FireRateMultiplier times faster, restoring the gun's own value the moment they let go of
        // it. Runs on every client (holdObject + _fireWait are both deterministic/synced), so the
        // scaled cooldown stays consistent everywhere. Works cloaked or not.
        private void ApplyFireRateBoost(Duck d)
        {
            Gun held = d != null ? d.holdObject as Gun : null;
            if (held == _boostedGun)
                return;                 // same gun (or still none) — nothing to change

            RestoreFireRate();          // put the previous gun's cooldown back first
            if (held != null)
            {
                _boostedGun = held;
                _boostedBaseWait = held._fireWait;
                held._fireWait = _boostedBaseWait / FireRateMultiplier;
            }
        }

        private void RestoreFireRate()
        {
            if (_boostedGun != null)
            {
                if (!_boostedGun.removeFromLevel)
                    _boostedGun._fireWait = _boostedBaseWait;
                _boostedGun = null;
            }
        }

        // ── Rendering ───────────────────────────────────────────────────────────

        public override void Draw()
        {
            base.Draw();   // Equipment.Draw → PositionOnOwner + Thing.Draw (the gold band sprite)

            // A faint golden shimmer ring over the sprite for a little life.
            float pulse = 0.75f + 0.25f * (float)Math.Sin(_glow);
            Color gold = new Color(255, 205, 60) * (alpha * pulse * 0.5f);
            int segs = 10;
            float r = 8f;
            for (int k = 0; k < segs; k++)
            {
                float a0 = (float)(k * 2.0 * Math.PI / segs);
                float a1 = (float)((k + 1) * 2.0 * Math.PI / segs);
                Vec2 p0 = position + new Vec2((float)Math.Cos(a0) * r, (float)Math.Sin(a0) * r);
                Vec2 p1 = position + new Vec2((float)Math.Cos(a1) * r, (float)Math.Sin(a1) * r);
                Graphics.DrawLine(p0, p1, gold, 1.5f, depth + 2);
            }

            // Wraith vision: while WE are the cloaked wearer, outline enemy ducks in fiery orange.
            // (Only reached when visible == true, which holds for the local wearer.)
            bool localViewer = _cloakedDuck != null && _cloakedDuck.profile != null
                               && _cloakedDuck.profile.connection == DuckNetwork.localConnection;
            if (_cloaked && localViewer)
                DrawWraithVision(_cloakedDuck);
        }

        private void DrawWraithVision(Duck wearer)
        {
            Team myTeam = wearer != null ? wearer.team : null;
            foreach (Duck other in Level.current.things[typeof(Duck)])
            {
                if (other == null || other == wearer || other.dead)
                    continue;
                if (myTeam != null && other.team == myTeam)
                    continue;

                Color o = Color.OrangeRed;
                float hw = 7f, ht = 11f;
                Vec2 c = other.position;
                Depth dd = other.depth + 4;
                Graphics.DrawLine(c + new Vec2(-hw, -ht), c + new Vec2(hw, -ht), o, 1.5f, dd);
                Graphics.DrawLine(c + new Vec2(-hw,  ht), c + new Vec2(hw,  ht), o, 1.5f, dd);
                Graphics.DrawLine(c + new Vec2(-hw, -ht), c + new Vec2(-hw, ht), o, 1.5f, dd);
                Graphics.DrawLine(c + new Vec2( hw, -ht), c + new Vec2( hw, ht), o, 1.5f, dd);
            }
        }
    }
}
