using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class slomoShell : slomoEjectedShell
    {

        public slomoShell(float xpos, float ypos)
          : base(xpos, ypos, "slomoShell", "metalBounce")
        {
        }
    }
}
