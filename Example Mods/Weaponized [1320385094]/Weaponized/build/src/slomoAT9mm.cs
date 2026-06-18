using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
  public class slomoAT9mm : AmmoType
  {
    public slomoAT9mm()
    {

      this.accuracy = 0.75f;
      this.range = 300f;
      this.penetration = 0.4f;
      this.combustable = true;
      this.bulletSpeed = 1.4f;
      this.speedVariation = 0.2f;
        }

    public override void PopShell(float x, float y, int dir)
    {
      slomoShell slomoShell = new slomoShell(x, y);
      slomoShell.hSpeed = (float) dir * (0.1f + Rando.Float(1f));
      Level.Add((Thing) slomoShell);
    }
  }
}
