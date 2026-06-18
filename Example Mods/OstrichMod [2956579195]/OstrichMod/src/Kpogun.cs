using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Virgo")]
    class Kpogun : Gun
    {
        private SpriteMap sprite;

        public Kpogun(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Kpogun";
            this.ammo = 1;
            this._ammoType = new ATPene();
            this._type = "gun";
            this._fullAuto = true;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Kpogun"),17, 13);
            base.graphic = this.sprite;
            this.center = new Vec2(8f, 6f);
            this.collisionOffset = new Vec2(-8f, -6f);
            this.collisionSize = new Vec2(17f, 13f);
            this._barrelOffsetTL = new Vec2(16f, 7f);
            this._holdOffset = new Vec2(-1f, 0f);
            this._fireSound = GetPath("SFX/virgo");
            this.ammoType.affectedByGravity = false;
            this.weight = 5f;
            this.ammoType.accuracy = 0.3f;
            this._numBulletsPerFire = 17;
            this._editorName = "Kpogun";
        }

    }
}
