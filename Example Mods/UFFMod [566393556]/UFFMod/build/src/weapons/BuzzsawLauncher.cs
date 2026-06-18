using System;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|misc")]
    internal class BuzzsawLauncher : Gun
    {
        public StateBinding _resetDuckStateBinding = new StateBinding("_resetDuck");
        public StateBinding _gunStateStateBinding = new StateBinding("_gunState");
        public StateBinding _forceStateBinding = new StateBinding("_force");
        public StateBinding _pitchStateBinding = new StateBinding("pitch");
        public StateBinding _volumeStateBinding = new StateBinding("volume");
        public StateBinding netSFX_buzzsawFireStateBinding = new NetSoundBinding("netSFX_buzzsawFire");

        public NetSoundEffect netSFX_buzzsawFire = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\buzzsawFire")
        });

        public Duck _resetDuck;
        public int _gunState;
        public float _force;
        private SpriteMap sprite;
        private float pitch;
        private float volume;
        private Sound noise;

        public BuzzsawLauncher(float xval, float yval)
          : base(xval, yval)
        {
            // buzzsaw launcher
            // hold to charge weapon

            // editor name
            _editorName = "Buzzsaw Launcher";

            // sprite and collisions
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\sawLauncher"), 31, 20);
            sprite.AddAnimation("idle", 1f, false, 0);
            sprite.AddAnimation("spin1", 0.2f, true, 0, 1, 2, 3);
            sprite.AddAnimation("spin2", 0.55f, true, 0, 1, 2, 3);
            sprite.AddAnimation("spin3", 0.8f, true, 0, 1, 2, 3);
            sprite.AddAnimation("empty", 1f, false, 4);
            graphic = sprite;
            _center = new Vec2(15f, 10f);
            _barrelOffsetTL = new Vec2(20f, 10f);
            _collisionSize = new Vec2(23f, 9f);
            _collisionOffset = new Vec2(-15f, -2f);
            _holdOffset = new Vec2(5f, -2f);

            // weapon settings
            ammo = 3;
            _kickForce = 3f;
            _force = 0f;
        }

        public override void Terminate()
        {
            if (noise != null)
            {
                noise.Stop();
                noise = null;
            }
            base.Terminate();
        }

        public override void Update()
        {
            if (_resetDuck != null)
            {
                _resetDuck.frictionMult = 1f;
                _resetDuck = null;
            }

            base.Update();

            _force -= (_force > 0f && _gunState == 0f) ? 0.03f : 0f; // reduce to 0 if not charging

            // sound is limited so that it wouldn't break the game
            pitch = (_force / 3f < 1f) ? (_force / 3f) : 1f;
            volume = (_force / 3f < 0.5f) ? (_force / 3f) : 0.5f;
            _kickForce = 3f * (_force * 0.75f); // adjust kicking force to charging force

            if (owner != null && _gunState == 1)
            {
                if (noise == null)
                    noise = SFX.Play(Mod.GetPath<UffMod>("SFX\\buzzsawNoise"), volume, pitch, looped: true);
                else
                {
                    noise.Volume = volume;
                    noise.Pitch = pitch;
                }
                foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(Offset(barrelOffset), 10f))
                {
                    if (materialThing == owner)
                        continue;

                    if (materialThing is Duck)
                        materialThing.Destroy(new DTImpale(this));
                    else if (materialThing is RagdollPart)
                        ((RagdollPart)materialThing)._doll._duck.Destroy(new DTImpale(this));
                    if (materialThing.Hurt(materialThing is Door ? 1.8f : 0.5f))
                    {
                        pitch = 0.25f;
                        if (duck != null)
                        {
                            duck.frictionMult = 4f;
                            _resetDuck = duck;
                        }
                        Vec2 vec2_1 = Collision.LinePoint(Offset(barrelOffset) - barrelVector * 5f, Offset(barrelOffset) + barrelVector * 5f, materialThing.rectangle);
                        if (vec2_1 != Vec2.Zero)
                        {
                            Vec2 vec2_2 = vec2_1 + barrelVector * Rando.Float(0.0f, 3f);
                            Vec2 vec2_3 = -barrelVector.Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero);
                            if (materialThing.physicsMaterial == PhysicsMaterial.Wood)
                            {
                                Thing thing = WoodDebris.New(vec2_2.x, vec2_2.y);
                                thing.hSpeed = vec2_3.x * 3f;
                                thing.vSpeed = vec2_3.y * 3f;
                                Level.Add(thing);
                            }
                            else if (materialThing.physicsMaterial == PhysicsMaterial.Metal)
                            {
                                Thing thing = Spark.New(vec2_2.x, vec2_2.y, Vec2.Zero, 0.02f);
                                thing.hSpeed = vec2_3.x * 3f;
                                thing.vSpeed = vec2_3.y * 3f;
                                Level.Add(thing);
                            }
                        }
                    }
                }
            }
            else if (noise != null)
            {
                noise.Stop();
                noise = null;
            }
        }

        public override void Draw()
        {
            if (ammo > 0) {
                if (_force < 0.1f) sprite.SetAnimation("idle");
                else if (_force >= 0.1f && _force < 0.8f) sprite.SetAnimation("spin1");
                else if (_force >= 0.8f && _force < 1.6f) sprite.SetAnimation("spin2");
                else if (_force >= 1.6f && _force < 2.5f) sprite.SetAnimation("spin3");
            }
            else
                sprite.SetAnimation("empty");

            base.Draw();
        }

        public override void Thrown()
        {
            _gunState = 0;
            base.Thrown();
        }

        public override void Fire()
        {
            // nothing
        }

        public override void OnPressAction()
        {
            _gunState = 1;
        }

        public override void OnHoldAction()
        {
            if (ammo > 0 && _force < 2.5f)
                _force += 0.06f;
        }

        public override void OnReleaseAction()
        {
            if (_gunState == 1 && ammo > 0)
            {
                if (isServerForObject)
                {
                    var blade = new BuzzsawBlade(Offset(barrelOffset).x, Offset(barrelOffset).y);
                    float finalForce = offDir * _force * 7f;
                    blade.hSpeed = finalForce * (float)Math.Cos(angle);
                    blade.vSpeed = finalForce * (float)Math.Sin(angle);
                    blade.offDir = offDir;
                    blade.force = _force;
                    Level.Add(blade);
                    netSFX_buzzsawFire.Play();
                }
                ApplyKick();
                ammo--;
                _force = 0f;
                _gunState = 0;
            }
        }
    }
}