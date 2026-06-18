using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    // credits to Garoslaw

    internal class HHGHalo : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _theGrenadeStateBinding = new StateBinding("_theGrenade");

        public HolyHandGrenade _theGrenade;

        private SpriteMap sprite;
        private float transition;

        public HHGHalo(float xpos, float ypos, HolyHandGrenade grenade) :
            base(xpos, ypos, null)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\halo"), 242, 82);
            graphic = sprite;
            center = new Vec2(121f, 41f);
            alpha = 0f;

            transition = 0.2f;
            _theGrenade = grenade;
        }

        public override void Update()
        {
            if (_theGrenade == null || _theGrenade._realTimer <= 0f)
            {
                Level.Remove(this);
                return;
            }

            position = _theGrenade.position - new Vec2(0f, 12f);

            transition = MathHelper.Lerp(transition, 0.02f, 0.02f);
            yscale = xscale = MathHelper.SmoothStep(yscale, 0.025f, transition);
            alpha = 1f - _theGrenade._realTimer;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
