using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace DuckGame.HaloWeapons
{
    public static class Utilities
    {
        public static void SetCollisionBox(Thing thing, Vec2 collisionSize)
        {
            thing.collisionSize = collisionSize;
            thing.center = collisionSize / 2f;
            thing.collisionOffset = -thing.center;
        }

        public static void SetCollisionBox(Thing thing, float width, float height)
        {
            SetCollisionBox(thing, new Vec2(width, height));
        }
    }
}
