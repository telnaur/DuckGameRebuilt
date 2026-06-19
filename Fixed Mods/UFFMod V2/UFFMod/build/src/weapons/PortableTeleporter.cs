namespace DuckGame.UFFMod
{
    // [EditorGroup("uff|weapons|tech")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isSuperWeapon", true)]
    public class PortableTeleporter : Gun
    {
        public StateBinding _reticleStateBinding = new StateBinding("_reticle");

        public TeleportReticle _reticle;

        private bool aiming;

        private SpriteMap sprite;

        public PortableTeleporter(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Portable Teleporter";

            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\railgun"), 23, 11);
            graphic = sprite;
            _center = new Vec2(11f, 6f);
            _collisionSize = new Vec2(20f, 8f);
            _collisionOffset = new Vec2(-10f, -4f);
            _holdOffset = new Vec2(-4f, -1f);
            _barrelOffsetTL = new Vec2(21f, 5f);
            _laserOffsetTL = new Vec2(22f, 5f);

            // weapon settings
            ammo = 1;
        }

        public override void Terminate()
        {
            if (_reticle != null)
                Level.Remove(_reticle);

            base.Terminate();
        }

        public override void OnPressAction()
        {
            if (ammo > 0 && duck != null)
            {
                if (_reticle != null)
                {
                    _reticle.position = position;
                    _reticle.visible = true;
                }
                else if (isServerForObject)
                {
                    _reticle = new TeleportReticle(x, y);
                    Level.Add(_reticle);
                }
                aiming = true;
                duck.immobilized = true;
                duck.remoteControl = true;
            }
            else
                SFX.Play("click");
        }

        public override void OnReleaseAction()
        {
            Duck d = duck ?? prevOwner as Duck;
            if (d != null)
            {
                if (_reticle != null)
                    _reticle.visible = false;
                d.immobilized = false;
                d.remoteControl = false;
                if (_wait == 0f && aiming)
                {
                    d.position = _reticle.position;
                    d.vSpeed = -0.5f;
                    ammo--;
                    if (ammo == 0)
                        Level.Remove(this);
                }
            }
            aiming = false;
        }

        public override void Update()
        {
            if (_wait == 0f && aiming)
            {
                if (duck != null)
                {
                    if (_reticle != null)
                    {
                        if (duck.inputProfile.Down(Triggers.Left))
                            _reticle.x -= 2f;
                        if (duck.inputProfile.Down(Triggers.Right))
                            _reticle.x += 2f;
                        if (duck.inputProfile.Down(Triggers.Up))
                            _reticle.y -= 2f;
                        if (duck.inputProfile.Down(Triggers.Down))
                            _reticle.y += 2f;
                    }
                    if (duck.inputProfile.Released(Triggers.Grab))
                    {
                        duck.immobilized = false;
                        duck.remoteControl = false;
                        aiming = false;
                    }
                }
                else
                    aiming = false;
            }
            else if(_reticle != null)
                _reticle.visible = false;

            base.Update();
        }
    }

    public class TeleportReticle : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _visibleStateBinding = new StateBinding("visible");

        private SpriteMap sprite;

        public TeleportReticle(float xpos, float ypos)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\impendingDoom"), 25, 27);
            sprite.AddAnimation("quack", 0.25f, true, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            sprite.SetAnimation("quack");
            graphic = sprite;
            center = new Vec2(13f, 14f);
            depth = 1f;
        }
    }
}
