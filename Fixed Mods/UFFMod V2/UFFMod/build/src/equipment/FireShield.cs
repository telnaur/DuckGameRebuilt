using System;
using System.Linq;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|barrier")]
    public class FireShield : Barrier
    {
        public FireShield(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Fire Barrier";
            
            // collision & sprite settings
            pickupSprite = new Sprite(Mod.GetPath<UffMod>("equipment\\fireShieldPickup"));
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\fireShield"), 26, 26);
            sprite.AddAnimation("flame", 0.5f, true, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            sprite.SetAnimation("flame");
            graphic = pickupSprite;
            center = new Vec2(6f, 4f);
            _holdOffset = new Vec2(1f, 1f);
            wearCenter = new Vec2(13f, 13f);
            collisionOffset = new Vec2(-3f, -3f);
            collisionSize = new Vec2(6f, 6f);
            sprite.CenterOrigin();

            // equipment settings
            _equippedThickness = 0f;
            flammable = 0f;
        }

        public override void Update()
        {
            if(equippedDuck != null)
            {
                foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(position, 13f))
                {
                    if (materialThing is RagdollPart && ((RagdollPart)materialThing)._doll._duck == equippedDuck)
                    {
                        materialThing.flammable = 0f;
                        materialThing.onFire = false;
                    }
                    if (materialThing.isServerForObject && materialThing.active && materialThing.visible && !(materialThing is FluidPuddle) && !(materialThing is RagdollPart && ((RagdollPart)materialThing)._doll._duck == equippedDuck) && !(materialThing is TrappedDuck && ((TrappedDuck)materialThing).captureDuck == equippedDuck) && materialThing != equippedDuck && !equippedDuck._equipment.Contains(materialThing))
                    {
                        if (materialThing.heat > 0.5f && !materialThing.onFire && materialThing.flammable > 0f)
                            materialThing.Burn(materialThing.position, owner);
                        materialThing.DoHeatUp(0.05f);
                    }
                }

                if(equippedDuck._equipment.Count > 0)
                    foreach (Equipment e in equippedDuck._equipment)
                        if (e.heat > 0f)
                            e.heat = 0f;

                if(equippedDuck.holdObject != null && equippedDuck.holdObject.heat > 0f)
                    equippedDuck.holdObject.heat = 0f;

                equippedDuck.flammable = 0f;
                equippedDuck.onFire = false;

                if (isServerForObject)
                {
                    bool extinguish = false;
                    foreach (FluidPuddle fluidPuddle in Level.CheckPointAll<FluidPuddle>(position))
                        if (fluidPuddle.data.flammable <= 0.5f && fluidPuddle.data.heat <= 0.5f)
                            extinguish = true;

                    if (Level.CheckCircleAll<ExtinguisherSmoke>(position, 13f).Count() > 4)
                        extinguish = true;

                    if (extinguish)
                    {
                        equippedDuck.Unequip(this);
                        Level.Remove(this);

                        for (float f = Rando.Float(30f); f < 360f; f += 60)
                        {
                            GlobalSteam iceS = new GlobalSteam(x, y, Rando.Float(24f, 32f));
                            Level.Add(iceS);
                            iceS.xscale = iceS.yscale = Rando.Float(0.4f, 0.6f);
                            float f2 = Rando.Float(0.4f, 1.2f);
                            iceS.hSpeed = f2 * (float)Math.Cos(Maths.DegToRad(f));
                            iceS.vSpeed = f2 * (float)Math.Sin(Maths.DegToRad(f));
                        }
                    }
                }
            }

            base.Update();
        }

        public override void UnEquip()
        {
            foreach (RagdollPart ragdollPart in Level.CheckCircleAll<RagdollPart>(position, 13f))
                if (ragdollPart._doll._duck == equippedDuck)
                    ragdollPart.flammable = 1f;
            equippedDuck.flammable = 1f;

            base.UnEquip();
        }
    }
}
