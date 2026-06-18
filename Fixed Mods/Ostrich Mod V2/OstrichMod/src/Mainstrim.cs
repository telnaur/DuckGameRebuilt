using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Aeris")]
    internal class Mainstrim : Gun
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

        public Mainstrim(float xval, float yval)
      : base(xval, yval)
        {
            this._editorName = "Mainstrim";
            this.ammo = 999;
            this._ammoType = new ATLaser();
            this._fireWait = 0.1f;
            this.graphic = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Mainstrim"),29, 13);
            this.center = new Vec2(4f, 6f);
            this.collisionOffset = new Vec2(-4f, -6f);
            this._ammoType.range = 15f;
            this.collisionSize = new Vec2(29f, 13f);
            this._barrelOffsetTL = new Vec2(27f, 6f);
            this._kickForce = 16f;
            this._holdOffset = new Vec2(0f, 0.0f);
            this._weight = 0f;
            this._numBulletsPerFire = 13;
            this._ammoType.accuracy = 0.5f;
            this._fireSound = GetPath("sounds/drop");

        }

        public override void Update()
        {
            _aimAngle = duck != null ? offDir * -(float)Math.PI / 0.9f : 0f;
            base.Update();
        }
    }
}