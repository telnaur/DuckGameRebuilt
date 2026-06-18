using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Curses")]
    [BaggedProperty("isFatal", false)]

    public class plumbus : Gun
    {

        private int used = 0;

        public plumbus(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.ammo = 20;
            this.graphic = new Sprite(GetPath("plumbus"), 0.0f, 0.0f);
            this.center = new Vec2(6f, 8f);
            this.collisionOffset = new Vec2(-6f, -8f);
            this.collisionSize = new Vec2(11f, 15f);
            this._holdOffset = new Vec2(0f, 0f);
            this.weight = 1f;
            this.flammable = 0.3f;
            this._editorName = "Plumbus";
            this.editorTooltip = "Everyone knows what it does. Just click a bunch of times.";
        }
        public override void OnPressAction()
        {
            if (this.owner == null || this.owner is MagnetGun)
                return;
            else
            { 
                used++;
                SFX.Play("smallSplat", 0.9f, Rando.Float(-0.4f, 0.4f), 0.0f, false);
                FluidData water = Fluid.Water;
                water.amount = Rando.Float(0.0001f, 0.0005f);
                int num = Rando.Int(4) + 2;
                for (int index = 0; index < num; ++index)
                {
                    Fluid fluid = new Fluid(this.x + (float)this.duck.offDir * (2f + Rando.Float(0.0f, 4f)), this.y, new Vec2((float)this.duck.offDir * Rando.Float(0.5f, 3f), Rando.Float(0.0f, -2f)), water, (Fluid)null, 5f);
                    fluid.depth = this.depth + 1;
                    Level.Add((Thing)fluid);
                }
                this._holdOffset = new Vec2(1f, 0f);
                if (used >= Rando.Int(6, 10))
                {
                    int cursedItem = Rando.Int(0, 1);
                    Vec2 vec2 = this.Offset(new Vec2(0f, 0f));
                    switch (cursedItem)
                    {
                        case 0:
                            {
                                if (this.isServerForObject)
                                {
                                    curseShotgun cursedReward = new curseShotgun(vec2.x, vec2.y);
                                    cursedReward.hSpeed = Rando.Float(-2f, 2f);
                                    cursedReward.vSpeed = -3.5f + Rando.Float(0f, -0.2f);
                                    Level.Add((Thing)cursedReward);
                                }
                                break;
                            }
                        case 1:
                            {
                                if (this.isServerForObject)
                                {
                                    curseBody cursedReward = new curseBody(vec2.x, vec2.y);
                                    cursedReward.hSpeed = Rando.Float(-2f, 2f);
                                    cursedReward.vSpeed = -3.5f + Rando.Float(0f, -0.2f);
                                    Level.Add((Thing)cursedReward);
                                }
                                break;
                            }
                        default:
                            {
                                if (this.isServerForObject)
                                {
                                    curseBanana cursedReward = new curseBanana(vec2.x, vec2.y);
                                    cursedReward.hSpeed = Rando.Float(-2f, 2f);
                                    cursedReward.vSpeed = -3.5f + Rando.Float(0f, -0.2f);
                                    Level.Add((Thing)cursedReward);
                                }
                                break;
                            }
                    }
                    SFX.Play("respawn", 1f, 0.0f, 0.0f, false);
                    Level.Remove((Thing)this);
                }
            }
        }
        public override void OnReleaseAction()
        {
            base.OnReleaseAction();
            this._holdOffset = new Vec2(0f, 0f);
        }
    }
}
