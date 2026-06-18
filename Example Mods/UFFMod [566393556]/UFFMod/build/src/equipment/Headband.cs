
namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|hats")]
    public class Headband : Hat, IQuackOverrideEquipment
    {
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");

        public int _cooldown;

        public int cooldown
        {
            get
            {
                return _cooldown;
            }
        }

        public Headband(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Headband";

            // collision & sprite settings
            _pickupSprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\headbandPickup"), 32, 32);
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\headband"), 32, 32);
            graphic = sprite;
            center = new Vec2(15f, 19f);
            _collisionSize = new Vec2(10f, 6f);
            _collisionOffset = new Vec2(-5f, -3f);

            // equipment settings
            _equippedThickness = 0.1f;
        }

        public override void Update()
        {
            base.Update();

            if (equippedDuck != null
                && equippedDuck.inputProfile.Pressed(Triggers.Quack)
                && _cooldown == 0
                && !duck.immobilized
                && !duck.inNet
                && !duck.sliding)
            {
                SFX.Play(Mod.GetPath<UffMod>("SFX\\bwoosh"), 1f, Rando.Float(-0.2f, 0.2f), 0f, false);
                if (isServerForObject)
                    Level.Add(new Hadouken(x + offDir * 8f, y + 8f, offDir, equippedDuck));
                _cooldown = 120;
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