
namespace DuckGame.UFFMod
{
    internal class Hadouken : Thing
    {
        public StateBinding _theDuckStateBinding = new StateBinding("_theDuck");
        public StateBinding _countStateBinding = new StateBinding("_count");
        public StateBinding _offDirStateBinding = new StateBinding("offDir");
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        public Duck _theDuck;
        public int _count;

        private SpriteMap sprite;

        public Hadouken(float xpos, float ypos, sbyte oD, Duck d)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\hadouken"), 18, 12);
            sprite.AddAnimation("*shazoom*", 0.5f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            sprite.SetAnimation("*shazoom*");
            graphic = sprite;
            offDir = oD;
            _theDuck = d;
            center = new Vec2(9f, 6f);
            _collisionOffset = new Vec2(-9f, -6f);
            _collisionSize = new Vec2(18f, 12f);
        }

        public override void Update()
        {
            if (isServerForObject && (x > Level.current.bottomRight.x + 200 || x < Level.current.topLeft.x - 200 || _count >= 60))
                Level.Remove(this);

            _count++;

            sprite.flipH = offDir > 0 ? false : true;
            x += offDir * 4f;

            if (alpha >= 0.6f)
                foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(position, 6f))
                {
                    Equipment e = materialThing as Equipment;
                    if (materialThing.isServerForObject && materialThing.active && materialThing.visible && materialThing != _theDuck && !(e != null && _theDuck._equipment.Contains(e)) && !(_theDuck.holdObject != null && _theDuck.holdObject == materialThing) && !(materialThing is FluidPuddle))
                    {
                        if (!materialThing.onFire && materialThing.flammable > 0f)
                            materialThing.Burn(materialThing.position, owner);
                        materialThing.DoHeatUp(0.1f);
                    }
                    if (materialThing is Block)
                        _count = 60; 
                }

            base.Update();
        }
    }
}
