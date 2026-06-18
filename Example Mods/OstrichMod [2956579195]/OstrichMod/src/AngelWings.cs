using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Eternal")]
    class AngelWings : Gun
    {
        private SpriteMap sprite;

        public AngelWings(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "AngelWings";
            this.ammo = 8;
            this._ammoType = new ATFuciel();
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("AngelWings"),21, 13);
            base.graphic = this.sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-7f, -8f);
            this.collisionSize = new Vec2(21f, 13f);
            this._barrelOffsetTL = new Vec2(21f, 3f);
            this._kickForce = 0.3f;
            this.loseAccuracy = 0.4f;
            this.maxAccuracyLost = 0.4f;
            this._fireSound = GetPath("sounds/epic_shot");
            this.weight = 5f;
        }

    }
}
