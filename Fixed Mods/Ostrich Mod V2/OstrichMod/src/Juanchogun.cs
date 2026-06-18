using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Virgo")]
    class Juanchogun : Gun
    {
        private SpriteMap sprite;

        public Juanchogun(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Juanchogun";
            this.ammo = 3;
            this._ammoType = new ATEgg();
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Juanchogun"),54, 20);
            base.graphic = this.sprite;
            this.center = new Vec2(24f, 10f);
            this.collisionOffset = new Vec2(-9f, -5f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(30f, 10f);
            this._holdOffset = new Vec2(4f, 0.0f);
            this._fireSound = GetPath("SFX/yepa.wav");
            this.weight = 5f;
            this._numBulletsPerFire = 3;
            this._editorName = "Juanchogun";
        }

    }
}
