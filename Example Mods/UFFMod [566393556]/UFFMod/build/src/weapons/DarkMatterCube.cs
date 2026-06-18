using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|explosives")]
    [BaggedProperty("isSuperWeapon", true)]
    public class DarkMatterCube : Gun
    {
        public StateBinding _hasSpawnedBlackHoleStateBinding = new StateBinding("_hasSpawnedBlackHole");
        public StateBinding _hasFiredStateBinding = new StateBinding("_hasFired");
        public StateBinding _timerStateBinding = new StateBinding("_timer");
        public StateBinding _spriteSpeedStateBinding = new StateBinding("_spriteSpeed");
        public StateBinding netSFX_chargeStateBinding = new NetSoundBinding("netSFX_charge");
        public StateBinding netSFX_whargarbleStateBinding = new NetSoundBinding("netSFX_whargarble");

        public NetSoundEffect netSFX_charge = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\charge")
        });
        public NetSoundEffect netSFX_whargarble = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\whargarble")
        });

        public bool _hasSpawnedBlackHole;
        public bool _hasFired;
        public int _timer;
        public float _spriteSpeed;

        private SpriteMap sprite;
        private bool localSpawned;

        public DarkMatterCube(float xpos, float ypos) :
            base(xpos, ypos)
        {
            _editorName = "Dark Matter Cube";

            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\darkMatterCube"), 10, 13);
            sprite.AddAnimation("inactive", 0.1f, true, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            sprite.AddAnimation("active", 1f, true, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23);
            sprite.SetAnimation("inactive");
            graphic = sprite;
            center = new Vec2(5f, 6f);
            _collisionOffset = new Vec2(-5f, -6f);
            _collisionSize = new Vec2(10f, 13f);
            _barrelOffsetTL = new Vec2(5f, -2f);
            _holdOffset = new Vec2(-1f, -1f);

            //defaults
            ammo = 1;
        }

        public override void Terminate()
        {
            // hey guess what fella THERE'S NOTHING HERE
        }

        public void EnableSpawn()
        {
            localSpawned = false;
        }

        public override void Update()
        {
            base.Update();

            if (_timer > 1)
                _timer--;

            if (_timer <= 121 && _timer > 11 && !localSpawned)
            {
                float randomAngle = Rando.Float(2f * (float)Math.PI);
                Level.Add(new Graviton(x + Rando.Float(96f, 128f) * (float)Math.Cos(randomAngle), y + Rando.Float(96f, 128f) * (float)Math.Sin(randomAngle), this, (_timer - Rando.Int(10))));
                localSpawned = true;
            }
            if (_timer == 1 && !_destroyed)
            {
                foreach (BlackHole bh in Level.current.things[typeof(BlackHole)])
                    if (bh._theCube != null && bh._theCube == this)
                        _hasSpawnedBlackHole = true;

                // spawn black hole
                if (isServerForObject && !_hasSpawnedBlackHole)
                {
                    GlobalDarkpulse darkpulse = new GlobalDarkpulse(x, y);
                    Level.Add(darkpulse);
                    Fondle(darkpulse);
                    netSFX_whargarble.Play();

                    BlackHole blackHole = new BlackHole(x, y, dmc: this, timer: 120);
                    blackHole._released = true;
                    Level.Add(blackHole);
                    Fondle(blackHole);

                    _hasSpawnedBlackHole = true;
                }

                // implode
                foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(position, 96f))
                {
                    if (physicsObject.owner == null)
                        Fondle(physicsObject);
                    Vec2 propulsion = (physicsObject.position - position).normalized * -6f;
                    physicsObject.hSpeed = propulsion.x;
                    physicsObject.vSpeed = propulsion.y;

                    // ragdoll hit ducks
                    if (physicsObject is Duck)
                    {
                        Duck theDuck = physicsObject as Duck;
                        if (theDuck.isServerForObject)
                        {
                            Holdable heldItem = theDuck.holdObject;
                            if (heldItem != null)
                            {
                                theDuck.ThrowItem(false);
                                physicsObject.vSpeed -= 4f;
                                physicsObject.hSpeed = theDuck.hSpeed * 0.8f;
                                physicsObject.clip.Add(theDuck);
                                theDuck.clip.Add(heldItem);
                            }
                            theDuck.GoRagdoll();
                            if (heldItem != null)
                            {
                                theDuck.ragdoll.part1.clip.Add(heldItem);
                                theDuck.ragdoll.part2.clip.Add(heldItem);
                                theDuck.ragdoll.part3.clip.Add(heldItem);
                                heldItem.clip.Add(theDuck.ragdoll.part1);
                                heldItem.clip.Add(theDuck.ragdoll.part2);
                                heldItem.clip.Add(theDuck.ragdoll.part3);
                            }
                        }
                    }
                }

                ammo = 0;
                _destroyed = true;
                Level.Remove(this);
            }

            float time = _timer / 100f;
            float timerSpeed = time - 0.6f;
            _spriteSpeed = 1f - (timerSpeed < 0f ? 0f : timerSpeed) / 2.25f;
        }

        public override void Draw()
        {
            sprite.speed = _spriteSpeed;

            base.Draw();
        }

        public override void OnPressAction()
        {
            if (!_hasFired)
            {
                _timer = 201;
                if (isServerForObject) // siren sfx
                    netSFX_charge.Play();
                sprite.SetAnimation("active");
                _hasFired = true;
            }
        }
    }
}
