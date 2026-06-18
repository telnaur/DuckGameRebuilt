using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    class ATMucosa : Gun
    {
        public StateBinding _timeBeforeExploteStateBinding = new StateBinding("_timeBeforeExplote");
        public int _timeBeforeExplote;
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

        public ATMucosa(float xval, float yval) : base(xval, yval)
        {
            this.canPickUp = false;
            this._editorName = "ATMucosa";
            this.ammo = 1;
            this._ammoType = new ATMin();
            this._type = "gun";
            this._sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Mucosa"), 6, 6);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(3f, 3f);
            this.collisionOffset = new Vec2(-3f, -3f);
            this.collisionSize = new Vec2(6f, 6f);
            this._barrelOffsetTL = new Vec2(3f, 3f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._timeBeforeExplote = 180;
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
            _timeBeforeExplote--;

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
            if (with is Block || with is Door || with is Window || with is ATMucosa)
            {
                this.vSpeed = 0.0f;
                this.hSpeed = 0.0f;
                this.gravMultiplier = 0.0f;
                this.bouncy = 0f;
                this.friction = 200f;
            }
            if (with is Duck)
            {
                stickedDuck = with as Duck;
                collideWithDuck = true;
                this.gravMultiplier = 0.0f;
                this.bouncy = 0f;
                this.friction = 200f;
            }
            SFX.Play(GetPath("SFX/balloonpop"), 1f, 0.0f, 0.0f, false);
        }

        public override void OnPressAction()
        {
        }
    }
}