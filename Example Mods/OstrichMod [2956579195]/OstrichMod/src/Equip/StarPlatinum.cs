
namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Magic")]
    public class StarPlatinum : Hat, IQuackOverrideEquipment
    {
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");
        public bool active = false;
        public bool firstTime = true;

        public int _cooldown;

        public int cooldown
        {
            get
            {
                return _cooldown;
            }
        }

        public StarPlatinum(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Star Platinum";

            // collision & sprite settings
            _pickupSprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("headbandPickup"), 32, 32, false);
            sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("headband"), 32, 32, false);
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
            if(equippedDuck != null)
            {
                if (firstTime)
                {
                    active = true;
                }
            }
            else if (equippedDuck == null)
            {
                firstTime = true;
            }
            if (active)
            {
                SFX.Play(GetPath("sounds/StarPlatinum"));
                active = false;
                firstTime = false;
            }
            if (equippedDuck != null
                && equippedDuck.inputProfile.Pressed(Triggers.Quack)
                && _cooldown == 0
                && !duck.immobilized
                && !duck.inNet
                && !duck.sliding)
            {
                if (isServerForObject)
                    Level.Add(new ATOra(x + offDir * 18f, y + 8f, offDir, equippedDuck));
                _cooldown = 10;
                SFX.Play(GetPath("sounds/Oraa2"));
            }
            if (_cooldown > 0)
            {
                _cooldown--;
            }

        }
    }
}