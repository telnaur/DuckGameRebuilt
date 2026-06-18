using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Western")]
    public class Canon : Gun
    {
        public StateBinding _aimAngleStateBinding = new StateBinding("_aimAngle");
        public StateBinding _fireAngleStateBinding = new StateBinding("_fireAngle");

        public float _aimAngle;
        public float _fireAngle;

        private bool aiming;

        private SpriteMap sprite;

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

        public Canon(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Canon"), 25, 12);
            graphic = sprite;
            _center = new Vec2(4f, 6f);
            _collisionSize = new Vec2(25f, 12f);
            _collisionOffset = new Vec2(-4f, -6f);
            _holdOffset = new Vec2(-2f, 2f);
            _barrelOffsetTL = new Vec2(23f, 5f);

            // weapon settings
            ammo = 12;
            _ammoType = new ATCannon();
            this._fireSound = "missile";
            _fireWait = 5f;
            _kickForce = 2f;
            _weight = 5f;
        }

        public override void OnPressAction()
        {
            if (ammo > 0 && duck != null)
            {
                aiming = true;
                duck.immobilized = true;
                duck.remoteControl = true;
            }
            else
                SFX.Play("click");
        }

        public override void OnReleaseAction()
        {

            Duck d = duck ?? prevOwner as Duck;
            if (d != null)
            {
                d.immobilized = false;
                d.remoteControl = false;
            }
            if (_wait == 0f && aiming)
                Fire();
            aiming = false;
        }

        public override void Update()
        {
            if (_wait == 0f && aiming)
            {
                if (duck != null)
                {

                    if (duck.inputProfile.Down(Triggers.Up) && _fireAngle > -45f)
                        _fireAngle -= 1.5f;
                    if (duck.inputProfile.Down(Triggers.Down) && _fireAngle < 45f)
                        _fireAngle += 1.5f;
                    if (duck.inputProfile.Released(Triggers.Grab))
                    {
                        duck.immobilized = false;
                        duck.remoteControl = false;
                        aiming = false;
                    }
                }
                else
                    aiming = false;
            }
            else
            {
                _fireAngle = MathHelper.Lerp(_fireAngle, 0f, 0.09f);
            }

            _aimAngle = Maths.DegToRad(_fireAngle);
            _aimAngle *= offDir;

            base.Update();
        }
    }
}
