using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Miusac")]
    class Ampli : Holdable, IPlatform
    {
        private SpriteMap sprite;

        public Ampli(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Ampli";
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Ampli"), 18, 16);
            this.graphic = sprite;
            this.center = new Vec2(6f, 8f);
            this.collisionOffset = new Vec2(-6f, -8f);
            this.collisionSize = new Vec2(18f, 16f);
            this.depth = -0.5f;
            this.thickness = 2f;
            this.weight = 6f;
            this.flammable = 0.2f;
            this.friction = 0.25f;
            this.physicsMaterial = PhysicsMaterial.Metal;
        }
        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is Duck && this.hSpeed > 0.01f)
            {
                ((Duck)with).GoRagdoll();
            }
            else if (with is Duck && this.hSpeed < -0.01f)
            {
                ((Duck)with).GoRagdoll();
            }
        }
    }
}

