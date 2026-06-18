
namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Magic")]
    public class ZawardoHat : Hat, IQuackOverrideEquipment
    {
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");
        public StateBinding _zawarudoWaitTimeBinding = new StateBinding("_zawarudoWaitTime");
        public StateBinding _colorTransTimeBinding = new StateBinding("_colorTransTime");
        public bool active = false;
        public bool firstTime = true;
        public bool zawaruded = false;
        public bool canquack = true;

        public int _cooldown;
        public int _colorTransTime;
        public int _zawarudoWaitTime;

        public int cooldown
        {
            get
            {
                return _cooldown;
            }
        }
        public int zawarudoWaitTime
        {
            get
            {
                return _zawarudoWaitTime;
            }
        }
        public int colorTransTime
        {
            get
            {
                return _colorTransTime;
            }
        }


        public ZawardoHat(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Zawarudo";

            // collision & sprite settings
            _pickupSprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("zawarudoPickup"), 32, 32, false);
            sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("zawarudo"), 32, 32, false);
            graphic = sprite;
            center = new Vec2(15f, 19f);
            _collisionSize = new Vec2(10f, 6f);
            _collisionOffset = new Vec2(-5f, -3f);

            // equipment settings
            _equippedThickness = 0.1f;
            _zawarudoWaitTime = 120;
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
                SFX.Play(GetPath("sounds/Wryy"));
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
                zawaruded = true;
                if(canquack == true)
                {
                    SFX.Play(GetPath("sounds/Zaawarudo"));
                    canquack = false;
                    Layer.Blocks.colorMul = new Vec3(0.8f, 0f, 1f);
                }
            }
            if (_zawarudoWaitTime == 0)
            {
                if (isServerForObject)
                {
                    Level.Add(new ATZawarudo(x + 170f, y + 8f, equippedDuck, true, false, false, false));
                    Level.Add(new ATZawarudo(x - 170f, y + 8f, equippedDuck, false, true, false, false));
                    Level.Add(new ATZawarudo(x, y + 170f, equippedDuck, false, false, true, false));
                    Level.Add(new ATZawarudo(x, y - 170f, equippedDuck, false, false, false, true));
                    Level.Add(new ATZawarudo(x + 170f, y + 170f, equippedDuck, true, false, true, false));
                    Level.Add(new ATZawarudo(x - 170f, y + 170f, equippedDuck, false, true, true, false));
                    Level.Add(new ATZawarudo(x + 170f, y - 170f, equippedDuck, true, false, false, true));
                    Level.Add(new ATZawarudo(x - 170f, y - 170f, equippedDuck, false, true, false, true));
                    Layer.Blocks.colorMul = new Vec3(1f, 1f, 0f);
                    _cooldown = 1000;
                    _zawarudoWaitTime = 120;
                    _colorTransTime = 150;
                    zawaruded = false;
                    canquack = true;
                }
            }
            if (zawaruded == true)
            {
                if (_colorTransTime > 0)
                {
                    _colorTransTime--;
                }
                if (_zawarudoWaitTime > 0)
                {
                    _zawarudoWaitTime--;
                }
            }

            if (_cooldown > 0)
            {
                _cooldown--;
            }

        }
    }
}