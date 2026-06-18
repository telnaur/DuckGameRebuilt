namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|props")]
    public class Anvil : Holdable, IPlatform
    {
        public StateBinding netSFX_tingStateBinding = new NetSoundBinding("netSFX_ting");

        public NetSoundEffect netSFX_ting = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\anvilTing")
        });

        private SpriteMap sprite;

        public Anvil(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Anvil";

            // general settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\anvil"), 20, 16);
            graphic = sprite;
            center = new Vec2(10f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            thickness = 10f;
            weight = 10f;
            flammable = 0f;
            friction = 0.25f;
            physicsMaterial = PhysicsMaterial.Metal;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            return false;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            Level.Add(MetalRebound.New(hitPos.x, hitPos.y, (double)bullet.travelDirNormalized.x > 0.0 ? 1 : -1));
            SFX.Play(Mod.GetPath<UffMod>("SFX\\anvilTing"));
            hitPos -= bullet.travelDirNormalized;
            for (int index = 0; index < 3; ++index)
                Level.Add(Spark.New(hitPos.x, hitPos.y, bullet.travelDirNormalized, 0.02f));
            return thickness > bullet.ammo.penetration;
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is IPlatform && impactPowerV >= 1f)
                netSFX_ting.Play(1f, Rando.Float(0.15f));
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            /* do nothing */
        }

        public override void Update()
        {
            gravMultiplier = 2f;
            base.Update();
        }
    }
}
