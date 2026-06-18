using System;

namespace DuckGame.UFFMod
{
    internal class JadeShot : Thing, ITeleport
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _fireSpeedStateBinding = new CompressedFloatBinding("_fireSpeed");
        public StateBinding _fireAngleStateBinding = new CompressedFloatBinding("_fireAngle");
        public StateBinding _hurtOwnerStateBinding = new StateBinding("_hurtOwner");

        public float _fireSpeed;
        public float _fireAngle;
        public int _hurtOwner;

        private SpriteMap sprite;

        public JadeShot(float xpos, float ypos, float fS, float fA)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\jadePulse"), 28, 28);
            sprite.AddAnimation("pulse", 0.25f, true, 0, 1, 2, 3, 4, 5, 6);
            sprite.SetAnimation("pulse");
            sprite.CenterOrigin();
            graphic = sprite;
            _fireSpeed = fS;
            _fireAngle = fA;
            xscale = yscale = 0.33f;
            center = new Vec2(14f, 14f);
            _collisionOffset = new Vec2(-12f, -12f);
            _collisionSize = new Vec2(24f, 24f);
            depth = 1f;
        }

        public override void Update()
        {
            if (isServerForObject && (x > Level.current.bottomRight.x + 200 || x < Level.current.topLeft.x - 200 || y > Level.current.bottomRight.y + 200 || y < Level.current.topLeft.y - 200))
                Level.Remove(this);

            xscale = yscale = MathHelper.Lerp(yscale, 1f, 0.04f);
            sprite.CenterOrigin();

            x += _fireSpeed * (float)Math.Cos((Math.PI * _fireAngle) / 180);
            y += _fireSpeed * (float)Math.Sin((Math.PI * _fireAngle) / 180);

            if (_hurtOwner < 10)
                _hurtOwner++;

            foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(position, 14f * yscale))
            {
                if (_hurtOwner < 10 && physicsObject == owner)
                    continue;
                if (isLocal)
                    Thing.Fondle(physicsObject, DuckNetwork.localConnection);
                physicsObject.Destroy(new DTImpact(this));
                physicsObject.sleeping = false;
            }

            base.Update();
        }
    }
}
