using System.Collections.Generic;
using System.Linq;
using System;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|boots")]
    public class FastGoingShoes : Boots
    {
        public FastGoingShoes(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Fast Going Shoes";

            _pickupSprite = new Sprite(Mod.GetPath<UffMod>("equipment\\fastGoingShoesPickup"), 0f, 0f);
            _sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\fastGoingShoes"), 32, 32, false);
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
                if (!equippedDuck.sliding && !equippedDuck.immobilized)
                {
                    if(equippedDuck.inputProfile.Down(Triggers.Right) && equippedDuck.hSpeed < 8f)
                        equippedDuck.hSpeed = MathHelper.Lerp(equippedDuck.hSpeed, 8f, 0.12f);
                    if (equippedDuck.inputProfile.Down(Triggers.Left) && equippedDuck.hSpeed > -8f)
                        equippedDuck.hSpeed = MathHelper.Lerp(equippedDuck.hSpeed, -8f, 0.12f);
                }
                if (Math.Abs(equippedDuck.hSpeed) > 3.1f)
                    Level.Add(SmallSmoke.New(x, y + 12f));
            }
            base.Update();
        }

    }
}
