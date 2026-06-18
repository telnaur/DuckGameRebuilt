using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class openCrateWallL : Block
    {
        private int crateNr;
        public openCrateWallL(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite(GetPath("openCrateWallL"), 0.0f, 0.0f);
            this.center = new Vec2(3f, 13f);
            this.collisionOffset = new Vec2(-3f, -13f);
            this.collisionSize = new Vec2(5f, 25f);
            this.thickness = 2f;
            this.physicsMaterial = PhysicsMaterial.Wood;
        }
        public override void Update()
        {
            foreach (openCrate crate in Level.CheckCircleAll<openCrate>(this.position, 15f))
                crateNr++;
            if (crateNr == 0)
                Level.Remove((Thing)this);
            crateNr = 0;
            base.Update();
        }
        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            for (int index = 0; (double)index < 3 ; ++index)
            {
                Thing thing = (Thing)WoodDebris.New(hitPos.x, hitPos.y);
                thing.hSpeed = (float)(-(double)bullet.travelDirNormalized.x * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                thing.vSpeed = (float)(-(double)bullet.travelDirNormalized.y * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929)) - Rando.Float(2f);
                Level.Add(thing);
            }
            SFX.Play("woodHit", 1f, 0.0f, 0.0f, false);
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            for (int index = 0; (double)index < 3; ++index)
            {
                Thing thing = (Thing)WoodDebris.New(exitPos.x, exitPos.y);
                thing.hSpeed = (float)((double)bullet.travelDirNormalized.x * 3.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                thing.vSpeed = (float)((double)bullet.travelDirNormalized.y * 3.0 * ((double)Rando.Float(1f) + 0.300000011920929) - ((double)Rando.Float(2f) - 1.0));
                Level.Add(thing);
            }
        }
    }
}
