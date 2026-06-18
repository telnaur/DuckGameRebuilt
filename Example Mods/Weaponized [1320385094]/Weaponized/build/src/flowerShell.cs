using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class flowerShell : EjectedShell
    {
        public flowerShell(float xpos, float ypos)
          : base(xpos, ypos, "pistolShell", "woodHit")
        {
        }
    }
}
