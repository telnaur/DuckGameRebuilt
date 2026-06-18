using System.Collections.Generic;
using System.Linq;
using System;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | FireAndWater")]
    public class IceBoots : Boots
    {
        public IceBoots(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Ice Boots";

            _pickupSprite = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("IceBoots_pickup"), 0.0f, 0.0f);
            _sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("IceBoots"), 32, 32, false);
            graphic = _pickupSprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -6f);
            collisionSize = new Vec2(12f, 13f);
            _equippedDepth = 1;
        }

        public override void Update()
        {
            if (equippedDuck != null && equippedDuck.grounded)
            {
                if (equippedDuck.sliding && !equippedDuck.immobilized)
                {
                    if(equippedDuck.inputProfile.Down(Triggers.Right) && equippedDuck.hSpeed < 4f)
                        equippedDuck.hSpeed = MathHelper.Lerp(equippedDuck.hSpeed, 4f, 0.12f);
                    if (equippedDuck.inputProfile.Down(Triggers.Left) && equippedDuck.hSpeed > -8f)
                        equippedDuck.hSpeed = MathHelper.Lerp(equippedDuck.hSpeed, -4f, 0.12f);
                }
                if (!equippedDuck.sliding)
                {
                    equippedDuck.frictionMult = 0.4f;
                }
            }
            base.Update();
        }

    }
}
