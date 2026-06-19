
namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|hats")]
    public class TurtleHat : Hat, IQuackOverrideEquipment
    {
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");
        public StateBinding netSFX_popStateBinding = new NetSoundBinding("netSFX_pop");

        public NetSoundEffect netSFX_pop = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\pop")
        });

        public int _cooldown;

        private int _priority;

        public int priority
        {
            get
            {
                return _priority;
            }
        }

        public int cooldown
        {
            get
            {
                return _cooldown;
            }
        }

        public TurtleHat(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Turtle Hat";

            // collision & sprite settings
            _pickupSprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\turtleHat"), 32, 32);
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\turtleHat"), 32, 32);
            graphic = sprite;
            center = new Vec2(16f, 14f);
            _collisionSize = new Vec2(13f, 8f);
            _collisionOffset = new Vec2(-6f, -4f);

            // equipment settings
            _equippedThickness = 0.1f;
            _priority = 2;
        }

        public override void Update()
        {
            base.Update();

            if (equippedDuck != null
                && equippedDuck.inputProfile.Pressed(Triggers.Quack)
                && _cooldown == 0
                && !duck.immobilized
                && !duck.inNet
                && !duck.sliding
                && Level.CheckLine<Block>(equippedDuck.position, equippedDuck.position + (8f * equippedDuck.velocity), this) == null)
            {
                if (isServerForObject)
                    netSFX_pop.Play();
                equippedDuck.position = equippedDuck.position + (8f * equippedDuck.velocity);
                _cooldown = 40;
            }

            if (_cooldown > 0)
            {
                _cooldown--;
                if (isServerForObject && _cooldown == 0)
                    Level.Add(new QuacktionFlash(x, y, this));
            }
        }
    }
}