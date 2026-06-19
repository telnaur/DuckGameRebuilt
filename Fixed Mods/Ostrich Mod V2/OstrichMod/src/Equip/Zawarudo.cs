using System;

namespace DuckGame.OstrichMod
{
    public class Zawarudo : Thing
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

        public Zawarudo(Duck stunTarget, int stunTime = 10, bool fixH = false, bool fixV = false, bool showDaze = false)
        {
            swirl = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("ZawarudoState"), 0f, 0f);
            swirl.CenterOrigin();
            swirl.scale = new Vec2(0.75f, 0.75f);
            _stunTarget = stunTarget;
            _stunTime = stunTime;
            _fixH = fixH;
            _fixV = fixV;
            _showDaze = showDaze;
            // Cancel out duplicates that target the same duck: fold our values into the
            // existing one and mark ourselves as overrider so we self-remove next Update.
            // (Was checking StunHandler, a different type that never matched here.)
            foreach (Thing t in Level.current.things)
                if (t is Zawarudo && t != this && ((Zawarudo)t)._stunTarget == stunTarget && !((Zawarudo)t)._overrider)
                {
                    if (showDaze)
                        ((Zawarudo)t)._showDaze = true;
                    if (stunTime > ((Zawarudo)t)._stunTime)
                        ((Zawarudo)t)._stunTime = stunTime;
                    _overrider = true;
                }
        }

        public override void Update()
        {
            if (_overrider || _stunTime <= 0 || _stunTarget == null)
            {
                // Restore the target only if it still exists and we aren't an
                // overridden duplicate (overriders must not touch the shared target).
                if (!_overrider && _stunTarget != null)
                {
                    _stunTarget.immobilized = false;
                    _stunTarget.gravMultiplier = 1f;
                    _stunTarget.vMax = 8f;
                }
                Layer.Blocks.colorMul = new Vec3(1f, 1f, 1f);
                Level.Remove(this);
                return;
            }

            position = _stunTarget.position - new Vec2(0f, 12f);

            _stunTarget.hSpeed = _fixH ? 0f : _stunTarget.hSpeed;
            _stunTarget.vSpeed = _fixV ? 0f : _stunTarget.vSpeed;
            _stunTarget.vMax = 0f;
            _stunTarget.gravMultiplier = 0f;
            _stunTarget.immobilized = true;


            _stunTime--;

            base.Update();
        }

        public override void Draw()
        {
            if (!_overrider && _showDaze)
                Graphics.Draw(swirl, position.x, position.y);
            base.Draw();
        }

        public override void Removed()
        {
            // Called both when the server's Zawarudo removes itself and when the
            // network manager removes the ghost on remote clients. Without this,
            // remote clients' ducks stay permanently immobilized/frozen because
            // the cleanup in Update() is never reached via the ghost removal path.
            if (!_overrider && _stunTarget != null)
            {
                _stunTarget.immobilized = false;
                _stunTarget.gravMultiplier = 1f;
                _stunTarget.vMax = 8f;
            }
            Layer.Blocks.colorMul = new Vec3(1f, 1f, 1f);
            base.Removed();
        }
    }
}
