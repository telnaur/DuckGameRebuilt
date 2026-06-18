using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Rifles")]
    public class awp : Gun
    {
        public StateBinding _loadStateBinding = new StateBinding("_loadState", -1, false, false);
        public StateBinding _angleOffsetBinding = new StateBinding("_angleOffset", -1, false, false);
        public StateBinding _netLoadBinding = (StateBinding)new NetSoundBinding("_netLoad");
        public NetSoundEffect _netLoad = new NetSoundEffect(new string[1] { "loadSniper" });
        public int _loadState = -1;
        public int _loadAnimation = -1;
        public float _angleOffset;
        public float _aimAngle;
        public float _fireAngle;

        private bool goingUp = true;
        private bool isHovering = false;
        private float maxAngleInnacuracy = 30f;

        public override float angle
        {
            get
            {
                return base.angle + this._aimAngle;
            }
            set
            {
                this._angle = value;
            }
        }

        public awp(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 5;
            this._ammoType = (AmmoType)new ATSniper();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("awp"), 0.0f, 0.0f);
            this.center = new Vec2(17f, 4f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(17f, 8f);
            this._barrelOffsetTL = new Vec2(33f, 3f);
            this._fireSound = GetPath("awpShot");
            this._kickForce = 2f;
            this._manualLoad = true;
            this.ammoType.penetration = 2f;
            this.ammoType.accuracy = 0.96f;
            this._holdOffset = new Vec2(1f, 0f);
            this._editorName = "AWP";
            this.editorTooltip = "The main feature is its sweet sound. The lethality is a good bonus too.";
        }

        public override void Update()
        {
            base.Update();

            Duck owner = this.owner as Duck;
            if (owner != null)
            {
                //if (owner.hSpeed != 0 || !owner.grounded)
                if (!owner.grounded)
                {/*
                    if (owner.grounded)
                    { // jak spadasz i dalej biegniesz to bron nie wraca natychmiast na miejsce
                        //if (isHovering)
                        //{
                        //    this._fireAngle = 0;
                        //    isHovering = false;
                        //}
                        if (goingUp)
                        {
                            this._fireAngle = Lerp.Float(this._fireAngle, maxAngleInnacuracy, 4f);
                            if (this._fireAngle >= maxAngleInnacuracy)
                            {
                                goingUp = false;
                            }
                        }
                        else
                        {
                            this._fireAngle = Lerp.Float(this._fireAngle, -maxAngleInnacuracy, 4f);
                            if (this._fireAngle <= -maxAngleInnacuracy)
                            {
                                goingUp = true;
                            }
                        }
                    }
                    else
                    {*/
                        //isHovering = true;
                        if (this.offDir >= (sbyte)0)
                        {
                            this._fireAngle += 8f;
                        }
                        else
                        {
                            this._fireAngle -= 8f;
                        }
                    //}
                    this._aimAngle = -Maths.DegToRad(this._fireAngle);
                }
                else
                {
                    //this._fireAngle %= 360f;
                    //this._fireAngle = Lerp.Float(this._fireAngle, 0, 8);
                    this._fireAngle = 0;
                    this._aimAngle = -Maths.DegToRad(this._fireAngle);
                }
            }
            else
            {
                this._fireAngle = 0;
                this._aimAngle = -Maths.DegToRad(this._fireAngle);
            }

            if (this._loadState > -1)
            {
                if (this.owner == null)
                {
                    if (this._loadState == 3)
                        this.loaded = true;
                    this._loadState = -1;
                    this._angleOffset = 0.0f;
                    this.handOffset = Vec2.Zero;
                }
                if (this._loadState == 0)
                {
                    if (Network.isActive)
                    {
                        if (this.isServerForObject)
                            this._netLoad.Play(1f, 0.0f);
                    }
                    else
                        SFX.Play("loadSniper", 1f, 0.0f, 0.0f, false);
                    ++this._loadState;
                }
                else if (this._loadState == 1)
                {
                    if ((double)this._angleOffset < 0.159999996423721)
                        this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.2f, 0.15f);
                    else
                        ++this._loadState;
                }
                else if (this._loadState == 2)
                {
                    this.handOffset.x += 0.4f;
                    if ((double)this.handOffset.x > 4.0)
                    {
                        ++this._loadState;
                        this.Reload(true);
                        this.loaded = false;
                    }

                }
                else if (this._loadState == 3)
                {
                    this.handOffset.x -= 0.4f;
                    if ((double)this.handOffset.x <= 0.0)
                    {
                        ++this._loadState;
                        this.handOffset.x = 0.0f;
                    }

                }
                else if (this._loadState == 4)
                {
                    if ((double)this._angleOffset > 0.0399999991059303)
                    {
                        this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.0f, 0.15f);
                    }
                    else
                    {
                        this._loadState = -1;
                        this.loaded = true;
                        this._angleOffset = 0.0f;
                    }
                }
            }

        }

        public override void OnPressAction()
        {
            if (this.loaded)
            {
                base.OnPressAction();
            }
            else
            {
                if (this.ammo <= 0 || this._loadState != -1)
                    return;
                this._loadState = 0;
                this._loadAnimation = 0;
                EmitParticles();
            }
        }

        public void EmitParticles()
        {
            for (int index = 0; index < 4; ++index)
            {
                int i = Rando.Int(0, 3);
                switch (i)
                {
                    case 0:
                        doritoSmoke doritoSmoke = new doritoSmoke((float)this.barrelPosition.x, this.barrelPosition.y);
                        doritoSmoke.depth = (Depth)((float)(0.899999976158142 + (double)1 * (1.0 / 1000.0)));
                        doritoSmoke.fly = new Vec2(Rando.Float(-2f, 2f) + this.barrelVector.x * Rando.Float(2f, 6f), Rando.Float(-2f, 2f) + this.barrelVector.y * Rando.Float(2f, 6f));
                        Level.Add((Thing)doritoSmoke);
                        break;
                    case 1:
                        glassesSmoke glassesSmoke = new glassesSmoke((float)this.barrelPosition.x, this.barrelPosition.y);
                        glassesSmoke.depth = (Depth)((float)(0.899999976158142 + (double)1 * (1.0 / 1000.0)));
                        glassesSmoke.fly = new Vec2(Rando.Float(-2f, 2f) + this.barrelVector.x * Rando.Float(2f, 6f), Rando.Float(-2f, 2f) + this.barrelVector.y * Rando.Float(2f, 6f));
                        Level.Add((Thing)glassesSmoke);
                        break;
                    case 2:
                        mlgSmoke mlgSmoke = new mlgSmoke((float)this.barrelPosition.x, this.barrelPosition.y);
                        mlgSmoke.depth = (Depth)((float)(0.899999976158142 + (double)1 * (1.0 / 1000.0)));
                        mlgSmoke.fly = new Vec2(Rando.Float(-2f, 2f) + this.barrelVector.x * Rando.Float(2f, 6f), Rando.Float(-2f, 2f) + this.barrelVector.y * Rando.Float(2f, 6f));
                        Level.Add((Thing)mlgSmoke);
                        break;
                    case 3:
                        illuminatiSmoke illuminatiSmoke = new illuminatiSmoke((float)this.barrelPosition.x, this.barrelPosition.y);
                        illuminatiSmoke.depth = (Depth)((float)(0.899999976158142 + (double)1 * (1.0 / 1000.0)));
                        illuminatiSmoke.fly = new Vec2(Rando.Float(-2f, 2f) + this.barrelVector.x * Rando.Float(2f, 6f), Rando.Float(-2f, 2f) + this.barrelVector.y * Rando.Float(2f, 6f));
                        Level.Add((Thing)illuminatiSmoke);
                        break;
                    default:
                        doritoSmoke doritoSmoke2 = new doritoSmoke((float)this.barrelPosition.x, this.barrelPosition.y);
                        doritoSmoke2.depth = (Depth)((float)(0.899999976158142 + (double)1 * (1.0 / 1000.0)));
                        doritoSmoke2.fly = new Vec2(Rando.Float(-2f, 2f) + this.barrelVector.x * Rando.Float(2f, 6f), Rando.Float(-2f, 2f) + this.barrelVector.y * Rando.Float(2f, 6f));
                        Level.Add((Thing)doritoSmoke2);
                        break;
                }
            }
        }

        public override void Draw()
        {
            float angle = this.angle;
            bool rotateReload = true;
            Duck owner = this.owner as Duck;
            if (owner != null)
            {
                rotateReload = false;
                if (owner.hSpeed == 0 && owner.grounded)
                {
                    rotateReload = true;
                }
            }
            if (rotateReload)
            {
                if ((int)this.offDir > 0)
                    this.angle = this.angle - this._angleOffset;
                else
                    this.angle = this.angle + this._angleOffset;
            }
            base.Draw();
            this.angle = angle;
        }
    }
}
