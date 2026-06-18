using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Western")]
    public class Deathsnipe : Gun
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

        public Deathsnipe(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Snipe2"), 36, 10);
            graphic = sprite;
            _center = new Vec2(12f, 5f);
            _collisionSize = new Vec2(36f, 10f);
            _collisionOffset = new Vec2(-10f, -4f);
            _holdOffset = new Vec2(-3f, -1f);
            _barrelOffsetTL = new Vec2(36f, 4f);
            _laserOffsetTL = new Vec2(21f, 6f);

            // weapon settings
            ammo = 12;
            _ammoType = new ATHighCalSniper();
            this._fireSound = GetPath("sounds/saturn");
            _fireWait = 5f;
            _kickForce = 2f;
            _weight = 5f;
            laserSight = false;
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
                    laserSight = true;

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
                laserSight = false;
            }

            _aimAngle = Maths.DegToRad(_fireAngle);
            _aimAngle *= offDir;

            base.Update();
        }
    }
}
