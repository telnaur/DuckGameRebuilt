using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Virgo")]
    class Cocacola : Gun
    {
        public StateBinding _timeBeforeExploteStateBinding = new StateBinding("_timeBeforeExplote");
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

        public Cocacola(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Cocacola";
            this.ammo = 1;
            this._ammoType = new ATMin();
            this._type = "gun";
            this._sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Cocacola"), 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-4f, -8f);
            this.collisionSize = new Vec2(8f, 16f);
            this._barrelOffsetTL = new Vec2(7f, 8f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._timeBeforeExplote = 120;
            this._fireSound = GetPath("SFX/cocacola1");
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
                for (int i = 0; i < 30; i++)
                {
                    Level.Add(new ExtinguisherSmoke(x, y - 15));
                    Level.Add(new ExtinguisherSmoke(x + 15, y - 15));
                    Level.Add(new ExtinguisherSmoke(x - 15, y - 15));
                    Level.Add(new ExtinguisherSmoke(x, y - 15));
                    Level.Add(new ExtinguisherSmoke(x + 25, y - 15));
                    Level.Add(new ExtinguisherSmoke(x - 25, y - 15));
                    Level.Add(new ExtinguisherSmoke(x, y - 15));
                    Level.Add(new ExtinguisherSmoke(x + 35, y - 15));
                    Level.Add(new ExtinguisherSmoke(x - 35, y - 15));
                    SFX.Play(GetPath("SFX/cocacola2"), 0.4f, 0f, 0f, false);
                }
                Level.Remove((this));
            }
        }

        public override void OnPressAction()
        {
            if(pin == true)
            {
                pin = false;
                this._sprite.frame = this.pin ? 0 : 1;
                GrenadePin grenadePin = new GrenadePin(this.x, this.y);
                grenadePin.hSpeed = (float)-this.offDir * (1.5f + Rando.Float(0.5f));
                grenadePin.vSpeed = -2f;
                Level.Add((Thing)grenadePin);
                SFX.Play(GetPath("SFX/cocacola1"), 0.4f, 0f, 0f, false);
            }

        }
    }
}