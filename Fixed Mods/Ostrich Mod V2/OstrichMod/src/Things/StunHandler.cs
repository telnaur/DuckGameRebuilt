using System;

namespace DuckGame.OstrichMod
{
    public class StunHandler : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _stunTargetStateBinding = new StateBinding("_stunTarget");
        public StateBinding _stunTimeStateBinding = new StateBinding("_stunTime");
        public StateBinding _showDazeStateBinding = new StateBinding("_showDaze");
        public StateBinding _overriderStateBinding = new StateBinding("_overrider");

        public Duck _stunTarget;
        public int _stunTime;
        public bool _showDaze;
        public bool _overrider;

        private bool _fixH;
        private bool _fixV;

        private Sprite swirl;

        public StunHandler(Duck stunTarget, int stunTime = 10, bool fixH = false, bool fixV = false, bool showDaze = false)
        {
            swirl = new Sprite("swirl", 0f, 0f);
            swirl.CenterOrigin();
            swirl.scale = new Vec2(0.75f, 0.75f);
            _stunTarget = stunTarget;
            _stunTime = stunTime;
            _fixH = fixH;
            _fixV = fixV;
            _showDaze = showDaze;
            foreach (Thing t in Level.current.things)
                if (t is StunHandler && t != this && ((StunHandler)t)._stunTarget == stunTarget && !((StunHandler)t)._overrider)
                {
                    if (showDaze)
                        ((StunHandler)t)._showDaze = true;
                    if (stunTime > ((StunHandler)t)._stunTime)
                        ((StunHandler)t)._stunTime = stunTime;
                    _overrider = true;
                }
        }

        public override void Update()
        {
            if (_overrider || _stunTime <= 0 || _stunTarget == null)
            {
                if (!_overrider && _stunTarget != null)
                    _stunTarget.immobilized = false;
                Level.Remove(this);
                return;
            }

            position = _stunTarget.position - new Vec2(0f, 12f);

            _stunTarget.hSpeed = _fixH ? 0f : _stunTarget.hSpeed;
            _stunTarget.vSpeed = _fixV ? 0f : _stunTarget.vSpeed;
            _stunTarget.immobilized = true;

            swirl.angle = (swirl.angle > 2f * (float)Math.PI ? swirl.angle - 2f * (float)Math.PI : swirl.angle) + 0.2f;

            _stunTime--;

            base.Update();
        }

        public override void Draw()
        {
            if (!_overrider && _showDaze)
                Graphics.Draw(swirl, position.x, position.y);
            base.Draw();
        }
    }
}
