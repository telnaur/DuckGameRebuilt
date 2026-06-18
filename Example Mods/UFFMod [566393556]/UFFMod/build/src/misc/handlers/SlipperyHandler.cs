using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.UFFMod
{
    public class SlipperyHandler : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _slipperyTargetStateBinding = new StateBinding("_slipperyTarget");

        public PhysicsObject _slipperyTarget;
        public bool hasUpdated;
        private bool hasInitialized;

        public SlipperyHandler(PhysicsObject slipperyTarget)
        {
            _slipperyTarget = slipperyTarget;
            hasUpdated = true;
            hasInitialized = false;
        }

        public override void Update()
        {
            if (_slipperyTarget == null)
                Level.Remove(this);

            if (!hasUpdated)
            {
                if(_slipperyTarget.isLocal && _slipperyTarget.owner == null)
                    Fondle(_slipperyTarget);
                PhysicsObject po = Activator.CreateInstance(_slipperyTarget.GetType(), Editor.GetConstructorParameters(_slipperyTarget.GetType())) as PhysicsObject;
                _slipperyTarget.friction = po.friction;
                Level.Remove(this);
            }

            if (!hasInitialized)
            {
                PhysicsObject po = Activator.CreateInstance(_slipperyTarget.GetType(), Editor.GetConstructorParameters(_slipperyTarget.GetType())) as PhysicsObject;
                _slipperyTarget.friction = po.friction /= 2.5f;
                hasInitialized = true;
            }

            if (hasUpdated)
                hasUpdated = false;
        }
    }
}
