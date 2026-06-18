using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class slomoSniperShell : slomoSniperEjectedShell
    {
        public slomoSniperShell(float xpos, float ypos)
          : base(xpos, ypos, "slomoSniperShell", "metalBounce")
        {
        }
    }
}