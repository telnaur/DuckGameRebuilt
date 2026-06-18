using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Aeris")]
    class Skypenance : Gun
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

        public Skypenance(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Skypenance";
            this.ammo = 1;
            this._ammoType = new ATMin();
            this._type = "gun";
            this._sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Skypenance"), 16, 16);
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
                if (this.isServerForObject)
                {
                    SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f, 0f, false);
                    Vec2 pos = this.Offset(this.barrelOffset);

                    DeathBeam deathBeam = new DeathBeam(pos, this.Offset(this.barrelOffset + new Vec2(1200f, 0.0f)) - pos);
                    DeathBeam deathBeam1 = new DeathBeam(pos, this.Offset(this.barrelOffset + new Vec2(-1200f, 0.0f)) - pos);
                    DeathBeam deathBeam2 = new DeathBeam(pos, this.Offset(this.barrelOffset + new Vec2(0f, 1200f)) - pos);
                    DeathBeam deathBeam3 = new DeathBeam(pos, this.Offset(this.barrelOffset + new Vec2(0f, -1200f)) - pos);

                    deathBeam.isLocal = this.isServerForObject;
                    deathBeam1.isLocal = this.isServerForObject;
                    deathBeam2.isLocal = this.isServerForObject;
                    deathBeam3.isLocal = this.isServerForObject;

                    Level.Add((Thing)deathBeam);
                    Level.Add((Thing)deathBeam1);
                    Level.Add((Thing)deathBeam2);
                    Level.Add((Thing)deathBeam3);

                    Level.Remove((this));
                }
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