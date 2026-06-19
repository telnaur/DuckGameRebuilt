using System;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|explosives")]
    public class Mortar : Gun
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

        public Mortar(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // collision & sprite settings
            graphic = new SpriteMap(Mod.GetPath<UffMod>("weapons\\mortar"), 25, 10);
            _center = new Vec2(12f, 5f);
            _barrelOffsetTL = new Vec2(24f, 6f);
            _collisionSize = new Vec2(20f, 5f);
            _collisionOffset = new Vec2(-10f, 0f);

            // weapon settings
            ammo = 6;
            _ammoType = new ATGrenade();
            _ammoType.sprite = new Sprite(Mod.GetPath<UffMod>("weapons\\mortarShell"));
            _fireSound = "deepMachineGun";
        }

        public override void Update()
        {
            _aimAngle = duck != null ? offDir * -(float)Math.PI / 4f : 0f;
            base.Update();
        }
    }
}