using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Western")]
    class StickyGrenade : Gun
    {
        public StateBinding _timeBeforeExploteStateBinding = new StateBinding("_timeBeforeExplote");
        public int _timeBeforeExplote;
        public bool pin = true;
        public Duck stickedDuck;
        public bool collideWithDuck = false;
        private SpriteMap _sprite;
        public int timeBeforeExplote
        {
            get
            {
                return _timeBeforeExplote;
            }
        }

        public StickyGrenade(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Sticky Grenade";
            this.ammo = 1;
            this._ammoType = new ATMin();
            this._type = "gun";
            this._sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("StickyGrenade"), 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-4f, -5f);
            this.collisionSize = new Vec2(8f, 10f);
            this._barrelOffsetTL = new Vec2(7f, 8f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._timeBeforeExplote = 150;
            this._fireSound = GetPath("SFX/drillklang");
        }
        public override void CheckIfHoldObstructed()
        {
            Duck duck = this.owner as Duck;
            if (duck != null)
            {
                duck.holdObstructed = false;
                this.gravMultiplier = 1f;
                this.bouncy = 0.4f;
                this.friction = 0.05f;
            }
        }

        public override void Update()
        {
            base.Update();
            if(collideWithDuck == true && stickedDuck != null)
            {
                this.position = stickedDuck.position;
                this.vSpeed = 0.0f;
                this.hSpeed = 0.0f;
            }
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
                GrenadeExplosion exp = new GrenadeExplosion(0, 0);
                exp.position = Offset(this._barrelOffsetTL);
                Level.Add((Thing)exp);
                Level.Remove((this));
            }
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (pin == false && (with is Block || with is Door || with is Window))
            {
                this.vSpeed = 0.0f;
                this.hSpeed = 0.0f;
                this.gravMultiplier = 0.0f;
                this.bouncy = 0f;
                this.friction = 200f;
            }
            if (pin == false && with is Duck || with is Ragdoll)
            {
                stickedDuck = with as Duck;
                collideWithDuck = true;
                this.gravMultiplier = 0.0f;
                this.bouncy = 0f;
                this.friction = 200f;
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
                SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);
            }
        }
    }
}