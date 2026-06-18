namespace DuckGame.UFFMod
{
    internal class FirebarBall : Thing
    {
        public int order;
        private SpriteMap sprite;

        public FirebarBall(float xpos, float ypos, int ord, Firebar own)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\hazards\\firebarBit"), 6, 6);
            sprite.AddAnimation("bwahahaha", 0.25f, true, 0, 1, 2, 3);
            sprite.SetAnimation("bwahahaha");
            graphic = sprite;
            center = new Vec2(3f, 3f);
            order = ord;
            owner = own;
            depth = 1f;
        }

        public override void Update()
        {
            foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(position, 2f))
            {
                if (materialThing.isServerForObject && !(materialThing is FluidPuddle))
                {
                    materialThing.DoHeatUp(0.05f);
                    if(!materialThing.onFire && materialThing.flammable > 0f)
                        materialThing.Burn(materialThing.position, owner);
                }
            }
            base.Update();
        }
    }
}
