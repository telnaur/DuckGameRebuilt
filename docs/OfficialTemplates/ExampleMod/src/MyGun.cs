using System;

namespace DuckGame.ExampleMod
{
    [EditorGroup("ExampleMod|guns")]
    public class MyGun : Gun
    {
        public MyGun(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 10;
            this._ammoType = new AT9mm();
            this._ammoType.range = 200f;
            this._ammoType.accuracy = 1f;
            this._ammoType.penetration = 1f;
            this._type = "gun";
            base.graphic = new SpriteMap(Mod.GetPath<ExampleMod>("ExGun"), 16, 9);
            this.center = new Vec2(8f, 4.5f);
            this.collisionOffset = new Vec2(-8f, -2f);
            this.collisionSize = new Vec2(16f, 9f);
            this._barrelOffsetTL = new Vec2(16f, 1f);
            this._holdOffset = new Vec2(1f, 1.5f);
            this._fireSound = "pistolFire";
            this._kickForce = 3f;
        }
    }
}