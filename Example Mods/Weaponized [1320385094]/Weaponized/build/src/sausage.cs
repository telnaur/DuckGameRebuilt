using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Fire")]
    public class sausage : Gun
    {
        private SpriteMap _sprite;
        private bool thrown2 = false;
        private float timer = 12f;
        private float timer2 = 0.5f;
        public sausage(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("sausage2"), 15, 7, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 4f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(15f, 7f);
            this._barrelOffsetTL = new Vec2(8f, 4f);
            this._fireWait = 1f;
            this.flammable = 1f;
            this.physicsMaterial = PhysicsMaterial.Wood;
            this.bouncy = 0.8f;
            this.friction = 0.05f;
            this._editorName = "Sausage";
            this.editorTooltip = "It's hot to touch, which might explain the flames bursting after throwing it.";
        }

        public override void Update()
        {
            if (this.thrown2) {
                this.timer -= 0.1f;
                this.timer2 -= 0.1f;
                if (timer2 <= 0f)
                    Level.Add((Thing)Spark.New(this.x, this.y - 2f, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.1f));
                if(timer == 5f)
                    Level.Add((Thing)SmallFire.New(this.x, this.y, Rando.Float(4f) - 2f, Rando.Float(2f) - 2f, false, (MaterialThing)null, true, (Thing)this, false));
                if (timer <= 0f)
                {
                    SFX.Play("flameExplode", 0.9f, Rando.Float(-0.5f, 0.5f), 0.0f, false);
                    for (int index = 0; index < 8; ++index)
                    {
                        Level.Add((Thing)SmallFire.New(this.x, this.y, Rando.Float(4f) - 2f, Rando.Float(2f) - 2f, false, (MaterialThing)null, true, (Thing)this, false));
                    }
                    /*for (int index = 0; index < 12; ++index)
                    {
                        float num2 = (float)((double)index * 18.0 - 5.0) + Rando.Float(10f);
                        ATShrapnel atShrapnel = new ATShrapnel();
                        atShrapnel.range = 5f + Rando.Float(5f);
                        Bullet bullet = new Bullet(x + (float)(Math.Cos((double)Maths.DegToRad(num2)) * 6.0), y - (float)(Math.Sin((double)Maths.DegToRad(num2)) * 6.0), (AmmoType)atShrapnel, num2, (Thing)null, false, -1f, false, true);
                        bullet.firedFrom = (Thing)this;
                        this.firedBullets.Add(bullet);
                        Level.Add((Thing)bullet);
                    }*/
                    Level.Add((Thing)SmallSmoke.New(this.x, this.y));
                    Level.Remove((Thing)this);
                }
            }
            base.Update();
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnImpact(with, from);
            if((with is Duck || with is RagdollPart || with is TrappedDuck) && this.thrown2)
            {
                timer = 0f;
                with.Burn(with.center, this);
            }
        }

        public override void OnPressAction()
        {
            Duck owner = this.owner as Duck;
            if (owner != null)
            {
                owner.doThrow = true;
                this.clip.Add((MaterialThing)owner);
                owner.clip.Add((MaterialThing)this);
            }
            if (!this.thrown2)
            {
                SFX.Play("netGunFire", 0.5f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
            }
            this.thrown2 = true;
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            this.thrown2 = true;
            SFX.Play("netGunFire", 0.5f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
            return base.OnBurn(firePosition, litBy);
        }
    }
}

