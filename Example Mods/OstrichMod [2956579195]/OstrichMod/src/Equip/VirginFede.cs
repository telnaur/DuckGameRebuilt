
namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Virgo")]
    public class VirginFede : Hat, IQuackOverrideEquipment
    {
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");
        public bool active = false;

        public int _cooldown;

        public int cooldown
        {
            get
            {
                return _cooldown;
            }
        }

        public int _invisibilityTimer = 600;


        public VirginFede(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Virgin Fede";

            // collision & sprite settings
            _pickupSprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("fedePickup"), 32, 32, false);
            sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("fedeActive"), 32, 32, false);
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
            if (equippedDuck == null)
            {
                active = false;
                _invisibilityTimer = 600;
                this.alpha = 1f;
            }
            if (equippedDuck != null
                && equippedDuck.inputProfile.Pressed(Triggers.Quack)
                && _cooldown == 0
                && !duck.immobilized
                && !duck.inNet
                && !duck.sliding)
            {
                if (active)
                {
                    active = false;
                    _invisibilityTimer = 600;
                    if (isServerForObject)
                        duck.alpha = 1f;
                }
                else
                {
                    if (isServerForObject)
                        duck.alpha = 0f;
                    active = true;
                    _cooldown = 800;
                }
                SFX.Play(GetPath("sounds/Oraa2"));
            }
            if (active)
            {
                _invisibilityTimer--;
            }
            if (_invisibilityTimer <= 0)
            {
                active = false;
                duck.alpha = 1f;
                _invisibilityTimer = 600;
            }
            if (_cooldown > 0)
            {
                _cooldown--;
            }

        }
    }
}