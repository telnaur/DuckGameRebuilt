using System;
using System.Linq;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|tech")]
    [BaggedProperty("isSuperWeapon", true)]
    public class FreezeRay : Gun
    {
        public StateBinding _targetStateBinding = new StateBinding("_target");
        public StateBinding _drawPositionStateBinding = new CompressedVec2Binding("_drawPosition");
        public StateBinding _chargingStateBinding = new StateBinding("_charging");
        public StateBinding _coolingDownStateBinding = new StateBinding("_coolingDown");
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");
        public StateBinding _chargeTimerStateBinding = new StateBinding("_chargeTimer");
        public StateBinding netSFX_freezeStateBinding = new NetSoundBinding("netSFX_freeze");
        public StateBinding netSFX_chargeStateBinding = new NetSoundBinding("netSFX_charge");

        public NetSoundEffect netSFX_freeze = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\exp1")
        });
        public NetSoundEffect netSFX_charge = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\freezeRay")
        })
        {
            volume = 0.5f
        };

        public PhysicsObject _target;
        public Vec2 _drawPosition;
        public bool _charging;
        public bool _coolingDown;
        public int _cooldown;
        public int _chargeTimer;

        private SpriteMap sprite;
        private Sprite freezeBit;
        private Tex2D beam;
        private Tex2D laserTexture;

        public FreezeRay(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor name
            _editorName = "Freeze Ray";

            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\freezeRay"), 17, 14);
            graphic = sprite;
            _center = new Vec2(9f, 7f);
            _collisionSize = new Vec2(16f, 14f);
            _collisionOffset = new Vec2(-9f, -7f);
            _holdOffset = new Vec2(1f, -1f);
            _barrelOffsetTL = new Vec2(16f, 4f);

            // weapon settings
            ammo = 1;
            _weight = 3f;

            // defaults
            freezeBit = new Sprite(Mod.GetPath<UffMod>("weapons\\freezeBit"));
            freezeBit.CenterOrigin();
            beam = Content.Load<Tex2D>(Mod.GetPath<UffMod>("weapons\\freezeBeam.png"));
            laserTexture = Content.Load<Tex2D>("pointerLaser");
        }

        public override void Update()
        {
            if (owner != null)
            {
                float ang = angle + (offDir < 0 ? (float)Math.PI : 0f);

                foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(Offset(barrelOffset) + 90f * new Vec2((float)Math.Cos(ang), (float)Math.Sin(ang)), 90f))
                {
                    if (physicsObject != this
                        && physicsObject != owner
                        && physicsObject.owner == null
                        && physicsObject.solid
                        && physicsObject.thickness > 0.4f
                        && !(physicsObject is IceCubeUFFEdition)
                        && !(physicsObject is Equipment)
                        && !(physicsObject is Gun)
                        && (physicsObject is Duck || physicsObject is RagdollPart || physicsObject is TrappedDuck
                        || (physicsObject.collisionSize.length <= 256f
                        && physicsObject.collisionSize.x <= 26f
                        && physicsObject.collisionSize.y <= 26f))
                        && Level.CheckLine<Block>(Offset(barrelOffset), physicsObject.position, physicsObject) == null
                        && (_target == null || (Offset(barrelOffset) - physicsObject.position).length < (Offset(barrelOffset) - _target.position).length))
                        _target = physicsObject;
                }

                if (_target != null)
                {
                    if (!Level.CheckCircleAll<PhysicsObject>(Offset(barrelOffset) + 90f * new Vec2((float)Math.Cos(ang), (float)Math.Sin(ang)), 90f).Contains(_target) || Level.CheckLine<Block>(Offset(barrelOffset), _target.position, _target) != null)
                    {
                        _target = null;
                        _chargeTimer = 0;
                    }
                    else
                    {
                        if (_chargeTimer >= 0 && !_coolingDown)
                        {
                            if (_charging)
                            {
                                if (_chargeTimer < 30)
                                {
                                    if (isServerForObject && _chargeTimer % 4 == 0)
                                        netSFX_charge.Play();
                                    _chargeTimer++;
                                }
                                else
                                {
                                    if (isServerForObject)
                                    {
                                        netSFX_freeze.Play();
                                        _drawPosition = _target.position;
                                        Chill(_target.position);
                                        Freeze(_target);
                                        _target = null;
                                    }
                                    _coolingDown = true;
                                    _chargeTimer = 0;
                                }
                            }
                            else
                                _chargeTimer = 0;
                        }
                    }
                }
                else
                    _chargeTimer = 0;
            }
            else
            {
                _charging = false;
                if (_chargeTimer > 0)
                    _chargeTimer = 0;
            }

            if (_coolingDown)
            {
                if (_cooldown < 30)
                    _cooldown++;
                else
                {
                    _cooldown = 0;
                    _coolingDown = false;
                }
            }

            base.Update();
        }

        private void Chill(Vec2 pos)
        {
            for (float f = Rando.Float(30f); f < 360f; f += 60)
            {
                GlobalSteam iceS = new GlobalSteam(pos.x, pos.y, Rando.Float(24f, 32f));
                Level.Add(iceS);
                iceS.xscale = iceS.yscale = Rando.Float(0.4f, 0.6f);
                float f2 = Rando.Float(0.4f, 1.2f);
                iceS.hSpeed = f2 * (float)Math.Cos(Maths.DegToRad(f));
                iceS.vSpeed = f2 * (float)Math.Sin(Maths.DegToRad(f));
            }
        }

        private void Freeze(PhysicsObject physicsObject)
        {
            Duck hitDuck = physicsObject as Duck;
            RagdollPart ragdollPart = physicsObject as RagdollPart;
            TrappedDuck trappedDuck = physicsObject as TrappedDuck;
            if (hitDuck != null)
            {
                Vec2 icePos = hitDuck.position;
                hitDuck.Kill(new DTImpact(this));
                Level.Add(new IceCubeUFFEdition(icePos.x, icePos.y));
                Fondle(hitDuck);
                if (hitDuck.ragdoll != null)
                    Level.Remove(hitDuck.ragdoll);
                Level.Remove(hitDuck);
            }
            else if (ragdollPart != null)
            {
                if (!ragdollPart._doll._duck.dead)
                    ragdollPart._doll._duck.Kill(new DTImpact(this));
                Level.Add(new IceCubeUFFEdition(ragdollPart.x, ragdollPart.y));
                Fondle(ragdollPart._doll);
                Level.Remove(ragdollPart._doll);
            }
            else if (trappedDuck != null)
            {
                Level.Add(new IceCubeUFFEdition(trappedDuck.x, trappedDuck.y));
                if (!trappedDuck.captureDuck.dead)
                    trappedDuck.captureDuck.Kill(new DTImpact(this));
                Fondle(trappedDuck);
                if (trappedDuck.captureDuck.ragdoll != null)
                    Level.Remove(trappedDuck.captureDuck.ragdoll);
                Level.Remove(trappedDuck);
            }
            else
            {
                Fondle(physicsObject);
                Level.Add(new IceCubeUFFEdition(physicsObject.x, physicsObject.y));
                Level.Remove(physicsObject);
            }
        }

        public override void OnReleaseAction()
        {
            _charging = false;
        }

        public override void OnPressAction()
        {
            if (owner != null)
                _charging = true;
        }

        public override void Fire()
        {
            // do nothing
        }

        public override void Draw()
        {
            if (_coolingDown)
            {
                float fade = 1f - (_cooldown / 30f);
                freezeBit.alpha = fade;
                Sprite barrelBit = freezeBit;
                barrelBit.xscale = barrelBit.yscale = 0.75f;
                Graphics.Draw(barrelBit, Offset(barrelOffset).x, Offset(barrelOffset).y, 0.7f);
                Graphics.Draw(freezeBit, _drawPosition.x, _drawPosition.y, 0.7f);
                Graphics.DrawTexturedLine(beam, Offset(barrelOffset), _drawPosition, Color.White * fade, 0.3f, 0.6f);
            }
            base.Draw();
        }

        public override void DrawGlow()
        {
            if (owner != null && _target != null && !_coolingDown)
            {
                Graphics.DrawTexturedLine(laserTexture, Offset(barrelOffset), _target.position, Color.Cyan, 0.2f + 0.016f * _chargeTimer, depth - 1);
                _sightHit.color = Color.Cyan;
                Sprite barrelDot = _sightHit;
                barrelDot.xscale = barrelDot.yscale = 0.75f;
                Graphics.Draw(barrelDot, Offset(barrelOffset).x, Offset(barrelOffset).y);
                Graphics.Draw(_sightHit, _target.x, _target.y);
            }
            base.DrawGlow();
        }
    }
}
