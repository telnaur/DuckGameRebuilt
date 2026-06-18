using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Melee")]
    public class fingerStick : Gun
    {
        public float _addOffsetX;
        public int _attackState = -1;
        public bool readyToSwing = true;
        public bool swinging = false;
        public bool upward = false;

        private List<IAmADuck> hitDucks = new List<IAmADuck>();

        public fingerStick(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = (AmmoType)new ATLaser();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("fingerStick"), 0.0f, 0.0f);
            this.center = new Vec2(12f, 5f);
            this.collisionOffset = new Vec2(-12f, -5f);
            this.collisionSize = new Vec2(23f, 9f);
            this._barrelOffsetTL = new Vec2(20f, 5f);
            this._fireSound = "smg";
            this._fullAuto = false;
            this._fireWait = 1f;
            this._kickForce = 3f;
            this._holdOffset = new Vec2(6f, 2f);
            this.physicsMaterial = PhysicsMaterial.Plastic;
            this.holsterAngle = 180f;
            this._editorName = "Finger Stick";
            this.editorTooltip = "Finger other ducks to push them away.";
        }

        public override void Update() //I GOTTA UPDATE THIS SO THAT THE FINGER CANCELS CURRENT SPEED AND THEN OVERRIDES IT WITH ITS OWN
        {
            base.Update();

            if (this._attackState > -1)
            {
                if (this.owner == null)
                {
                    this.readyToSwing = true;
                    this._attackState = -1;
                    this.handOffset = Vec2.Zero;
                    this.hitDucks.Clear();
                    this._holdOffset = new Vec2(6, 2);
                    this._addOffsetX = 0;
                    this.swinging = false;
                }
                else
                {
                    Duck owner2 = this.owner as Duck;
                    if (owner2 != null)
                    {
                        if (owner2.holdObstructed || owner2._hovering)
                        {
                            upward = true;
                        }
                        else
                        {
                            upward = false;
                        }
                    }
                }
                if (this._attackState == 0)
                {
                    this.hitDucks.Clear();
                    this.readyToSwing = false;
                    SFX.Play("swipe", 1f, 0.0f, 0.0f, false);
                    ++this._attackState;
                }
                else if (this._attackState == 1)
                {
                    if (this._addOffsetX <= 7.95f)
                    {
                        this.swinging = true;
                        this._addOffsetX = MathHelper.Lerp(this._addOffsetX, 8f, 0.35f);
                        if (upward)
                        {
                            this.handOffset = new Vec2(0, -this._addOffsetX);
                        }
                        else
                        {
                            this.handOffset = new Vec2(this._addOffsetX, 0);
                        }
                        this._holdOffset = new Vec2(this._addOffsetX + 6, 2);
                    }
                    else
                    {
                        swinging = false;
                        ++this._attackState;
                    }
                }
                else if (this._attackState == 2)
                {
                    if (this._addOffsetX >= 0.05f)
                    {
                        this.swinging = false;
                        this._addOffsetX = MathHelper.Lerp(this._addOffsetX, 0f, 0.35f);
                        Duck owner = this.owner as Duck;
                        if (owner != null)
                        {
                            if (owner.holdObstructed || owner._hovering)
                            {
                                this.handOffset = new Vec2(0, -this._addOffsetX);
                            }
                            else
                            {
                                this.handOffset = new Vec2(this._addOffsetX, 0);
                            }
                        }
                        this._holdOffset = new Vec2(this._addOffsetX + 6, 2);
                    }
                    else
                    {
                        this._attackState = -1;
                        this.readyToSwing = true;
                    }
                }
            }
            if (this.swinging)
            {
                this.Offset(this.barrelOffset);
                IEnumerable<IAmADuck> amAducks = Level.CheckRectAll<IAmADuck>(this.barrelPosition + new Vec2(-5, -5), this.barrelPosition + new Vec2(5, 5));
                foreach (IAmADuck amAduck in amAducks)
                {
                    Duck owner = this.owner as Duck;
                    if (owner != null && owner == amAduck)
                    {
                        continue;
                    }
                    if (!this.hitDucks.Contains(amAduck))
                    {
                        this.hitDucks.Add(amAduck);
                        Duck ducky = amAduck as Duck;
                        if (ducky != null)
                        {
                            ducky.hSpeed = 8f * this.barrelVector.x + Rando.Float(-1f, 1f);
                            ducky.vSpeed = 6f * this.barrelVector.y - 3f;
                            /*
                            if (upward)
                            {
                                ducky.hSpeed += Rando.Float(2f, 2f);
                                ducky.vSpeed -= 10f;
                            }
                            else
                            {
                                ducky.hSpeed += 10f * this.offDir;
                                ducky.vSpeed -= 4f;
                            }*/
                            PhysicsObject holdObject = (PhysicsObject)ducky.holdObject;
                            if (holdObject != null)
                            {
                                ducky.ThrowItem(false);
                                holdObject.vSpeed -= 4f;
                                holdObject.hSpeed = ducky.hSpeed * 0.8f;
                                holdObject.clip.Add((MaterialThing)ducky);
                                ducky.clip.Add((MaterialThing)holdObject);
                            }
                            /*
                            if (upward)
                            {
                                ducky.hSpeed += Rando.Float(-0.5f, 0.5f);
                                ducky.vSpeed -= 2f;
                            }
                            else
                            {
                                ducky.hSpeed += 2.5f * this.offDir;
                                ducky.vSpeed -= 1f;
                            }
                            */
                            if(!Network.isActive) //if (ducky.isServerForObject)
                            {
                                ducky.crippleTimer = 3f;
                                ducky.GoRagdoll();
                            }
                        }
                    }
                }
                IEnumerable<RagdollPart> ragdolls = Level.CheckRectAll<RagdollPart>(this.barrelPosition + new Vec2(-5, -5), this.barrelPosition + new Vec2(5, 5));
                foreach (RagdollPart ragdoll in ragdolls)
                {
                    if (!this.hitDucks.Contains(ragdoll.duck))
                    {
                        RagdollPart a = ragdoll as RagdollPart;
                        this.hitDucks.Add(a.duck);
                        a.hSpeed = 5f * this.barrelVector.x + Rando.Float(-1f, 1f);
                        a.vSpeed = 5f * this.barrelVector.y - 3f;
                    }
                }
            }
            else
            {
                this.hitDucks.Clear();
            }
        }

        public override void OnPressAction()
        {
            if (readyToSwing)
            {
                this._attackState = 0;
            }
        }

        public override void Fire()
        {
        }
    }
}
