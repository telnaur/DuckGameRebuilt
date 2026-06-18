using System;

namespace DuckGame.UFFMod
{
    internal class FireSprinkle : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _fireSpeedStateBinding = new CompressedFloatBinding("_fireSpeed");
        public StateBinding _fireAngleStateBinding = new CompressedFloatBinding("_fireAngle");
        public StateBinding _theWandStateBinding = new StateBinding("_theWand");

        public FireWand _theWand;
        public float _fireSpeed;
        public float _fireAngle;

        private SpriteMap sprite;

        public FireSprinkle(float xpos, float ypos, float fS, float fA, FireWand tW)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\fireWandParticle"), 16, 16);
            sprite.AddAnimation("sch", 0.5f, false, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 5, 6, 7);
            sprite.SetAnimation("sch");
            graphic = sprite;
            angle = Rando.Float(2 * (float)Math.PI);
            center = new Vec2(8f, 8f);
            depth = 0.5f;
            _fireSpeed = fS;
            _fireAngle = fA;
            _theWand = tW;
        }

        public override void Update()
        {
            if (isServerForObject && (sprite.finished || x > Level.current.bottomRight.x + 200 || x < Level.current.topLeft.x - 200))
                Level.Remove(this);

            x += _fireSpeed * (float)Math.Cos((Math.PI * _fireAngle) / 180);
            y += _fireSpeed * (float)Math.Sin((Math.PI * _fireAngle) / 180);

            if (_theWand != null)
            {
                if (Level.CheckCircle<Block>(position, 3f) != null)
                    Level.Remove(this);
                else
                    foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(position, 6f))
                    {
                        if (materialThing.isServerForObject && materialThing != _theWand && materialThing != _theWand.owner && !(materialThing is FluidPuddle))
                        {
                            if (!materialThing.onFire && materialThing.flammable > 0f && materialThing.heat > 0.5f)
                                materialThing.Burn(materialThing.position, _theWand.owner);
                            materialThing.DoHeatUp(0.01f);
                        }
                    }
            }

            base.Update();
        }
    }
}
