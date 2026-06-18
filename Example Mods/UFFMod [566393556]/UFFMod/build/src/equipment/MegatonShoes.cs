using System;
using System.Linq;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|boots")]
    public class MegatonShoes : Boots, IForcedMovementImmunity
    {
        public StateBinding netSFX_groundPoundStateBinding = (StateBinding)new NetSoundBinding("netSFX_groundPound");

        public NetSoundEffect netSFX_groundPound = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\groundPound")
        })
        {
            volume = 1f
        };

        private bool groundPounding;

        public MegatonShoes(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Megaton Shoes";

            _pickupSprite = new Sprite(Mod.GetPath<UffMod>("equipment\\megatonShoesPickup"), 0f, 0f);
            _sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\megatonShoes"), 32, 32, false);
            graphic = _pickupSprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -6f);
            collisionSize = new Vec2(12f, 13f);
            _equippedCollisionOffset = new Vec2(-6f, 8f);
            _equippedCollisionSize = new Vec2(12f, 7f);
            _hasEquippedCollision = true;
            _isArmor = true;
            _equippedThickness = 3f;
            _equippedDepth = 1;
            weight = 8f;
        }

        public override void Update()
        {
            if (equippedDuck != null)
            {
                if (!equippedDuck.grounded)
                {
                    if (equippedDuck.inputProfile.Down(Triggers.Down) && !equippedDuck.sliding && !equippedDuck.immobilized && !groundPounding)
                    {
                        if (isServerForObject)
                            Level.Add(new StunHandler(duck, 1, true));
                        equippedDuck.hSpeed = 0f;
                        equippedDuck.vSpeed = 0.8f;
                        groundPounding = true;
                    }
                    if(groundPounding)
                    {
                        if (isServerForObject)
                            Level.Add(new StunHandler(duck, 1, true));
                        equippedDuck.hSpeed = 0f;
                        if (equippedDuck.vSpeed > 0f)
                            equippedDuck.vSpeed += 0.8f;
                        else
                            equippedDuck.vSpeed = 0.8f;
                    }
                }
                else
                {
                    if (groundPounding)
                    {
                        groundPounding = false;
                        if (isServerForObject)
                            netSFX_groundPound.Play();
                    }
                }
            }
            base.Update();
        }

        public override void Equip(Duck d)
        {
            d.runMax /= 1.75f;
            base.Equip(d);
        }

        public override void UnEquip()
        {
            equippedDuck.runMax *= 1.75f;
            base.UnEquip();
        }
    }
}
