using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Virgo")]
    internal class Pogo : Gun
    {
        public StateBinding _aimAngleState = new StateBinding("_aimAngle");

        public float _aimAngle;

        public override float angle
        {
            get
            {
                return base.angle + _aimAngle;
            }
            set
            {
                _angle = value;
            }
        }

        public Pogo(float xval, float yval)
      : base(xval, yval)
        {
            this._editorName = "Pogo";
            this.ammo = 999;
            this._ammoType = new ATShrapnel();
            this._fireWait = 5f;
            this.graphic = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Pogo"),29, 13);
            this.center = new Vec2(4f, 6f);
            this.collisionOffset = new Vec2(-4f, -6f);
            this.collisionSize = new Vec2(29f, 9f);
            this._barrelOffsetTL = new Vec2(27f, 6f);
            this._kickForce = 12f;
            this._holdOffset = new Vec2(0f, 0.0f);
            this._weight = 0f;
            this._fireSound = GetPath("sounds/drop");

        }

        public override void Update()
        {
            _aimAngle = duck != null ? offDir * -(float)Math.PI / -2f : 0f;
            base.Update();
        }
    }
}