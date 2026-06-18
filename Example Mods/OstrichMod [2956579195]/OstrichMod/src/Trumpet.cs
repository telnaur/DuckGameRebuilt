using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Tech")]
    class Trumpet : Gun
    {
        private SpriteMap sprite;

        public Trumpet(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Trumpet";
            this.ammo = 3;
            this._ammoType = new ATLaser();
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("RailGun"),34, 12);
            this.sprite.AddAnimation("RailGun", 0.5f, true, new int[] { 0, 1 });
            this.sprite.SetAnimation("RailGun");
            base.graphic = this.sprite;
            this.center = new Vec2(15f, 5f);
            this.collisionOffset = new Vec2(-15f, -4f);
            this.collisionSize = new Vec2(30f, 10f);
            this._barrelOffsetTL = new Vec2(33f, 2f);
            this._kickForce = 4f;
            this._fireSound = GetPath("sounds/rail");
            this.ammoType.penetration = 25f;
            this._ammoType.accuracy = 0.5f;
            this.weight = 5f;
            this._numBulletsPerFire = 30;
            this._editorName = "Trumpet";
        }

    }
}
