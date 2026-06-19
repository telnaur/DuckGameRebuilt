using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.UFFMod
{
    [BaggedProperty("canSpawn", false)]
    internal class BilliardBall : PhysicsObject
    {
        // Without these, remote clients spawned the ghost at the muzzle with zero
        // velocity and it never moved (only the firer simulated the bounce). Replicate
        // the authority's position + velocity so the ball travels on every client,
        // matching the other projectile bits (JadeShot, GenocideOrb, etc.).
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _hSpeedStateBinding = new StateBinding("hSpeed");
        public StateBinding _vSpeedStateBinding = new StateBinding("vSpeed");

        public StateBinding _fadingStateBinding = new StateBinding("_fading");
        public StateBinding _hasImpactedStateBinding = new StateBinding("_hasImpacted");

        public bool _fading;
        public bool _hasImpacted;

        public SpriteMap sprite;

        public BilliardBall(float xpos, float ypos, float fireAngle, Thing own)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\billiardBall"), 8, 8);
            int[] array = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int ballNumber = Rando.Int(15);
            for (int i = 0; i < array.Count(); i++)
                array[i] += ballNumber * 10;
            sprite.AddAnimation("spin", 0.95f, true, array);
            sprite.SetAnimation("spin");
            graphic = sprite;
            center = new Vec2(4f, 4f);
            _collisionSize = new Vec2(8f, 8f);
            _collisionOffset = new Vec2(-4f, -4f);
            depth = -0.5f;
            thickness = 1f;
            angle = fireAngle;
            owner = own;
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            _hasImpacted = true;

            base.OnSolidImpact(with, from);
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            PhysicsObject physicsObject = with as PhysicsObject;
            if (physicsObject != null && !(_fading || with.weight < 5f || with == owner || (with is AutoPlatform && from != ImpactedFrom.Bottom) || with is BilliardBall || with is Gun || with is FeatherVolume || with is Teleporter || destroyed))
            {
                _hasImpacted = true;

                physicsObject.hSpeed += hSpeed / 6f;
                physicsObject.vSpeed += vSpeed / 6f;
                if (physicsObject is Duck && (((from == ImpactedFrom.Left || from == ImpactedFrom.Right) && Math.Abs(hSpeed) >= 4f)
                || ((from == ImpactedFrom.Top || from == ImpactedFrom.Bottom) && Math.Abs(vSpeed) >= 4f)))
                    physicsObject.Destroy(new DTImpact(this));

                hSpeed /= 3f;
            }

            base.OnImpact(with, from);
        }

        public override void Update()
        {
            angleDegrees += sprite.speed * 10f;
            while (angleDegrees >= 360)
                angleDegrees -= 360;

            sprite.speed = MathHelper.Lerp(sprite.speed, 0.1f, 0.015f);

            if (!_fading && ((Math.Abs(hSpeed) < 0.25f && Math.Abs(vSpeed) < 0.25f && _hasImpacted) || sprite.speed < 0.2f))
                _fading = true;
            
            if (_fading)
            {
                alpha = MathHelper.Lerp(alpha, 0f, 0.45f);
                if (alpha <= 0.1f)
                    Level.Remove(this);
            }
            
            base.Update();
        }
    }
}
