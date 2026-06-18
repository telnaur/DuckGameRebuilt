using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.UFFMod
{
    [BaggedProperty("canSpawn", false)]
    public class BalloonMineLauncher : Gun
    {
        private SpriteMap sprite;

        public BalloonMineLauncher(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor name
            _editorName = "Special Delivery";

            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\balloonMineLauncher"), 18, 13);
            graphic = sprite;
            _center = new Vec2(9f, 6f);
            _collisionSize = new Vec2(16f, 13f);
            _collisionOffset = new Vec2(-8f, -6f);
            _holdOffset = new Vec2(1f, -2f);
            _barrelOffsetTL = new Vec2(17f, 5f);

            // weapon settings
            ammo = 6;
            _fireSound = Mod.GetPath<UffMod>("SFX\\lob.wav");
            _kickForce = 1.2f;
        }

        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                ammo--;
                PlayFireSound();
                ApplyKick();

                Balloon b = new Balloon(Offset(barrelOffset + new Vec2(4f, 0)).x, Offset(barrelOffset + new Vec2(4f, 0)).y, 6f, offDir > 0 ? angle : angle + (float)Math.PI, offDir > 0 ? angle + (float)Math.PI / 2f : angle - (float)Math.PI / 2f, typeof(AerialMine));
                if (duck != null)
                    b.clip.Add(duck);
                if (isServerForObject)
                    Level.Add(b);
            }
            else
                DoAmmoClick();
        }

        public override void Fire()
        {
            // do nothing
        }
    }
}
