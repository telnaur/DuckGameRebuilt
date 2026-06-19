using System;

namespace DuckGame.UFFMod
{
    internal class GlobalSteam : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _xscaleStateBinding = new StateBinding("xscale");
        public StateBinding _yscaleStateBinding = new StateBinding("yscale");
        public StateBinding _startTimeStateBinding = new StateBinding("_startTime");
        public StateBinding _timeStateBinding = new StateBinding("_time");
        public StateBinding alphaStateBinding = new StateBinding("alpha");

        public float _startTime;
        public float _time;

        private SpriteMap sprite;

        public GlobalSteam(float xpos, float ypos, float time)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\iceSteam"), 32, 32);
            graphic = sprite;
            angle = Rando.Float(2 * (float)Math.PI);
            center = new Vec2(16f, 16f);
            depth = 0.7f;
            _startTime = _time = time;
        }

        public override void Update()
        {
            alpha = _time / _startTime;
            x += hSpeed;
            y += vSpeed;
            _time--;
            if (_time <= 0)
                Level.Remove(this);
        }
    }
}
