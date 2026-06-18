using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    class ATXenon : Gun
    {
        public StateBinding _timeBeforeExploteStateBinding = new StateBinding("_timeBeforeExplote");
        public int _timeBeforeExplote;
        private SpriteMap _sprite;
        public int timeBeforeExplote
        {
            get
            {
                return _timeBeforeExplote;
            }
        }

        public ATXenon(float xval, float yval) : base(xval, yval)
        {
            this.canPickUp = false;
            this.ammo = 3;
            this._ammoType = new ATLaser();
            this._ammoType.bulletThickness = 0.8f;
            this._type = "gun";
            this._sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("XenonAt"), 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this._barrelOffsetTL = new Vec2(8f, 8f);
            this.bouncy = 0f;
            this.friction = 1000f;
            this.gravMultiplier = 1f;
            this._flare = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("no"), 1, 1, false);
            this._flare.center = new Vec2(0.0f, 0f);
            this._ammoType.accuracy = 360f;
            this._timeBeforeExplote = 120;
            this._kickForce = 0f;
            this._fireSound = GetPath("SFX/drillklang");
        }

        public override void Update()
        {
            base.Update();

            _vSpeed = -0.2f;
            _timeBeforeExplote--;

            if (_timeBeforeExplote <= 0)
            {
                if (this.ammo <= 0)
                {
                    Level.Remove((this));
                }
                this.Fire();
            }
        }
        public override void OnPressAction()
        {
        }
    }
}