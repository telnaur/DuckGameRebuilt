using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Magic")]
    class Enderpearl : Gun
    {
        public bool pin = true;
        private SpriteMap _sprite;
        private Duck duck;

        public Enderpearl(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Enderpearl";
            this.ammo = 1;
            this._ammoType = new ATMin();
            this._type = "gun";
            this._sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Enderpearl"), 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this._barrelOffsetTL = new Vec2(0f, 0f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this.gravMultiplier = 0.8f;
            this._fireSound = GetPath("SFX/drillklang");
        }

        public override void Update()
        {
            base.Update();
            if (pin == true)
            {
                return;
            }
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is Block && pin == false)
            {
                this.duck.position = this.position;
                if (this.infinite == false) { Level.Remove((this)); }
                else
                {
                    duck = null;
                    pin = true;
                    this._sprite.frame = this.pin ? 1 : 0;
                }
            }
        }

        public override void OnPressAction()
        {
            if( pin == true)
            {
                duck = this.owner as Duck;
                pin = false;
                this._sprite.frame = this.pin ? 0 : 1;
                SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);
            }
        }
    }
}