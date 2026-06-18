/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Equipment")]
    public class plumberHat : Helmet
    {
        public bool sticking = false;

        public plumberHat(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite("knightHelmetPickup", 0.0f, 0.0f);
            this._sprite = new SpriteMap("knightHelmet", 32, 32, false);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(11f, 12f);
            this._equippedCollisionOffset = new Vec2(-4f, -2f);
            this._equippedCollisionSize = new Vec2(11f, 12f);
            this._hasEquippedCollision = true;
            this._sprite.CenterOrigin();
            this.depth = (Depth)0.0001f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._isArmor = true;
            this._equippedThickness = 3f;
            this.editorTooltip = "Protects ye olde medieval skull from impacts.";
        }

        public override void Update()
        {
            base.Update();
            if (this._equippedDuck != null)
            {
                if (_equippedDuck.inputProfile.Down("JUMP") && sticking)
                {
                    return;
                    sticking = false;
                    _equippedDuck.immobilized = false;
                }
            }
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._equippedDuck != null && !this.destroyed)
            {
                if (from != ImpactedFrom.Top)
                    return;
                if (with is Block)
                {
                    _equippedDuck.hSpeed = -5f;
                    _equippedDuck.immobilized = true;
                    sticking = true;
                }
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._equippedDuck != null && !this.destroyed)
            {
                if (from != ImpactedFrom.Top)
                    return;
                if (with is Block)
                {
                    _equippedDuck.hSpeed = 5f;
                    _equippedDuck.immobilized = true;
                    sticking = true;
                }
                //    Block block = Level.CheckRay<Block>(position, position + this.barrelVector * this._ammoType.range, out hitPos);
                //this._hasRay = true;
                //this._rayHit = hitPos;
                //if (block != null && block.physicsMaterial == PhysicsMaterial.Metal)
                //{
                //holdable1.duck.immobilized = true;
                //          this.duck.moveLock = true;
            }
        }
    }
}
*/