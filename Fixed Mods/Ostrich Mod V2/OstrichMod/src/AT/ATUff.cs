using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
  public class ATUff : AmmoType
  {
    public ATUff()
    {
      this.accuracy = 0.35f;
      this.penetration = 0.35f;
      this.bulletSpeed = 5f;
      this.rangeVariation = 0.0f;
      this.speedVariation = 0.0f;
      this.range = 2000f;
      this.rebound = false;
      this.affectedByGravity = true;
      this.deadly = false;
      this.weight = 5f;
      this.bulletThickness = 2f;
      this.bulletType = typeof (GrenadeBullet);
      this.immediatelyDeadly = true;
      this.sprite = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Uff"), 0.0f, 0.0f);
      this.sprite.CenterOrigin();
    }

    public override void PopShell(float x, float y, int dir)
    {
      PistolShell pistolShell = new PistolShell(x, y);
      pistolShell.hSpeed = (float) dir * (1.5f + Rando.Float(1f));
      Level.Add((Thing) pistolShell);
    }
  }
}

