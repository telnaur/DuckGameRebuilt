using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | FireAndWater")]
    class DivingHat : Hat
    {
        private SpriteMap sprite;

        public DivingHat(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "DivingHat";
            this._pickupSprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("divingHatPickup"), 16, 16);
            this._sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("divingHat"), 32, 32, false);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-7f, -7f);
            this.collisionSize = new Vec2(14f, 14f);
            this._sprite.CenterOrigin();
            this._equippedThickness = 5f;
            this._weight = 7.25f;
            this.physicsMaterial = PhysicsMaterial.Metal;
        }
        public override void Update()
        {
            if (base.equippedDuck != null && base.equippedDuck.grounded)
            {
                if (!base.equippedDuck.sliding && !base.equippedDuck.immobilized)
                {
                    if (base.equippedDuck.inputProfile.Down("RIGHT") && base.equippedDuck.hSpeed < 2f)
                    {
                        base.equippedDuck.hSpeed = MathHelper.Lerp(base.equippedDuck.hSpeed, 2f, 0.12f);
                    }
                    if (base.equippedDuck.inputProfile.Down("LEFT") && base.equippedDuck.hSpeed > -2f)
                    {
                        base.equippedDuck.hSpeed = MathHelper.Lerp(base.equippedDuck.hSpeed, -2f, 0.12f);
                    }
                }
            }
            base.Update();
        }

    }
}
