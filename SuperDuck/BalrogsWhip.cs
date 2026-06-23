using System;
using System.Collections.Generic;
using DuckGame;

namespace DuckGame.SuperDuck
{
    [EditorGroup("SuperDuck")]
    public class BalrogsWhip : Gun
    {
        // ── Verlet chain ──────────────────────────────────────────────────────
        private const int NumJoints  = 7;
        private const float SegLen   = 5f;
        private const int SolveIter  = 5;
        private const float Gravity  = 0.45f;
        private const float Damping  = 0.96f;

        private Vec2[] _j = new Vec2[NumJoints];
        private Vec2[] _p = new Vec2[NumJoints];
        private Vec2 _prevAnchor;
        private bool _jointsReady;

        // ── Attack state ─────────────────────────────────────────────────────
        public StateBinding _crackBinding = new StateBinding("_cracking");
        public bool _cracking;
        private float _crackTimer;
        private bool _prevCracking;

        // ── Passive fire drip ─────────────────────────────────────────────────
        private int _dripCounter;
        private const int DripInterval = 45;

        // ── Ambient looping fire sound ────────────────────────────────────────
        private Sound _ambientSound;
        private bool _ambientPlaying;

        // ── Glow pulse ────────────────────────────────────────────────────────
        private float _glowPhase;

        // ── Fire shield ───────────────────────────────────────────────────────
        private Duck _shieldDuck;
        private float _shieldTimer;
        private const float ShieldLingerTime = 1.5f;

        public BalrogsWhip(float xval, float yval) : base(xval, yval)
        {
            _editorName = "Balrog's Whip";
            _type       = "gun";
            ammo      = 999;
            _ammoType = new AT9mm { range = 50f };
            isFatal   = false;
            graphic           = new Sprite(GetPath("sprites/balrogswhip"));
            center            = new Vec2(4f, 3f);
            collisionOffset   = new Vec2(-10f, -3f);
            collisionSize     = new Vec2(20f, 6f);
            _barrelOffsetTL   = new Vec2(20f, 3f);
            _holdOffset       = new Vec2(0f, 0f);
            _fireSound  = GetPath("SFX/firewhipcrack");
            _fullAuto   = false;
            _fireWait   = 9f;
            _kickForce  = 0f;
            editorTooltip = "YOU SHALL NOT PASS! A fire shield guards the wielder. The whip drips flame and cracks lethal fire forward on attack.";
        }

        // ── TEMP DIAGNOSTICS (remove once the client-visibility bug is solved) ────
        // Three gates, logged once each on every machine:
        //   INIT  → the ghost Thing was constructed + Initialize() ran on this machine.
        //   UPDATE→ GhostObject.Update passed IsInitialized() and called our DoUpdate (client only
        //           reaches here once ALL BufferedGhostProperties have received data).
        //   DRAW  → the Layer draw gate (visible && ghostObject.IsInitialized()) passed.
        // On the CLIENT, the pattern reveals the failing gate:
        //   INIT only          → ghost exists but IsInitialized() never true (property never inits).
        //   INIT+UPDATE, noDRAW→ updating but draw gate / held-draw path drops it.
        //   none               → ghost never created on the client (receive-path swallow).
        private bool _dbgInit, _dbgUpdate, _dbgDraw;

        private void Dbg(string gate)
        {
            string side = !Network.isActive ? "OFFLINE" : (isServerForObject ? "HOST" : "CLIENT");
            bool init = ghostObject != null && ghostObject.IsInitialized();
            DevConsole.Log(DCSection.DuckNet,
                "|DGGREEN|[WHIP " + gate + "] side=" + side +
                " ghost=" + (ghostObject != null) +
                " inited=" + init +
                " duck=" + (duck != null) +
                " vis=" + visible +
                " pos=" + position.x.ToString("0") + "," + position.y.ToString("0"));
        }

        public override void Initialize()
        {
            // Match the proven physics-sim mod guns (EmisGuitar/Reaper), whose Initialize() does
            // NOTHING but call base — all static setup lives in the constructor, and anything
            // dynamic happens lazily where it belongs. The two things this used to do here are now
            // handled elsewhere: the joints already self-init in Update (`if (!_jointsReady)
            // InitJoints()`), so calling it here was redundant; and the looped ambient sound is
            // created on first hold in Update (mirrors this mod's own SauronEyeGhost.StartChaseLoop).
            // Keeping Initialize trivial removes the only structural difference between this gun and
            // the working mod guns.
            base.Initialize();

            if (!_dbgInit) { _dbgInit = true; Dbg("INIT"); }
        }

        private void InitJoints()
        {
            Vec2 anchor = barrelPosition;
            for (int i = 0; i < NumJoints; i++)
            {
                _j[i] = anchor + new Vec2(0f, i * SegLen);
                _p[i] = _j[i];
            }
            _prevAnchor = anchor;
            _jointsReady = true;
        }

        public override void Update()
        {
            if (!_dbgUpdate) { _dbgUpdate = true; Dbg("UPDATE"); }

            base.Update();

            // ── Fire shield ───────────────────────────────────────────────────
            // Clear onFire + _burnTime every frame AND snuff nearby SmallFire
            // so the wielder cannot catch fire even for a single tick.
            if (duck != null)
            {
                _shieldDuck   = duck;
                _shieldTimer  = ShieldLingerTime;
                duck.onFire   = false;
                duck._burnTime = 1f;
                if (isServerForObject)
                    SnuffNearbyFire(duck);
            }
            else if (_shieldTimer > 0f)
            {
                _shieldTimer -= Maths.IncFrameTimer();
                if (_shieldDuck != null && !_shieldDuck.dead)
                {
                    _shieldDuck.onFire   = false;
                    _shieldDuck._burnTime = 1f;
                    if (isServerForObject)
                        SnuffNearbyFire(_shieldDuck);
                }
                if (_shieldTimer <= 0f)
                    _shieldDuck = null;
            }

            // ── Ambient sound ─────────────────────────────────────────────────
            // Created lazily on first hold (like SauronEyeGhost.StartChaseLoop), not in Initialize.
            // Guarded so a sound hiccup can never affect the held gun.
            if (_ambientSound == null && duck != null)
            {
                try { _ambientSound = SFX.Get(GetPath("SFX/firewhip"), 0f, looped: true); }
                catch { _ambientSound = null; }
            }
            if (_ambientSound != null)
            {
                float targetVol = duck != null ? 0.9f : 0f;
                _ambientSound.Volume = Maths.LerpTowards(_ambientSound.Volume, targetVol, 0.08f);
                if (!_ambientPlaying && _ambientSound.Volume > 0.01f)
                {
                    _ambientSound.Play();
                    _ambientPlaying = true;
                }
                else if (_ambientPlaying && _ambientSound.Volume < 0.01f)
                {
                    _ambientSound.Stop();
                    _ambientPlaying = false;
                }
            }

            _glowPhase += 0.13f;

            // ── Crack rising-edge: impulse + flash on server AND ghost clients ─
            if (_cracking && !_prevCracking)
            {
                ApplyCrackImpulse();
                _crackTimer = 0.4f;
                if (Options.Data.flashing)
                    Graphics.flashAdd = 0.5f;
            }
            _prevCracking = _cracking;

            if (_crackTimer > 0f)
            {
                _crackTimer -= Maths.IncFrameTimer();
                if (_crackTimer <= 0f)
                    _cracking = false;
            }

            if (!_jointsReady)
                InitJoints();
            SimulateWhip();

            // ── Lethal contact during crack: tip proximity + line trace ──────────
            if (_cracking && isServerForObject)
            {
                Vec2 tip = GetExtendedTip();
                Vec2 lineOrigin = _j[0];
                Vec2 lineVec = tip - lineOrigin;
                float lineLen = lineVec.length;
                Vec2 lineNorm = lineLen > 0.001f ? lineVec / lineLen : new Vec2(offDir, 0f);

                foreach (Duck target in Level.current.things[typeof(Duck)])
                {
                    if (target == null || target.dead || target == duck) continue;

                    bool hit = false;

                    // Tip proximity (original check).
                    float dx = tip.x - target.x;
                    float dy = tip.y - target.y;
                    if (dx * dx + dy * dy < 100f)
                        hit = true;

                    // Line trace: kills ducks within ~8px of the strike line so close-range
                    // ducks in the fire direction aren't missed by the tip check alone.
                    if (!hit)
                    {
                        Vec2 toTarget = target.position - lineOrigin;
                        float proj = toTarget.x * lineNorm.x + toTarget.y * lineNorm.y;
                        if (proj >= 0f && proj <= lineLen)
                        {
                            float perpX = toTarget.x - lineNorm.x * proj;
                            float perpY = toTarget.y - lineNorm.y * proj;
                            if (perpX * perpX + perpY * perpY < 64f)
                                hit = true;
                        }
                    }

                    if (hit)
                        target.Kill(new DTIncinerate(this));
                }
            }

            // ── Passive fire drip (only while held) ───────────────────────────
            if (duck != null)
            {
                _dripCounter++;
                if (_dripCounter >= DripInterval && isServerForObject)
                {
                    _dripCounter = 0;
                    int idx = Rando.Int(NumJoints - 2) + 1;
                    Level.Add(SmallFire.New(
                        _j[idx].x, _j[idx].y,
                        Rando.Float(-0.4f, 0.4f), 0f,
                        true, null, true, this));
                }
            }
            else
            {
                _dripCounter = 0;
            }
        }

        public override void Removed()
        {
            if (_ambientPlaying && _ambientSound != null)
            {
                _ambientSound.Stop();
                _ambientPlaying = false;
            }
            base.Removed();
        }

        // Removes all SmallFire within 16px of the target duck so the shield is
        // absolute — no fire can even touch the duck momentarily.
        private void SnuffNearbyFire(Duck target)
        {
            List<SmallFire> toRemove = new List<SmallFire>();
            foreach (SmallFire sf in Level.current.things[typeof(SmallFire)])
            {
                if (sf == null) continue;
                float dx = sf.x - target.x;
                float dy = sf.y - target.y;
                if (dx * dx + dy * dy < 256f)
                    toRemove.Add(sf);
            }
            foreach (SmallFire sf in toRemove)
                Level.Remove(sf);
        }

        private void SimulateWhip()
        {
            Vec2 anchor = barrelPosition;
            _prevAnchor = anchor;
            bool doIdleCurl = duck != null && !_cracking;

            for (int i = 1; i < NumJoints; i++)
            {
                Vec2 vel = (_j[i] - _p[i]) * Damping;
                _p[i]    = _j[i];
                float swayScale = i / (float)(NumJoints - 1);
                float sx = 0f, sy = 0f;

                // Spring joint 1: aim upward from the handle during idle so the chain
                // starts reaching above the duck rather than laterally.
                if (duck != null && i == 1)
                {
                    Vec2 ideal = doIdleCurl
                        ? _j[0] + new Vec2(offDir * SegLen, 0f)
                        : _j[0] + new Vec2(offDir * SegLen * 0.8f, -SegLen * 0.3f);
                    sx += (ideal.x - _j[1].x) * 0.4f;
                    sy += (ideal.y - _j[1].y) * 0.4f;
                }

                // Rotating S-curve curl on joints 2+ during idle.
                if (doIdleCurl && i >= 2)
                {
                    float jPhase = _glowPhase * 0.5f + i * 0.8f;
                    sx += (float)Math.Sin(jPhase) * 1.3f * swayScale;
                    sy += (float)Math.Cos(jPhase) * 1.2f * swayScale;
                }

                // Movement drag: far joints trail when the duck runs, making the
                // whip stream fluidly instead of following the handle like a rigid rod.
                if (duck != null)
                {
                    float moveScale = 0.45f * swayScale;
                    sx -= duck.hSpeed * moveScale;
                    sy -= duck.vSpeed * moveScale * 0.4f;
                }

                // Negative idle gravity lets curl forces push joints above the duck.
                float effGravity = doIdleCurl ? -0.1f : Gravity;
                _j[i] = _j[i] + vel + new Vec2(sx, effGravity + sy);
            }

            for (int s = 0; s < SolveIter; s++)
            {
                _j[0] = anchor;
                for (int i = 0; i < NumJoints - 1; i++)
                {
                    Vec2 diff = _j[i + 1] - _j[i];
                    float dist = diff.length;
                    if (dist < 0.001f) continue;
                    float corr = (dist - SegLen) / dist * 0.5f;
                    Vec2 corrVec = diff * corr;
                    if (i > 0) _j[i] = _j[i] + corrVec;
                    _j[i + 1] = _j[i + 1] - corrVec;
                }
                _j[0] = anchor;
            }
        }

        // Returns a point 2 segments beyond the whip tip along its final direction,
        // extending effective crack reach by ~33% beyond the rope's physical length.
        private Vec2 GetExtendedTip()
        {
            Vec2 dir = _j[NumJoints - 1] - _j[NumJoints - 2];
            float len = dir.length;
            if (len < 0.001f)
                return _j[NumJoints - 1];
            return _j[NumJoints - 1] + (dir / len) * SegLen * 2f;
        }

        public override void Fire()
        {
            if (!loaded || ammo <= 0 || _wait > 0f) return;

            _cracking = true;

            if (isServerForObject)
            {
                float dir = offDir;
                Vec2 tip = GetExtendedTip();
                Vec2 mid = _j[NumJoints - 2];
                Level.Add(SmallFire.New(tip.x, tip.y, dir * 9f,                             -3.5f,                         false, null, true, this));
                Level.Add(SmallFire.New(tip.x, tip.y, dir * 12f + Rando.Float(-1.5f, 1.5f), -2f + Rando.Float(-2f, 0.5f), false, null, true, this));
                Level.Add(SmallFire.New(tip.x, tip.y, dir * 7f,                             -5.5f,                         false, null, true, this));
                Level.Add(SmallFire.New(tip.x, tip.y, dir * 14f + Rando.Float(-0.5f, 0.5f), -1f,                           false, null, true, this));
                Level.Add(SmallFire.New(mid.x, mid.y, dir * 5f,                             -4f,                           false, null, true, this));
                Level.Add(SmallFire.New(mid.x, mid.y, dir * 8f + Rando.Float(-1f, 1f),      -2.5f,                         false, null, true, this));
            }

            Reload(false);
            _wait = _fireWait;
            PlayFireSound();
        }

        private void ApplyCrackImpulse()
        {
            float dir = offDir;
            for (int i = 1; i < NumJoints; i++)
            {
                float t    = i / (float)(NumJoints - 1);
                float fwd  = 22f * t;
                float lift = 4f;
                _p[i] = _j[i] + new Vec2(-dir * fwd, lift);
            }
        }

        public override void Draw()
        {
            if (!_dbgDraw) { _dbgDraw = true; Dbg("DRAW"); }

            base.Draw();
            if (!_jointsReady) return;

            float pulse = 0.7f + 0.3f * (float)Math.Sin(_glowPhase);

            for (int i = 0; i < NumJoints - 1; i++)
            {
                float t = i / (float)(NumJoints - 2);
                Color segColor = Color.Lerp(new Color(180, 25, 0), new Color(255, 195, 20), t) * pulse;
                float thick = 2.5f - t * 1.0f;
                Graphics.DrawLine(_j[i], _j[i + 1], segColor, thick, depth);
            }

            Vec2 tip = _j[NumJoints - 1];
            Color tipColor = new Color(255, 230, 90) * (pulse * 0.65f);
            Graphics.DrawLine(tip + new Vec2(-2f, 0f), tip + new Vec2(2f, 0f), tipColor, 4f, depth);
            Graphics.DrawLine(tip + new Vec2(0f, -2f), tip + new Vec2(0f, 2f), tipColor, 4f, depth);

            // During crack, draw the extended-reach segment so the player can see the kill zone.
            if (_cracking && _crackTimer > 0f)
            {
                Vec2 extTip  = GetExtendedTip();
                float fade   = _crackTimer / 0.4f;
                Color extCol = new Color(255, 255, 150) * (fade * 0.7f);
                Graphics.DrawLine(tip, extTip, extCol, 1.5f, depth);
                Graphics.DrawLine(extTip + new Vec2(-3f, 0f), extTip + new Vec2(3f, 0f), extCol, 3f, depth);
                Graphics.DrawLine(extTip + new Vec2(0f, -3f), extTip + new Vec2(0f, 3f), extCol, 3f, depth);
            }

            // Fire shield ring
            Duck shieldTarget = duck;
            if (shieldTarget == null && _shieldTimer > 0f)
                shieldTarget = _shieldDuck;

            if (shieldTarget != null && !shieldTarget.dead)
            {
                float alpha = duck != null ? 1f : (_shieldTimer / ShieldLingerTime);
                float sp    = 0.5f + 0.5f * (float)Math.Sin(_glowPhase * 2.5f);
                Color sc    = new Color(255, 100, 0) * (alpha * (0.35f + 0.25f * sp));
                float r     = 9f + sp * 2.5f;
                int segs    = 10;
                for (int k = 0; k < segs; k++)
                {
                    float a0 = (float)(k       * 2.0 * Math.PI / segs);
                    float a1 = (float)((k + 1) * 2.0 * Math.PI / segs);
                    Vec2 p0 = shieldTarget.position + new Vec2((float)Math.Cos(a0) * r, (float)Math.Sin(a0) * r);
                    Vec2 p1 = shieldTarget.position + new Vec2((float)Math.Cos(a1) * r, (float)Math.Sin(a1) * r);
                    Graphics.DrawLine(p0, p1, sc, 2f, depth);
                }
            }
        }
    }
}
