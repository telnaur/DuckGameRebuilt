using System;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|tech")]
    [BaggedProperty("isSuperWeapon", true)]
    public class PlasmaThruster : Gun
    {
        public StateBinding _flareAlphaStateBinding = new StateBinding("_flareAlpha");
        public StateBinding _speedModStateBinding = new StateBinding("_speedMod");
        public StateBinding _plasmaFlareStateBinding = new StateBinding("_plasmaFlare");
        public StateBinding _animationIndexStateBinding = new StateBinding("netAnimationIndex");
        public StateBinding _frameStateBinding = new StateBinding("spriteFrame");
        public StateBinding netSFX_fireStateBinding = new NetSoundBinding("netSFX_fire");
        public StateBinding netSFX_clickStateBinding = new NetSoundBinding("netSFX_click");

        public NetSoundEffect netSFX_fire = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\alien")
        });
        public NetSoundEffect netSFX_click = new NetSoundEffect(new string[1]
       {
            "click"
       });

        public float _speedMod;
        public int _plasmaFlare;

        private SpriteMap sprite;

        private byte netAnimationIndex
        {
            get
            {
                if (sprite == null)
                    return 0;
                return (byte)sprite.animationIndex;
            }
            set
            {
                if (sprite == null || sprite.animationIndex == (int)value)
                    return;
                sprite.animationIndex = (int)value;
            }
        }

        public byte spriteFrame
        {
            get
            {
                if (sprite == null)
                    return 0;
                return (byte)sprite._frame;
            }
            set
            {
                if (sprite == null)
                    return;
                sprite._frame = (int)value;
            }
        }

        public PlasmaThruster(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // editor name
            _editorName = "Plasma Thruster";

            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\plasmaThruster"), 20, 20);
            sprite.AddAnimation("normal", 1f, false, 0);
            sprite.AddAnimation("blast", 1f, true, 2, 3);
            sprite.SetAnimation("normal");
            graphic = sprite;
            _center = new Vec2(10f, 10f);
            _collisionSize = new Vec2(20f, 20f);
            _collisionOffset = new Vec2(-10f, -10f);
            _holdOffset = new Vec2(4f, 0f);

            // weapon settings
            ammo = 90;
            _kickForce = 0.6f;
            _fullAuto = true;
            weight = 8f;
        }

        public override void Update()
        {
            _speedMod = MathHelper.Lerp(_speedMod, 0f, 0.04f);

            if (_plasmaFlare <= 2)
                sprite.SetAnimation("normal");

            base.Update();
        }

        public override void Fire()
        {
            if (_wait == 0f)
            {
                if (ammo > 0)
                {
                    if (_speedMod < 3.2f)
                        _speedMod = MathHelper.Lerp(_speedMod, 3.2f, 0.35f);

                    ammo--;
                    ApplyKick();
                    if (duck != null && (offDir > 0 ? duck.hSpeed > -9.6f : duck.hSpeed < 9.6f))
                    {
                        float newHSpeed = duck.hSpeed - (offDir * _speedMod);
                        if (offDir > 0 ? newHSpeed < -9.6f : newHSpeed > 9.6f)
                            duck.hSpeed = offDir > 0 ? -9.6f : 9.6f;
                        else
                            duck.hSpeed -= offDir * _speedMod;
                    }

                    if (isServerForObject)
                    {
                        netSFX_fire.Play();

                        _plasmaFlare = 4;

                        foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(topLeft + new Vec2(offDir > 0 ? 20f : -32f, 0f), bottomRight + new Vec2(offDir > 0 ? 32f : -20f, 0f)))
                            if (Level.CheckLine<Block>(position, physicsObject.position, physicsObject) == null && physicsObject.solid && physicsObject.thickness > 0f && !(physicsObject is Gun) && !(physicsObject is Equipment) && !(physicsObject is RagdollPart) && physicsObject != owner)
                            {
                                if (physicsObject.owner == null)
                                    Fondle(physicsObject);
                                physicsObject.Destroy(new DTIncinerate(this));
                            }
                    }

                    sprite.SetAnimation("blast");
                }
                else if (isServerForObject)
                    netSFX_click.Play();
                _wait = 0.45f;
            }
        }

        public override void Draw()
        {
            if (_plasmaFlare > 0)
            {
                SpriteMap flareSprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\plasmaThrusterFlare"), 32, 24);
                flareSprite.frame = 4 - _plasmaFlare;
                flareSprite.CenterOrigin();
                flareSprite.flipH = offDir < 0;
                Graphics.Draw(flareSprite, x + offDir * 26f, y);
                _plasmaFlare--;
            }
            base.Draw();
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner == null)
                return;
            duck.holdObstructed = false;
        }
    }
}
