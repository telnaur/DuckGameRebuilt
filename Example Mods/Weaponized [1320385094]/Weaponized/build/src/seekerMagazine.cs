using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class seekerMagazine : ejectedSeekerMagazine
    {
        public seekerMagazine(float xpos, float ypos)
          : base(xpos, ypos, "seekerMagazine", "metalBounce")
        {
        }
    }
}
