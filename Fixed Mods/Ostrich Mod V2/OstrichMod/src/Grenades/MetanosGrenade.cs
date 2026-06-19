using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | BioLogic")]
    class MetanosGrenade : Gun
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

        public MetanosGrenade(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Metanos Grenade";
            this.ammo = 1;
            this._ammoType = new ATMin();
            this._type = "gun";
            this._sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("MetanosGrenade"), 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-4f, -5f);
            this.collisionSize = new Vec2(9f, 10f);
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
                // ToxicSmoke is now networked, so only the authority may spawn it; otherwise
                // every client would create its own set of ghosts (this Update runs on all
                // clients). The clouds replicate from here to everyone.
                if (isServerForObject)
                {
                    Level.Add(new ToxicSmoke(x, y, 9f + Rando.Float(1f)));
                    Level.Add(new ToxicSmoke(x, y + 25, 9f + Rando.Float(1f)));
                    Level.Add(new ToxicSmoke(x, y - 25, 9f + Rando.Float(1f)));
                    Level.Add(new ToxicSmoke(x + 25, y, 9f + Rando.Float(1f)));
                    Level.Add(new ToxicSmoke(x - 25, y, 9f + Rando.Float(1f)));
                }
                Level.Remove((this));
            }
        }

        public override void OnPressAction()
        {
            if (pin == true)
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