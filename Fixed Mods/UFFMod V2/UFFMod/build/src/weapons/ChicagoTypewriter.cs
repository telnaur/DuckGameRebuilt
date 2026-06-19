using System;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|guns")]
    [BaggedProperty("isSuperWeapon", true)]
    public class ChicagoTypewriter : Gun
    {
        public StateBinding _accuracyLostStateBinding = new StateBinding("_accuracyLost");
        public StateBinding _flareAlphaStateBinding = new StateBinding("_flareAlpha");
        public StateBinding netSFX_fireStateBinding = new NetSoundBinding("netSFX_fire");

        public NetSoundEffect netSFX_fire = new NetSoundEffect(new string[1]
        {
            "pistol"
        });

        private SpriteMap sprite;

        public ChicagoTypewriter(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // editor name
            _editorName = "Chicago Typewriter";

            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\chicagoTypewriter"), 26, 13);
            graphic = sprite;
            _center = new Vec2(13f, 7f);
            _collisionSize = new Vec2(26f, 11f);
            _collisionOffset = new Vec2(-13f, -7f);
            _holdOffset = new Vec2(-4f, 3f);
            _barrelOffsetTL = new Vec2(25f, 3f);

            // weapon settings
            ammo = 50;
            _kickForce = 1.2f;
            _fullAuto = true;
            loseAccuracy = 0.12f;
            maxAccuracyLost = 0.6f;
        }

        public override void Fire()
        {
            if (_wait == 0f)
            {
                if (ammo > 0)
                {
                    ammo--;
                    ApplyKick();

                    if (isServerForObject)
                    {
                        netSFX_fire.Play();

                        float ballAngle = barrelAngle;
                        BilliardBall billiardBall = new BilliardBall(Offset(barrelOffset).x, Offset(barrelOffset).y, -ballAngle, owner);
                        Vec2 ballVector = Maths.AngleToVec(ballAngle);
                        float speedMod = 4f + Rando.Float(4f);
                        billiardBall.hSpeed = ballVector.x * speedMod;
                        billiardBall.vSpeed = ballVector.y * speedMod;

                        if (duck != null)
                            billiardBall.clip.Add(duck);

                        Level.Add(billiardBall);

                        _flareAlpha = 1.5f;
                        _accuracyLost += loseAccuracy;
                        if (_accuracyLost > maxAccuracyLost)
                            _accuracyLost = maxAccuracyLost;
                    }
                }
                else
                    DoAmmoClick();
                _wait = 0.60f;
            }
        }
    }
}
