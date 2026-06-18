using System;
using System.Collections.Generic;
using System.Reflection;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|blocks")]
    public class AmmoBox : BaseBox
    {
        public AmmoBox(float xpos, float ypos)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\blocks\\ammoBox"), 16, 16);
            graphic = sprite;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            depth = 0.5f;
            timesUsed = 0;
            _canFlip = false;
            _netHitSound = new NetSoundEffect(new string[1]
            {
              Mod.GetPath<UffMod>("SFX\\reload")
            })
            {
                volume = 1f
            };
        }

        public override void Activate(MaterialThing with)
        {
            if (isServerForObject)
            {
                Duck duck = with as Duck;
                Gun gun = with as Gun;
                if (duck != null && duck.gun != null && !duck.gun.infinite)
                {
                    Gun g = Activator.CreateInstance(duck.gun.GetType(), Editor.GetConstructorParameters(duck.gun.GetType())) as Gun;
                    g.position = duck.gun.position;
                    g.offDir = duck.gun.offDir;
                    g.angle = duck.gun.angle;
                    duck.gun.position = new Vec2(-99999f, -99999f);
                    Level.Add(new AmmoBoxReload(x, y - 8f));
                    Level.Add(g);
                    Level.Remove(duck.gun);
                    duck.GiveHoldable(g);
                    base.Activate(with);
                }
                else if (gun != null && !gun.infinite)
                {
                    Gun g = Activator.CreateInstance(gun.GetType(), Editor.GetConstructorParameters(gun.GetType())) as Gun;
                    g.position = gun.position;
                    g.offDir = gun.offDir;
                    g.angle = gun.angle;
                    gun.position = new Vec2(-99999f, -99999f);
                    Level.Add(new AmmoBoxReload(x, y - 8f));
                    Level.Add(g);
                    Level.Remove(gun);
                    base.Activate(with);
                }
            }
        }
    }
}
