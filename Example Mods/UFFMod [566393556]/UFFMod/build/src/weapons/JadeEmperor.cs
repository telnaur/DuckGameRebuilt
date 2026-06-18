namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|tech")]
    public class JadeEmperor : Gun
    {
        public StateBinding _flareAlphaStateBinding = new StateBinding("_flareAlpha");
        public StateBinding netSFX_fireStateBinding = new NetSoundBinding("netSFX_fire");

        public NetSoundEffect netSFX_fire = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\bzhew")
        });

        private SpriteMap sprite;

        public JadeEmperor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // editor name
            _editorName = "Jade Emperor";

            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\jadeEmperor"), 22, 13);
            graphic = sprite;
            _center = new Vec2(11f, 7f);
            _collisionSize = new Vec2(22f, 13f);
            _collisionOffset = new Vec2(-11f, -7f);
            _holdOffset = new Vec2(-1f, -2f);
            _barrelOffsetTL = new Vec2(21f, 8f);

            // weapon settings
            ammo = 8;
            _kickForce = 1.5f;
            _flare = new SpriteMap("laserFlare", 16, 16);
            _flare.center = new Vec2(0f, 8f);
        }

        public override void OnPressAction()
        {
            if (_wait == 0f)
            {
                if (ammo > 0)
                {
                    ammo--;
                    ApplyKick();

                    if (isServerForObject)
                    {
                        netSFX_fire.Play();

                        JadeShot jadeShot = new JadeShot(Offset(barrelOffset).x + barrelVector.x * 8f, Offset(barrelOffset).y + barrelVector.y * 8f, 4f, offDir >= 0 ? angleDegrees : angleDegrees + 180f);
                        jadeShot.owner = owner ?? null;
                        Level.Add(jadeShot);

                        _flareAlpha = 1.5f;
                    }
                }
                else
                    DoAmmoClick();
                _wait = 5f;
            }
        }
    }
}
