using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Machine Guns")]
    public class sunflower : Gun
    {
        public sunflower(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 12;
            this._ammoType = (AmmoType)new ATFlower();
            this._ammoType.range = 260f;
            this._ammoType.accuracy = 0.9f;
            this._ammoType.penetration = 1f;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("sunflower"), 0.0f, 0.0f);
            this.center = new Vec2(5f, 14f);
            this.collisionOffset = new Vec2(-5f, -14f);
            this.collisionSize = new Vec2(10f, 28f);
            this._barrelOffsetTL = new Vec2(5f, Rando.Float(5f, 9f));
            this._fireSound = "deepMachineGun";
            this._fullAuto = true;
            this._fireWait = 1.8f;
            this._kickForce = 1.5f;
            this.loseAccuracy = 0.05f;
            this.maxAccuracyLost = 0.3f;
            this._holdOffset = new Vec2(-1f, -8f);
            this._editorName = "Sunflower";
            this.editorTooltip = "Genetically modified to effectively kill higher positioned ducks.";
        }
        public override void Update()
        {
            this._barrelOffsetTL = new Vec2(5f, Rando.Float(4f, 10f));
            base.Update();
        }
    }
}
