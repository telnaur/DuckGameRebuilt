using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Explosives")]

    public class textGun : Gun
    {

        public textGun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 5;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("textGun"), 0.0f, 0.0f);
            this.center = new Vec2(8f, 6f);
            this.collisionOffset = new Vec2(-8f, -6f);
            this.collisionSize = new Vec2(17f, 11f);
            this._barrelOffsetTL = new Vec2(21f, 5f);
            this._fireSound = "pistolFire";
            this._kickForce = 5f;
            this._holdOffset = new Vec2(0f, -1f);
            this.physicsMaterial = PhysicsMaterial.Paper;
            this._editorName = "Emoji Launcher";
            this.editorTooltip = "The fired emojis bounce around the room killing all ducks they encounter, what a horrible sight.";
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                SFX.Play("pistolFire", 1f, 0.0f, 0.0f, false);
                this.ApplyKick();
                Vec2 vec2 = this.Offset(this.barrelOffset);
                ATtext5 attext5 = new ATtext5(vec2.x, vec2.y);
                Duck owner = this.owner as Duck;
                if (owner != null)
                {
                    attext5.ownerDuck = owner;
                }
                this.Fondle((Thing)attext5);
                attext5.hSpeed = barrelVector.x * 3 + Rando.Float(-0.5f, 0.5f);
                attext5.vSpeed = barrelVector.y * 3 + Rando.Float(-0.5f, 0.5f);
                int ammoSprite = Rando.Int(0, 4);
                switch (ammoSprite)
                {
                    case 0:
                        attext5.graphic = new Sprite(GetPath("emoji5"), 0.0f, 0.0f);
                        break;
                    case 1:
                        attext5.graphic = new Sprite(GetPath("emoji4"), 0.0f, 0.0f);
                        break;
                    case 2:
                        attext5.graphic = new Sprite(GetPath("emoji3"), 0.0f, 0.0f);
                        break;
                    case 3:
                        attext5.graphic = new Sprite(GetPath("emoji2"), 0.0f, 0.0f);
                        break;
                    case 4:
                        attext5.graphic = new Sprite(GetPath("emoji1"), 0.0f, 0.0f);
                        break;
                    default:
                        attext5.graphic = new Sprite(GetPath("emoji5"), 0.0f, 0.0f);
                        break;
                }
                Level.Add((Thing)attext5);
                this.ammo -= 1;
            }
        }
    }
}
