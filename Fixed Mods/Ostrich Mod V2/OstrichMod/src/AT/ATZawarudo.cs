using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    internal class ATZawarudo : Thing
    {
        public StateBinding _theDuckStateBinding = new StateBinding("_theDuck");
        public StateBinding _countStateBinding = new StateBinding("_count");
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public bool rightVar = false;
        public bool leftVar = false;
        public bool upVar = false;
        public bool bottomVar = false;
        public Duck _theDuck;
        public int _count;

        private SpriteMap sprite;

        // Targets this wave has already affected, so each duck/holdable is stunned
        // exactly once instead of re-spawning a networked stun Thing every frame.
        private readonly HashSet<Thing> _alreadyHit = new HashSet<Thing>();


        public ATZawarudo(float xpos, float ypos, Duck d, bool right, bool left, bool up, bool bottom)
            : base(xpos, ypos)
        {
            rightVar = right;
            leftVar = left;
            upVar = up;
            bottomVar = bottom;

            _theDuck = d;
            center = new Vec2(9f, 6f);
            _collisionOffset = new Vec2(-9f, -6f);
            _collisionSize = new Vec2(18f, 12f);
        }

        public override void Update()
        {
            if (isServerForObject && (x > Level.current.bottomRight.x + 2000 || x < Level.current.topLeft.x - 2000 || _count >= 200))
                Level.Remove(this);

            _count++;

            if (rightVar)
            {
                x += offDir * 7f;
            }
            if (leftVar)
            {
                x -= offDir * 7f;
            }
            if (upVar)
            {
                y += 7f;
            }
            if (bottomVar)
            {
                y -= offDir * 7f;
            }

            // Only the authority spawns the networked stun Things; otherwise every
            // client also floods the level with ghosts -> desync + freeze online.
            if (isServerForObject && alpha >= 0.6f)
                foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(position, 151f))
                {
                    // Stun each target only once per wave (prevents the per-frame spawn flood).
                    if (!_alreadyHit.Add(materialThing))
                        continue;

                    if (materialThing is Duck)
                    {
                        Level.Add(new Zawarudo(materialThing as Duck, 410, showDaze: true));
                    }
                    else if (materialThing is Holdable)
                    {
                        Level.Add(new ZawarudoThing(materialThing as Holdable, 410, showDaze: true));
                    }
                }

            base.Update();
        }
    }
}


