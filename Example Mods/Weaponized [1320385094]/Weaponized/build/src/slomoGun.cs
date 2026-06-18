using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Slo-Mo")]
    public class slomoPistol : Gun
    {
        private SpriteMap _sprite;

        public slomoPistol(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 9;
            this._ammoType = (AmmoType)new slomoAT9mm();
            this._ammoType.immediatelyDeadly = true;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("slomoGun"), 18, 10, false);
            this._sprite.AddAnimation("idle", 1f, true, new int[1]);
            this._sprite.AddAnimation("fire", 0.8f, 0 != 0, 1, 2, 2, 3, 3);
            this._sprite.AddAnimation("empty", 1f, 1 != 0, 2);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(10f, 3f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 9f);
            this._barrelOffsetTL = new Vec2(18f, 2f);
            this._fireSound = "pistolFire";
            this._kickForce = 4f;
            this._holdOffset = new Vec2(-1f, 0.0f);
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._editorName = "Slo-Mo Pistol";
            this.editorTooltip = "These bullets are so slooow!";


        }

        public override void Update()
        {
            if (this._sprite.currentAnimation == "fire" && this._sprite.finished)
                this._sprite.SetAnimation("idle");
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                this._sprite.SetAnimation("fire");
                for (int index = 0; index < 3; ++index)
                {
                    Vec2 vec2 = this.Offset(new Vec2(-9f, 0.0f));
                    Vec2 hitAngle = this.barrelVector.Rotate(Rando.Float(1f), Vec2.Zero);
                    Level.Add((Thing)Spark.New(vec2.x, vec2.y, hitAngle, 0.1f));
                }
            }
            else
                this._sprite.SetAnimation("empty");
            this.Fire();
        }
    }
}
