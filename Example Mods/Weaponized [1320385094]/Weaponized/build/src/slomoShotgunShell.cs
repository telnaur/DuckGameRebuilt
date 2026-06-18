using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class slomoShotgunShell : slomoShotgunEjectedShell
    {
        public slomoShotgunShell(float xpos, float ypos)
          : base(xpos, ypos, "slomoShotgunShell", "plasticBounce")
        {
        }
    }
}