using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Pistols")]
    public class tec9 : Gun
    {
        public tec9(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 20;
            this._ammoType = (AmmoType)new AT9mm();
            this._ammoType.range = 80f;
            this._ammoType.accuracy = 0.5f;
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("tec9"), 0.0f, 0.0f);
            this.center = new Vec2(10f, 6f);
            this.collisionOffset = new Vec2(-10f, -6f);
            this.collisionSize = new Vec2(19f, 12f);
            this._barrelOffsetTL = new Vec2(19f, 1f);
            this._fireSound = "littleGun";
            this._kickForce = 1f;
            this.maxAccuracyLost = 0.7f;
            this._holdOffset = new Vec2(1f, 2f);
            this._editorName = "Tec-9";
            this.editorTooltip = "Run and gun with some fast clicking action.";
        }
        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                for (int index = 0; index < 3; ++index)
                {
                    Vec2 vec2 = this.Offset(new Vec2(-9f, 0.0f));
                    Vec2 hitAngle = this.barrelVector.Rotate(Rando.Float(1f), Vec2.Zero);
                    Level.Add((Thing)Spark.New(vec2.x, vec2.y, hitAngle, 0.1f));
                }
            }
            this.Fire();
        }
    }
}
