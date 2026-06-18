using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class slomoMagnumShell : slomoMagnumEjectedShell
    {
        public slomoMagnumShell(float xpos, float ypos)
          : base(xpos, ypos, "slomoMagnumShell", "metalBounce")
        {
        }
    }
}
