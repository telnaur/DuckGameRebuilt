using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Rifles")]
    public class leeEnfield : Gun
    {
        public StateBinding _loadStateBinding = new StateBinding("_loadState", -1, false);
        public StateBinding _netClickBinding = (StateBinding)new NetSoundBinding("_netClick");
        public StateBinding _netSwipeBinding = (StateBinding)new NetSoundBinding("_netSwipe");
        public StateBinding _netSwipe2Binding = (StateBinding)new NetSoundBinding("_netSwipe2");
        public StateBinding _netLoadBinding = (StateBinding)new NetSoundBinding("_netLoad");
        public NetSoundEffect _netClick = new NetSoundEffect(new string[1] { "click" })
        {
            volume = 1f,
            pitch = 0.5f
        };
        public NetSoundEffect _netSwipe = new NetSoundEffect(new string[1] { "swipe" })
        {
            volume = 0.6f,
            pitch = -0.3f
        };
        public NetSoundEffect _netSwipe2 = new NetSoundEffect(new string[1] { "swipe" })
        {
            volume = 0.7f
        };
        public NetSoundEffect _netLoad = new NetSoundEffect(new string[1] { "shotgunLoad" });
        public int _loadState = -1;
        public float _angleOffset;
        private SpriteMap _sprite;

        public leeEnfield(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 2;
            this._ammoType = (AmmoType)new ATShrapnel();
            this._ammoType.range = 470f;
            this._ammoType.rangeVariation = 70f;
            this._ammoType.accuracy = 0.5f;
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("leeEnfield"), 41, 11, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(19f, 6f);
            this.collisionOffset = new Vec2(-8f, -6f);
            this.collisionSize = new Vec2(16f, 10f);
            this._barrelOffsetTL = new Vec2(38f, 3f);
            this._fireSound = "shotgun";
            this._kickForce = 2f;
            this._manualLoad = true;
            this._holdOffset = new Vec2(2f, 2f);
            this._editorName = "Lee Enfield";
            this.editorTooltip = "Reload time is not too bad, but its accuracy leaves a lot to be desired.";
        }

        public override void Update()
        {
            base.Update();
            this._sprite.frame = this.ammo <= 1 ? 1 : 0;
            if (this._loadState <= -1)
                return;
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
                        this._netSwipe.Play(1f, 0.0f);
                }
                else
                    SFX.Play("swipe", 0.6f, -0.3f, 0.0f, false);
                ++this._loadState;
            }
            else if (this._loadState == 1)
            {
                if ((double)this._angleOffset < 0.159999996423721)
                    this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.2f, 0.08f);
                else
                    ++this._loadState;
            }
            else if (this._loadState == 2)
            {
                this.handOffset.y -= 0.28f;
                if ((double)this.handOffset.y >= -4.0)
                    return;
                ++this._loadState;
                this.ammo = 2;
                this.loaded = false;
                if (Network.isActive)
                {
                    if (!this.isServerForObject)
                        return;
                    this._netLoad.Play(1f, 0.0f);
                }
                else
                    SFX.Play("shotgunLoad", 1f, 0.0f, 0.0f, false);
            }
            else if (this._loadState == 3)
            {
                this.handOffset.y += 0.15f;
                if ((double)this.handOffset.y < 0.0)
                    return;
                ++this._loadState;
                this.handOffset.y = 0.0f;
                if (Network.isActive)
                {
                    if (!this.isServerForObject)
                        return;
                    this._netSwipe2.Play(1f, 0.0f);
                }
                else
                    SFX.Play("swipe", 0.7f, 0.0f, 0.0f, false);
            }
            else
            {
                if (this._loadState != 4)
                    return;
                if ((double)this._angleOffset > 0.0399999991059303)
                {
                    this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.0f, 0.08f);
                }
                else
                {
                    this._loadState = -1;
                    this.loaded = true;
                    this._angleOffset = 0.0f;
                    if (Network.isActive)
                    {
                        if (!this.isServerForObject)
                            return;
                        this._netClick.Play(1f, 0.0f);
                    }
                    else
                        SFX.Play("click", 1f, 0.5f, 0.0f, false);
                }
            }
        }

        public override void OnPressAction()
        {
            if (this.loaded && this.ammo > 1)
            {
                base.OnPressAction();
                for (int index = 0; index < 4; ++index)
                    Level.Add((Thing)Spark.New((int)this.offDir > 0 ? this.x - 9f : this.x + 9f, this.y - 6f, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.05f));
                for (int index = 0; index < 4; ++index)
                    Level.Add((Thing)SmallSmoke.New(this.barrelPosition.x + (float)this.offDir * 4f, this.barrelPosition.y));
                this.ammo = 1;
            }
            else
            {
                if (this._loadState != -1)
                    return;
                this._loadState = 0;
            }
        }

        public override void Draw()
        {
            float angle = this.angle;
            if ((int)this.offDir > 0)
                this.angle = this.angle - this._angleOffset;
            else
                this.angle = this.angle + this._angleOffset;
            base.Draw();
            this.angle = angle;
        }
    }
}
