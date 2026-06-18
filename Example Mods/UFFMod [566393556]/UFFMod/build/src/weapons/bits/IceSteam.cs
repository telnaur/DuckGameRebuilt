using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.UFFMod
{
    internal class IceSteam : Thing
    {
        private SpriteMap sprite;
        private float startTime;
        private float _time;

        public IceSteam(float xpos, float ypos, float time)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\iceSteam"), 32, 32);
            graphic = sprite;
            angle = Rando.Float(2 * (float)Math.PI);
            center = new Vec2(16f, 16f);
            depth = 0.7f;
            startTime = _time = time;
        }

        public override void Update()
        {
            alpha = _time / startTime;
            x += hSpeed;
            y += vSpeed;
            _time--;
            if (_time <= 0)
                Level.Remove(this);
        }
    }
}
