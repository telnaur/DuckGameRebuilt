namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|props")]
    public class Golduck : Holdable, IPlatform
    {
        public StateBinding _theDuckStateBinding = new StateBinding("_theDuck");

        public Duck _theDuck;

        private SpriteMap sprite;

        public Golduck(float xpos, float ypos, Duck theDuck = null)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Golden Duck Statue";

            // general settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\golduck"), 16, 24);
            graphic = sprite;
            center = new Vec2(8f, 12f);
            collisionOffset = new Vec2(-6f, -12f);
            collisionSize = new Vec2(12f, 24f);
            depth = -0.5f;
            thickness = 5f;
            weight = 10f;
            flammable = 0f;
            friction = 0.25f;
            physicsMaterial = PhysicsMaterial.Metal;

            // defaults
            _theDuck = theDuck;
        }
    }
}
