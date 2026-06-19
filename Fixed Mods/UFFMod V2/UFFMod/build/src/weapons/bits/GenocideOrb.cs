using System;

namespace DuckGame.UFFMod
{
    internal class GenocideOrb : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _updateExplosionStateBinding = new StateBinding("_updateExplosion");
        public StateBinding _fireSpeedStateBinding = new CompressedFloatBinding("_fireSpeed");
        public StateBinding _fireAngleStateBinding = new CompressedFloatBinding("_fireAngle");

        public int _updateExplosion;
        public float _fireSpeed;
        public float _fireAngle;

        public GenocideOrb(float xpos, float ypos, float fS, float fA)
            : base(xpos, ypos)
        {
            _fireSpeed = fS;
            _fireAngle = fA;
            _updateExplosion = 0;
        }

        public override void Update()
        {
            if (isServerForObject && (x > Level.current.bottomRight.x + 200 || x < Level.current.topLeft.x - 200))
                Level.Remove(this);

            x += _fireSpeed * (float)Math.Cos((Math.PI * _fireAngle) / 180);
            y += _fireSpeed * (float)Math.Sin((Math.PI * _fireAngle) / 180);

            if (_updateExplosion == 0)
            {
                Level.Add(new ExplosionPart(x + Rando.Float(-1f, 1f), y + Rando.Float(-1f, 1f), false));
                SFX.Play("explode", Rando.Float(0.12f, 0.15f), Rando.Float(0.2f), 0f, false);
                foreach (Block block in Level.CheckCircleAll<Block>(position, 8f))
                {
                    if (block is Door || block is VerticalDoor)
                    {
                        if (isLocal)
                            Thing.Fondle(block, DuckNetwork.localConnection);
                        if (Level.CheckLine<Block>(position, block.position, block) == null)
                        {
                            block.Destroy(new DTRocketExplosion(this));
                            Level.Remove(block);
                        }
                    }
                }
                foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(position, 8f))
                {
                    if (isLocal)
                        Thing.Fondle(physicsObject, DuckNetwork.localConnection);
                    physicsObject.Destroy(new DTImpact(this));
                    physicsObject.sleeping = false;
                    physicsObject.vSpeed = -2f;
                }
                _updateExplosion = 4;
            }
            else
                _updateExplosion--;

            base.Update();
        }
    }
}
