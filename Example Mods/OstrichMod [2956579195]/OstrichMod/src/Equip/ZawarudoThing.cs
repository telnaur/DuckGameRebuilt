using System;

namespace DuckGame.OstrichMod
{
    public class ZawarudoThing : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _stunTargetStateBinding = new StateBinding("_stunTarget");
        public StateBinding _stunTimeStateBinding = new StateBinding("_stunTime");
        public StateBinding _showDazeStateBinding = new StateBinding("_showDaze");
        public StateBinding _overriderStateBinding = new StateBinding("_overrider");

        public Holdable _stunTarget;
        public int _stunTime;
        public bool _showDaze;
        public bool _overrider;

        private bool _fixH;
        private bool _fixV;

        private Sprite swirl;

        public ZawarudoThing(Holdable stunTarget, int stunTime = 10, bool fixH = false, bool fixV = false, bool showDaze = false)
        {
            _stunTarget = stunTarget;
            _stunTime = stunTime;
            _fixH = fixH;
            _fixV = fixV;
            _showDaze = showDaze;
        }

        public override void Update()
        {
            if (_overrider || _stunTime <= 0 || _stunTarget == null)
            {
                if (!_overrider && _stunTarget != null)
                _stunTarget.gravMultiplier = 1f;
                _stunTarget.vMax = 8f;
                Level.Remove(this);
                return;
            }

            position = _stunTarget.position - new Vec2(0f, 12f);

            _stunTarget.hSpeed = _fixH ? 0f : _stunTarget.hSpeed;
            _stunTarget.vSpeed = _fixV ? 0f : _stunTarget.vSpeed;
            _stunTarget.vMax = 0f;
            _stunTarget.gravMultiplier = 0f;

            _stunTime--;

            base.Update();
        }

    }
}
