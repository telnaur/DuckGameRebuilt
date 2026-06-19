using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.UFFMod
{
    internal class IceFlash : Thing
    {
        public StateBinding _theRayStateBinding = new StateBinding("_theRay");

        public FreezeRay _theRay;

        private SpriteMap sprite;
        private float tSpeed;
        private float tAngle;
        private int time;

        public IceFlash(float xpos, float ypos, float spe, float ang, FreezeRay fr)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\iceFlash"), 8, 8);
            sprite.AddAnimation("todust", 0.5f, false, 0, 1, 2, 3);
            sprite.SetAnimation("todust");
            graphic = sprite;
            center = new Vec2(4f, 4f);
            depth = 0.5f;
            tSpeed = spe;
            tAngle = ang;
            _theRay = fr;
        }

        public override void Update()
        {
            position = _theRay.Offset(_theRay.barrelOffset);
            x += time * tSpeed * (float)Math.Cos((_theRay.offDir > 0 ? _theRay.angle : _theRay.angle + (float)Math.PI) + tAngle);
            y += time * tSpeed * (float)Math.Sin((_theRay.offDir > 0 ? _theRay.angle : _theRay.angle + (float)Math.PI) + tAngle);
            time++;
            if (sprite.finished)
                Level.Remove(this);
        }
    }
}
