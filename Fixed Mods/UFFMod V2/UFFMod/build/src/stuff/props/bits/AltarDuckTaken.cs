using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.UFFMod
{
    internal class AltarDuckTaken : Thing
    {
        public StateBinding _theDuckStateBinding = new StateBinding("_theDuck");
        public StateBinding _theGolduckStateBinding = new StateBinding("_theGolduck");
        public StateBinding _theGunStateBinding = new StateBinding("_theGun");
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        public Duck _theDuck;
        public Golduck _theGolduck;
        public Gun _theGun;

        private bool localDoneInfinite;

        public AltarDuckTaken(float xpos, float ypos, Duck td = null, Golduck gd = null, Gun gun = null)
            : base(xpos, ypos)
        {
            if (td != null)
                _theDuck = td;
            if (gd != null)
                _theGolduck = gd;
            if (gun != null)
                _theGun = gun;
        }

        public override void Update()
        {
            if (_theGun != null && !localDoneInfinite && !_theGun.infinite)
            {
                _theGun.infinite = true;
                localDoneInfinite = true;
            }

            base.Update();
        }
    }
}
