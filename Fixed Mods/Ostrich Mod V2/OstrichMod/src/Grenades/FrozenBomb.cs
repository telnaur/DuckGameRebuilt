using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | FireAndWater")]
    class FrozenBomb : Gun
    {
        public StateBinding _timeBeforeExploteStateBinding = new StateBinding("_timeBeforeExplote");
        // pin must be networked so ghost clients transition from pin=true to pin=false
        // and trigger their local IceCloud spawn when the countdown expires.
        public StateBinding _pinStateBinding = new StateBinding("pin");
        public int _timeBeforeExplote;
        public bool pin = true;
        private SpriteMap _sprite;
        public int timeBeforeExplote
        {
            get
            {
                return _timeBeforeExplote;
            }
        }

        public FrozenBomb(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Frozen Bomb";
            this.ammo = 1;
            this._ammoType = new ATMin();
            this._type = "gun";
            this._sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("FrozenBomb"), 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-4f, -5f);
            this.collisionSize = new Vec2(8f, 10f);
            this._barrelOffsetTL = new Vec2(7f, 8f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._timeBeforeExplote = 120;
            this._fireSound = GetPath("SFX/drillklang");
        }

        public override void Update()
        {
            base.Update();
            if (pin == true)
            {
                return;
            }
            else if (pin == false)
            {
                _timeBeforeExplote--;
            }
            if (_timeBeforeExplote <= 0)
            {
                // Every client creates its own local IceClouds for the visual effect.
                // pin is now a StateBinding so ghost clients also see pin=false and reach
                // this path. Game-state effects (duck slowing, IceBlock creation) are
                // gated by isServerForObject inside IceCloud.ToxicOnDucks().
                for (int i = 0; i < 4; i++)
                {
                    Level.Add(new IceCloud(x, y, 2.5f + Rando.Float(1f)));
                    Level.Add(new IceCloud(x, y + 25, 2.5f + Rando.Float(1f)));
                    Level.Add(new IceCloud(x, y - 25, 2.5f + Rando.Float(1f)));
                    Level.Add(new IceCloud(x + 25, y, 2.5f + Rando.Float(1f)));
                    Level.Add(new IceCloud(x - 25, y, 2.5f + Rando.Float(1f)));
                }
                Level.Remove((this));
            }
        }

        public override void OnPressAction()
        {
            if( pin == true)
            {
                pin = false;
                this._sprite.frame = this.pin ? 0 : 1;
                GrenadePin grenadePin = new GrenadePin(this.x, this.y);
                grenadePin.hSpeed = (float)-this.offDir * (1.5f + Rando.Float(0.5f));
                grenadePin.vSpeed = -2f;
                Level.Add((Thing)grenadePin);
                SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);
            }
        }
    }
}