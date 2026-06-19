using System.Linq;

namespace DuckGame.UFFMod
{
    // [EditorGroup("uff|stuff|props")]
    [BaggedProperty("canSpawn", false)]
    public class IceCubeUFFEdition : Crate, IAmSlippery
    {
        public StateBinding _heatStateBinding = new StateBinding("heat");
        public StateBinding _meltingStateBinding = new StateBinding("_melting");
        public StateBinding _collisionSizeStateBinding = new CompressedVec2Binding("collisionSize");
        public StateBinding _collisionOffsetStateBinding = new CompressedVec2Binding("collisionOffset");
        public StateBinding _frameStateBinding = new StateBinding("spriteFrame");

        public float _melting;

        private SpriteMap sprite;

        public byte spriteFrame
        {
            get
            {
                if (sprite == null)
                    return (byte)0;
                return (byte)sprite._frame;
            }
            set
            {
                if (sprite == null)
                    return;
                sprite._frame = (int)value;
            }
        }

        public IceCubeUFFEdition(float xpos, float ypos)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\iceCube"), 16, 16);
            graphic = sprite;
            flammable = 0f;
            friction = 0.05f;
            _melting = 0f;
            physicsMaterial = PhysicsMaterial.Default;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            Level.Remove(this);
            // replace with melt SFX: SFX.Play("crateDestroy");
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            SFX.Play(Mod.GetPath<UffMod>("SFX\\iceblockHit"), 1f, Rando.Float(0f, 0.2f));
            return thickness > bullet.ammo.penetration;
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            // do nothing
        }

        public override void Update()
        {
            foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(topLeft - new Vec2(-1f, 2f), bottomRight - new Vec2(1f, 16f)))
            {
                if (physicsObject != this && physicsObject.grounded && !Level.CheckRectAll<PhysicsObject>(topLeft + new Vec2(1f, 1f), bottomRight - new Vec2(1f, 0f)).Contains(physicsObject))
                {
                    bool skipAdd = false;
                    foreach (Thing thing in Level.current.things)
                    {
                        SlipperyHandler slipperyHandler = thing as SlipperyHandler;
                        if (slipperyHandler != null && slipperyHandler._slipperyTarget == physicsObject)
                        {
                            slipperyHandler.hasUpdated = true;
                            skipAdd = true;
                        }
                    }
                    if (!skipAdd && isServerForObject)
                        Level.Add(new SlipperyHandler(physicsObject));
                }
            }

            if (heat >= 0.3f)
            {
                heat = MathHelper.Lerp(heat, 0f, 0.08f);
                if (_melting % 0.04 >= 0.03)
                {
                    FluidStream waterStream;
                    waterStream = new FluidStream(Rando.Float(x - 8f, x + 8f), y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)), 4f, new Vec2());
                    FluidData data = Fluid.Water;
                    data.amount = 0.01f;
                    waterStream.Feed(data);
                }
                _melting += 0.01f;
                if (_melting > 0.25f && _melting <= 0.5f && sprite.frame != 1)
                {
                    for (int i = 0; i < Rando.Int(5, 7); i++)
                    {
                        IceSteam iceS = new IceSteam(x + Rando.Float(-4f, 4f), y, Rando.Float(18f, 24f));
                        Level.Add(iceS);
                        iceS.xscale = iceS.yscale = Rando.Float(0.3f, 0.5f);
                        iceS.hSpeed = Rando.Float(-0.4f, 0.4f);
                        iceS.vSpeed = -Rando.Float(0.5f, 1f);
                    }
                    collisionSize = new Vec2(16f, 10f);
                    sprite.frame = 1;
                }
                if (_melting > 0.5f && _melting <= 0.75f && sprite.frame != 2)
                {
                    for (int i = 0; i < Rando.Int(4, 6); i++)
                    {
                        IceSteam iceS = new IceSteam(x + Rando.Float(-4f, 4f), y, Rando.Float(18f, 24f));
                        Level.Add(iceS);
                        iceS.xscale = iceS.yscale = Rando.Float(0.5f, 0.7f);
                        iceS.hSpeed = Rando.Float(-0.4f, 0.4f);
                        iceS.vSpeed = -Rando.Float(0.5f, 1f);
                    }
                    collisionSize = new Vec2(16f, 6f);
                    sprite.frame = 2;
                }
                if (_melting > 0.75f && _melting <= 1f && sprite.frame != 3)
                {
                    for (int i = 0; i < Rando.Int(3, 5); i++)
                    {
                        IceSteam iceS = new IceSteam(x + Rando.Float(-8f, 8f), y, Rando.Float(18f, 24f));
                        Level.Add(iceS);
                        iceS.xscale = iceS.yscale = Rando.Float(0.3f, 0.5f);
                        iceS.hSpeed = Rando.Float(-0.4f, 0.4f);
                        iceS.vSpeed = -Rando.Float(0.5f, 1f);
                    }
                    collisionSize = new Vec2(12f, 4f);
                    sprite.frame = 3;
                }
                collisionOffset = new Vec2(-8f, 8f - _collisionSize.y);
            }
            if (_melting >= 1f)
            {
                for (int i = 0; i < Rando.Int(2, 4); i++)
                {
                    IceSteam iceS = new IceSteam(x + Rando.Float(-8f, 8f), y, Rando.Float(18f, 24f));
                    Level.Add(iceS);
                    iceS.xscale = iceS.yscale = Rando.Float(0.3f, 0.5f);
                    iceS.hSpeed = Rando.Float(-0.4f, 0.4f);
                    iceS.vSpeed = -Rando.Float(0.5f, 1f);
                }
                Level.Remove(this);
            }
            base.Update();
        }
    }
}
