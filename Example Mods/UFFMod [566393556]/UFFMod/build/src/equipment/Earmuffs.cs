using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|hats")]
    public class Earmuffs : Hat
    {
        public Earmuffs(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // collision & sprite settings
            _pickupSprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\earmuffs"), 32, 32);
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\earmuffs"), 32, 32);
            graphic = sprite;
            center = new Vec2(16f, 16f);
            _collisionSize = new Vec2(10f, 13f);
            _collisionOffset = new Vec2(-5f, -6f);

            // equipment settings
            _equippedThickness = 0.1f;
        }
    }
}
