using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Stuff")]

    public class floatMine : MaterialThing
    {
        public bool pin = false;
        public float _rotAngle;
        private bool duckClose = false;
        private bool shouldPing = false;
        private Sprite _mineFlash;

        public override float angle
        {
            get
            {
                return base.angle + Maths.DegToRad(-this._rotAngle);
            }
            set
            {
                this._angle = value;
            }
        }
        public floatMine(float xpos, float ypos)

          : base(xpos, ypos)
        {
            this.graphic = new Sprite(GetPath("floatMine"), 0.0f, 0.0f);
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.collisionSize = new Vec2(12f, 12f);
            this.depth = -0.5f;
            this.thickness = 4f;
            this.weight = 0f;
            this.flammable = 0.0f;
            this.collideSounds.Add("metalRebound");
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._mineFlash = new Sprite("mineFlash", 0.0f, 0.0f);
            this._mineFlash.CenterOrigin();
            this._mineFlash.alpha = 0.0f;
            this._editorName = "Floating Mine";
            this.editorTooltip = "Remnants after the Great Duck War, be careful not to touch them as they are still fully functional.";

        }
        public override void Update()
        {
            if (this.pin == true)
                this._rotAngle -= 0.13f;
            if ((double)this._rotAngle > 20.0)
                this.pin = true;
            if (this.pin == false)
                this._rotAngle += 0.13f;
            if ((double)this._rotAngle < -20.0)
                this.pin = false;

            foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(this.position, 45f))
            {
                if (physicsObject is Duck || physicsObject is RagdollPart || physicsObject is TrappedDuck)
                    duckClose = true;
            }
            if (duckClose == false)
            {
                this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0f, 0.08f);
                shouldPing = true;
            }
            else
            {
                if (shouldPing == true)
                {
                    SFX.Play("badBeep", 1f, 0.0f, 0.0f, false);
                    shouldPing = false;
                }
                this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0.4f, 0.08f);
                duckClose = false;
            }

            base.Update();
        }
        public override void Impact(MaterialThing with, ImpactedFrom from, bool solidImpact)
        {
            if(!(with is Hat))
            {
                Explode();
            }
            base.Impact(with, from, solidImpact);

        }

        public void Explode()
        {
            Graphics.FlashScreen();
            for (int index = 0; index < 16; ++index)
            {
                float num = (float)((double)index * 30.0 - 10.0) + Rando.Float(20f);
                ATMissileShrapnel atMissileShrapnel = new ATMissileShrapnel();
                atMissileShrapnel.range = 30f + Rando.Float(5f);
                Vec2 vec2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(num)), (float)Math.Sin((double)Maths.DegToRad(num)));
                Bullet bullet = new Bullet(x + vec2.x * 8f, y - vec2.y * 8f, (AmmoType)atMissileShrapnel, num, (Thing)null, false, -1f, false, true);
                bullet.firedFrom = this;
                Level.Add((Thing)bullet);
                Level.Add((Thing)Spark.New(x + Rando.Float(-8f, 8f), y + Rando.Float(-8f, 8f), vec2 + new Vec2(Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f)), 0.02f));
                Level.Add((Thing)SmallSmoke.New(x + vec2.x * 8f + Rando.Float(-8f, 8f), y + vec2.y * 8f + Rando.Float(-8f, 8f)));
            }
            Level.Add((Thing)new ExplosionPart(x, y, true));
            int num1 = 4;
            for (int index = 0; index < num1; ++index)
            {
                float deg = (float)index * 60f + Rando.Float(-10f, 10f);
                float num2 = Rando.Float(20f, 26f);
                Level.Add((Thing)new ExplosionPart(x + (float)Math.Cos((double)Maths.DegToRad(deg)) * num2, y - (float)Math.Sin((double)Maths.DegToRad(deg)) * num2, true));
            }
            SFX.Play("explode", 1f, 0.0f, 0.0f, false);
            Level.Remove((Thing)this);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            Explode();
            return base.Hit(bullet, hitPos);
        }

        public override void Draw()
        {

            if ((double)this._mineFlash.alpha > 0.00999999977648258)
                Graphics.Draw(this._mineFlash, this.x, this.y - 3f);
            base.Draw();
        }
    }
}