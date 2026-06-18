using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Slo-Mo")]
    public class slomoSniper : Gun
    {
        public StateBinding _loadStateBinding = new StateBinding("_loadState", -1, false, false);
        public StateBinding _angleOffsetBinding = new StateBinding("_angleOffset", -1, false, false);
        public StateBinding _netLoadBinding = (StateBinding)new NetSoundBinding("_netLoad");
        public NetSoundEffect _netLoad = new NetSoundEffect(new string[1] { "loadSniper" });
        public int _loadState = -1;
        public int _loadAnimation = -1;
        public float _angleOffset;
        private bool laserSightGreen;
        private Tex2D _laserTex;
        private bool _laserInit;

        public slomoSniper(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 3;
            this._ammoType = (AmmoType)new slomoATSniper();
            this._ammoType.immediatelyDeadly = true;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("slomoSniper"), 0.0f, 0.0f);
            this.center = new Vec2(16f, 4f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(30f, 3f);
            this._fireSound = "sniper";
            this._kickForce = 5f;
            this._fireRumble = RumbleIntensity.Light;
            this.laserSight = false;
            this.laserSightGreen = true;
            this._laserOffsetTL = new Vec2(32f, 3.5f);
            this._manualLoad = true;
            this._editorName = "Slo-Mo Sniper";
            this.editorTooltip = "These bullets are so slooow!";
        }

        public override void Update()
        {
            base.Update();
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
            if (this.loaded && this.owner != null && this._loadState == -1)
                this.laserSightGreen = true;
            else
                this.laserSightGreen = false;
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
            }
        }

        public override void Draw()
        {
            float angle = this.angle;
            if (this.offDir > (sbyte)0)
                this.angle -= this._angleOffset;
            else
                this.angle += this._angleOffset;
            base.Draw();
            this.angle = angle;

            if (this.laserSightGreen && this.held)
            {
                ATTracer atTracer = new ATTracer();
                atTracer.range = 2000f;
                float ang = this.angleDegrees * -1f;
                if (this.offDir < (sbyte)0)
                    ang += 180f;
                Vec2 vec2 = this.Offset(this.laserOffset);
                atTracer.penetration = 0.4f;
                this._wallPoint = new Bullet(vec2.x, vec2.y, (AmmoType)atTracer, ang, this.owner, false, -1f, true, true).end;
                this._laserInit = true;
            }
        }

        public override void DoUpdate()
        {
            if (this.laserSightGreen && this._laserTex == null)
            {
                this._laserTex = Content.Load<Tex2D>("pointerLaser");
            }
            base.DoUpdate();
        }

        public override void DrawGlow()
        {
            if (this.laserSightGreen && this.held && (this._laserTex != null && this._laserInit))
            {
                float num = 1f;
                if (!Options.Data.fireGlow)
                    num = 0.4f;
                Vec2 p1 = this.Offset(this.laserOffset);
                float length = (p1 - this._wallPoint).length;
                float val1 = 100f;
                if (this.ammoType != null)
                    val1 = this.ammoType.range;
                Vec2 normalized = (this._wallPoint - p1).normalized;
                Vec2 vec2 = p1 + normalized * Math.Min(val1, length);
                Graphics.DrawTexturedLine(this._laserTex, p1, vec2, Color.Green * num, 0.5f, this.depth - 1);
                if ((double)length > (double)val1)
                {
                    for (int index = 1; index < 4; ++index)
                    {
                        Graphics.DrawTexturedLine(this._laserTex, vec2, vec2 + normalized * 2f, Color.Green * (float)(1.0 - (double)index * 0.200000002980232) * num, 0.5f, this.depth - 1);
                        vec2 += normalized * 2f;
                    }
                }
                if (this._sightHit != null && (double)length < (double)val1)
                {
                    this._sightHit.alpha = num;
                    this._sightHit.color = Color.Green * num;
                    Graphics.Draw(this._sightHit, this._wallPoint.x, this._wallPoint.y);
                }
            }
            base.DrawGlow();
        }
    }
}
