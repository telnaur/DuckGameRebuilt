using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    //[EditorGroup("Zyrafa|Other")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class duckEgg : Gun
    {
        private SpriteMap _sprite;
        public bool hugging = false;
        public Duck duckThatHugged;
        private bool splatted = false;
        private float hugPos = -3f;


        public duckEgg(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new AT9mm();
            this._sprite = new SpriteMap(GetPath("duckEgg"), 10, 12, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(5f, 6f);
            this.collisionOffset = new Vec2(-5f, -6f);
            this.collisionSize = new Vec2(10f, 12f);
            this._fireSound = "deepMachineGun2";
            this._barrelOffsetTL = new Vec2(5f, 12f);
            this._sprite.AddAnimation("idle", 1f, true, 0);
            this._sprite.AddAnimation("cracking", 0.2f, false, 0, 1, 2, 3, 4);
            this._sprite.SetAnimation("idle");
        }

        public override void OnPressAction()
        {   
            if (this.duckThatHugged == null)
                this.hugging = true;
        }

        public override void Update()
        {
            if (this.duck == null)
            {
                this.hugging = false;
            }
            if (this.splatted)
            {
                SFX.Play("glassBreak", 1f, Rando.Float(-0.2f, 0.2f), 0.0f, false);
                this.canPickUp = false;
                for (int index = 0; index < 6; ++index)
                {
                    potionDebris thing = potionDebris.New(this.x - 4f + Rando.Float(10f), this.y - 8f + Rando.Float(8f));
                    thing.hSpeed = (float)(((double)Rando.Float(1f) > 0.5 ? 1.0 : -1.0) * (double)Rando.Float(2f) + (double)Math.Sign(x) * 0.4);
                    thing.vSpeed = -Rando.Float(1f);
                    Level.Add((Thing)thing);
                }
                if (duckThatHugged != null)
                {
                    Duck newDuck = new Duck(x, y, duckThatHugged.profile);
                    Level.Add((Thing)newDuck);
                    newDuck.duckSize = 0.7f;
                    newDuck.crippleTimer = 1.5f;
                    newDuck.GoRagdoll();
                    //newDuck.ConvertDuck(duckThatHugged);
                    //GameLevel childInstance = new GameLevel(GameLevel.current.level);
                    //FollowCam myProtectedProperty = (FollowCam)childInstance.GetType().BaseType.GetField("_followCam", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(childInstance);
                    //myProtectedProperty.Add((Thing)newDuck);
                    //Level.current.camera = FollowCam;
                }
                Level.Remove((Thing)this);
            }
            if (this.hugging)
            {
                this.hugPos += 0.2f;
                if(this.hugPos > 4f)
                {
                    this.hugging = false;
                    this._sprite.SetAnimation("cracking");
                    duckThatHugged = duck;
                }
            }
            else
            {
                hugPos = -3f;
            }
            base.Update();
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {

                this.splatted = true;
            base.OnSolidImpact(with, from);
        }

        public override void Draw()
        {
            base.Draw();
            if (this.duck != null && this.hugging)
            {
                Vec2 vec2 = this.Offset(this.center - this.center);
                Vec2 p = vec2 + new Vec2(3f * this.offDir, this.hugPos);
                this.duck._spriteArms.depth = this.depth + 1;
                Graphics.Draw((Sprite)duck._spriteArms, p.x, p.y, -1f, 1f);
            }
            this.position = new Vec2(this.position.x, this.position.y);
        }
    }
}
