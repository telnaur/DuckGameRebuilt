using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{

    [EditorGroup("Zyrafa|Guns|Explosives")]
    [BaggedProperty("isSuperWeapon", true)]
    public class shrekZooka : Gun

    {
        public bool _pin = false;

        public shrekZooka(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new ATShrek();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("shrekZooka"), 0.0f, 0.0f);
            this.center = new Vec2(15f, 5f);
            this.collisionOffset = new Vec2(-15f, -4f);
            this.collisionSize = new Vec2(30f, 10f);
            this._barrelOffsetTL = new Vec2(29f, 4f);
            this._fireSound = GetPath("swamp");
            this._kickForce = 8f;
            this._holdOffset = new Vec2(-2f, -3f);
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._barrelAngleOffset = 180f;
            this._bulletColor = Color.DarkOliveGreen;
            this._editorName = "Shrekzooka";
            this.editorTooltip = "What are you doing on his swamp?!";

        }
        public override void OnPressAction()


        {
            if(this.ammo > 0)
            SFX.Play("missile", 1f, 0.0f, 0.0f, false);
            this.kick = 1.2f;
            base.OnPressAction();
            int num = 0;
            if (this._pin == false)
            {
                for (int index = 0; index < 14; ++index)
                {

                    shrekSmoke shrekSmoke = new shrekSmoke((float)((double)this.x - 16.0 + (double)Rando.Float(32f) + (double)this.offDir * 10.0), this.y - 16f + Rando.Float(32f));
                    shrekSmoke.depth = (Depth)((float)(0.899999976158142 + (double)index * (1.0 / 1000.0)));
                    if (num < 6)
                        shrekSmoke.move.x -= (float)this.offDir * Rando.Float(0.1f);
                    if (num > 5 && num < 10)
                        shrekSmoke.fly.x += (float)this.offDir * (2f + Rando.Float(7.8f));
                    Level.Add((Thing)shrekSmoke);
                    ++num;
                }
                this._pin = true;
            }
        }



        public override void Update()
        {

            if (this.ammo > 0)
            {
                this.graphic = new Sprite(GetPath("shrekZooka"), 0.0f, 0.0f);

            }
            else
                this.graphic = new Sprite(GetPath("shrekZookaEmpty"), 0.0f, 0.0f);

            base.Update();
            if (this.ammo > 0)
                if (this.offDir > 0)
                {
                    this.ammoType.sprite = new Sprite(Mod.GetPath<DuckGame.MyMod.MyMod>("shrek"), 0.0f, 0.0f);
                    this.ammoType.sprite.CenterOrigin();
                }
                else
                {
                    this.ammoType.sprite = new Sprite(Mod.GetPath<DuckGame.MyMod.MyMod>("shrek2"), 0.0f, 0.0f);
                    this.ammoType.sprite.CenterOrigin();
                }
            else
                return;
        }
    }
}
