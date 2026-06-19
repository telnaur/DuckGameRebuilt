using System;

namespace DuckGame.UFFMod
{
    // black hole particles
    internal class Graviton : Thing
    {
        /*
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _theCubeStateBinding = new StateBinding("_theCube");
        public StateBinding _timerStateBinding = new StateBinding("_timer");
        public StateBinding _maxTimeStateBinding = new StateBinding("_maxTime");
        public StateBinding _glowStateBinding = new StateBinding("_glow");
        public StateBinding _isDarkeningStateBinding = new StateBinding("_isDarkening");
        */

        public DarkMatterCube _theCube;
        public int _timer;
        public float _maxTime;
        public float _glow;
        public bool _isDarkening;

        public Graviton(float xpos, float ypos, DarkMatterCube theCube, float maxTime)
            : base(xpos, ypos)
        {
            _theCube = theCube;
            depth = 0.98f;
            alpha = 0f;
            _maxTime = maxTime;
            _glow = 0f;
        }

        public override void Update()
        {
            if (_timer >= _maxTime || _theCube == null)
                Level.Remove(this);

            if (_theCube != null)
            {
                if (_timer == 1)
                    _theCube.EnableSpawn();

                Vec2 drawPoint = _theCube.Offset(_theCube.barrelOffset);
                float angleToObject = (float)Math.Atan2(drawPoint.y - y, drawPoint.x - x);
                float distanceToObject = (drawPoint - position).length;
                float division = (float)Math.Pow(_maxTime - _timer, 1.15);
                x += (distanceToObject / (division > 1f ? division : 1f)) * (float)Math.Cos(angleToObject);
                y += (distanceToObject / (division > 1f ? division : 1f)) * (float)Math.Sin(angleToObject);
            }

            if (!_isDarkening)
            {
                if (_glow <= 0.05f)
                    _glow += 0.005f;
                else
                {
                    _glow -= 0.005f;
                    _isDarkening = true;
                }
            }
            else
            {
                if (_glow > 0f)
                    _glow -= 0.005f;
                else
                {
                    _glow += 0.005f;
                    _isDarkening = false;
                }
            }
            alpha = _timer / _maxTime;
            _timer++;
        }

        public override void Draw()
        {
            
            Graphics.DrawRect(position - new Vec2(1f, 1f), position + new Vec2(1f, 1f), new Color(_glow, _glow, _glow, alpha), depth);
        }
    }
}
