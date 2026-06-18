using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Equipment")]
    public class rollerBoots : Boots
    {
        private bool _static = false;
        private bool slidingBonus = false;
        private float walkingSpeed = 2f;
        private float slidingSpeed = 2.5f;

        public rollerBoots(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite(GetPath("rollerBootsPickup"), 0.0f, 0.0f);
            this._sprite = new SpriteMap(GetPath("rollerBoots"), 32, 32, false);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -7f);
            this.collisionSize = new Vec2(12f, 14f);
            this._equippedDepth = 1;
            this._editorName = "Rollerblade Boots";
            this.editorTooltip = "Stylish, make you run faster and let you slide along the map at incredible speed.";
        }

        public override void Update()
        {
            if (this._equippedDuck != null && !this.destroyed)
            {
                _equippedDuck.specialFrictionMod = 0.4f;
                _equippedDuck.modFric = true;
                if (this.owner.hSpeed < 1f && this.owner.hSpeed > -1f)
                {
                    this._static = true;
                    slidingBonus = false;
                }
                else if (_static == true)
                {
                    if (this.owner.hSpeed > 1f)
                    {
                        this.owner.hSpeed *= 1.2f;
                        /*if (this._equippedDuck.sliding)
                        {
                            this.owner.hSpeed += slidingSpeed;
                        }*/
                        this._static = false;
                    }
                    else if (this.owner.hSpeed < -1f)
                    {
                        this.owner.hSpeed *= 1.2f;
                        /*if (this._equippedDuck.sliding)
                        {
                            this.owner.hSpeed -= slidingSpeed;
                        }*/
                        this._static = false;
                    }
                }
                if (slidingBonus == false && this._equippedDuck.sliding)
                {
                    if (this.owner.hSpeed > 1f)
                    {
                        this.owner.hSpeed += slidingSpeed;
                    }
                    else if (this.owner.hSpeed < -1f)
                    {
                        this.owner.hSpeed -= slidingSpeed;
                    }
                    slidingBonus = true;
                }
            }
            base.Update();
        }
    }
}