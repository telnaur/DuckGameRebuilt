using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|hats")]
    public class ReaperHood : Hat
    {
        public StateBinding _reaperStateBinding = new StateBinding("_reaper");
        public StateBinding _reaperSpawnedStateBinding = new StateBinding("_reaperSpawned");

        public Reaper _reaper;
        public bool _reaperSpawned;

        public ReaperHood(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor name
            _editorName = "Reaper's Hood";

            // collision & sprite settings
            _pickupSprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\reaperHoodPickup"), 32, 32);
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\reaperHood"), 32, 32);
            graphic = sprite;
            center = new Vec2(14f, 22f);
            _collisionSize = new Vec2(16f, 14f);
            _collisionOffset = new Vec2(-8f, -7f);

            // equipment settings
            _equippedDepth = 11;
            _equippedThickness = 0.1f;
        }

        public override void Terminate()
        {
        }

        public override void UnEquip()
        {
            if (equippedDuck != null && equippedDuck.dead)
            {
                if (isServerForObject)
                {
                    _reaper._controlDuck = equippedDuck;
                    _reaper._waitForPlayer = false;
                    _reaper.position = position;
                    equippedDuck.Fondle(_reaper);
                }
                Level.Remove(this);
            }
            else
                base.UnEquip();
        }

        public override void Update()
        {
            if (_reaper == null && isServerForObject)
            {
                _reaper = new Reaper(x, y, true, true);
                Level.Add(_reaper);
            }

            base.Update();
        }
    }
}
