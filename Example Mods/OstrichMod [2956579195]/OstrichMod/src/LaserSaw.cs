using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Aeris")]
    class LaserSaw : Gun
    {
        private SpriteMap sprite;

        public LaserSaw(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Laser Saw";
            this.ammo = 5;
            this._ammoType = new ATLaser();
            this._ammoType.bulletThickness = 2f;
            this._fireWait = 3f;
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("LaserSaw"),33, 8);
            base.graphic = this.sprite;
            this.center = new Vec2(16f, 4f);
            this.collisionOffset = new Vec2(-16f, -4f);
            this.collisionSize = new Vec2(33f, 8f);
            this._barrelOffsetTL = new Vec2(34f, 1f);
            this._holdOffset = new Vec2(4f, 3f);
            this._fireSound = "laserRifle";
            this.weight = 5f;
            this._flare = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("ProfileFlare"), 16, 16, false);
            this._flare.center = new Vec2(0.0f, 8f);
        }

    }
}
