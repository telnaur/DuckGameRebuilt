
namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Magic")]
    public class Sans : Hat, IQuackOverrideEquipment
    {
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");
        public bool active = false;
        public bool firstTime = true;
        public int megalovania_counter = 0;

        public int _cooldown;

        public int cooldown
        {
            get
            {
                return _cooldown;
            }
        }

        public Sans(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Sans?";

            // collision & sprite settings
            _pickupSprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("sansPickup"), 32, 32, false);
            sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("sansActive"), 32, 32, false);
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
                SFX.Play(GetPath("sounds/Sans noise"));
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
                {
                    Vec2 beamPos = new Vec2(x + offDir * 50f, y - 1800);
                    Vec2 beamTarget = new Vec2(0f, 3600f);
                    if (Network.isActive)
                        Send.Message(new NMDeathBeam(null, beamPos, beamTarget));
                    Level.Add(new DeathBeam(beamPos, beamTarget));
                }
                _cooldown = 62;
             
                SFX.Play(GetPath("SFX/drillklang"));
                if (megalovania_counter == 0)
                {
                    megalovania_counter++;
                    SFX.Play(GetPath("sounds/megalovania_1s"));
                }
                else
                {
                    megalovania_counter = 0;
                    SFX.Play(GetPath("sounds/megalovania_2s"));
                }
            }
            if (_cooldown > 0)
            {
                _cooldown--;
            }

        }
    }
}