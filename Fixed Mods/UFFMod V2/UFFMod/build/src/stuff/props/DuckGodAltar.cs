using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|props")]
    [BaggedProperty("canSpawn", false)]
    public class DuckGodAltar : PhysicsObject, IContainAThing, IPlatform
    {
        public StateBinding netSFX_sacrificeStateBinding = new NetSoundBinding("netSFX_sacrifice");
        public StateBinding _animationIndexStateBinding = new StateBinding("netAnimationIndex");
        public StateBinding _frameStateBinding = new StateBinding("spriteFrame");

        public NetSoundEffect netSFX_sacrifice = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\sacrifice")
        });

        private List<Type> physicsObjects;
        private List<Type> guns = new List<Type>();
        private List<Type> normalObjects = new List<Type>();
        private List<Type> superObjects = new List<Type>();
        private SpriteMap sprite;

        private byte netAnimationIndex
        {
            get
            {
                if (sprite == null)
                    return 0;
                return (byte)sprite.animationIndex;
            }
            set
            {
                if (sprite == null || sprite.animationIndex == value)
                    return;
                sprite.animationIndex = value;
            }
        }

        public byte spriteFrame
        {
            get
            {
                if (sprite == null)
                    return 0;
                return (byte)sprite._frame;
            }
            set
            {
                if (sprite == null)
                    return;
                sprite._frame = value;
            }
        }

        public Type contains { get; set; }

        public DuckGodAltar(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Altar";

            // general settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\duckGodAltar"), 30, 39);
            sprite.AddAnimation("grimey", 1f, true, 0);
            sprite.AddAnimation("bloody", 0.5f, false, 1, 2, 3, 4, 5, 6, 7, 8);
            sprite.SetAnimation("grimey");
            graphic = sprite;
            center = new Vec2(15f, 30f);
            collisionOffset = new Vec2(-14f, -9f);
            collisionSize = new Vec2(28f, 18f);
            depth = -0.5f;
            thickness = 10f;
            weight = 30f;
            flammable = 0f;
            physicsMaterial = PhysicsMaterial.Default;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (type != null && type.thing is HolyHandGrenade)
                return true;
            return false;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            Level.Add(MetalRebound.New(hitPos.x, hitPos.y, (double)bullet.travelDirNormalized.x > 0.0 ? 1 : -1));
            hitPos -= bullet.travelDirNormalized;
            for (int index = 0; index < 3; ++index)
                Level.Add(Spark.New(hitPos.x, hitPos.y, bullet.travelDirNormalized, 0.02f));
            return thickness > bullet.ammo.penetration;
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            // do nothing
        }

        public static List<System.Type> GetPhysicsObjects(EditorGroup group)
        {
            return Enumerable.ToList(Enumerable.Where(Editor.ThingTypes, (t =>
            {
                if (t.IsAbstract || !t.IsSubclassOf(typeof(PhysicsObject)) || t.GetCustomAttributes(typeof(EditorGroupAttribute), false).Length == 0)
                    return false;
                IReadOnlyPropertyBag bag = ContentProperties.GetBag(t);
                return bag.GetOrDefault<bool>("canSpawn", true) && (!Network.isActive || !bag.GetOrDefault<bool>("noRandomSpawningOnline", false)) && ((!Network.isActive || bag.GetOrDefault<bool>("isOnlineCapable", true)) && (Main.isDemo || !bag.GetOrDefault<bool>("onlySpawnInDemo", false)));
            })));
        }

        public override void Initialize()
        {
            physicsObjects = GetPhysicsObjects(Editor.Placeables);
            physicsObjects.RemoveAll(t => t == typeof(Present));
            foreach (Type o in physicsObjects)
            {
                if (o.IsSubclassOf(typeof(Gun)))
                    guns.Add(o);
                if (ContentProperties.GetBag(o).GetOrDefault("isSuperWeapon", false))
                    superObjects.Add(o);
                else
                    normalObjects.Add(o);
            }
        }

        public override void Update()
        {
            hSpeed = 0f; // remain stationary horizontally

            if (sprite.finished)
                sprite.SetAnimation("grimey");

            bool check = false;
            foreach (Ragdoll ragdoll in Level.CheckRectAll<Ragdoll>(this.topLeft - new Vec2(-2f, 11f), this.bottomRight - new Vec2(2f, 18f)))
            {
                if (isServerForObject)
                {
                    check = true;
                    if (ragdoll._duck.dead)
                    {
                        contains = normalObjects[Rando.Int(normalObjects.Count - 1)];
                        Holdable h = Activator.CreateInstance(contains, Editor.GetConstructorParameters(contains)) as Holdable;
                        Level.Add(h);
                        h.position = position - new Vec2(0f, 20f);
                    }
                    else if (!ragdoll._duck.dead)
                    {
                        ragdoll._duck.Kill(new DTImpale(ragdoll._duck));
                        contains = superObjects[Rando.Int(superObjects.Count - 1)];
                        Holdable h = Activator.CreateInstance(contains, Editor.GetConstructorParameters(contains)) as Holdable;
                        Level.Add(h);
                        h.position = position - new Vec2(0f, 20f);
                    }
                    Level.Add(new AltarDuckTaken(x, y, ragdoll._duck));
                    Level.Remove(ragdoll._duck);
                    Level.Remove(ragdoll);
                }
                foreach (AltarDuckTaken adc in Level.CheckRectAll<AltarDuckTaken>(new Vec2(x - 8f, y - 8f), new Vec2(x + 8f, y + 8f)))
                    if (adc._theDuck != null && ragdoll._duck == adc._theDuck)
                    {
                        check = false; 
                        if (!ragdoll._duck.dead)
                            ragdoll._duck.Kill(new DTImpale(ragdoll._duck));
                        Level.Remove(ragdoll._duck);
                        Level.Remove(ragdoll);
                    }
            }
            foreach (TrappedDuck tduck in Level.CheckRectAll<TrappedDuck>(this.topLeft - new Vec2(-2f, 11f), this.bottomRight - new Vec2(2f, 18f)))
            {
                if (isServerForObject)
                {
                    check = true;
                    contains = superObjects[Rando.Int(superObjects.Count - 1)];
                    Holdable h = Activator.CreateInstance(contains, Editor.GetConstructorParameters(contains)) as Holdable;
                    Level.Add(h);
                    h.position = position - new Vec2(0f, 20f);
                    tduck.captureDuck.Kill(new DTImpale(tduck.captureDuck));
                    Level.Add(new AltarDuckTaken(x, y, tduck.captureDuck));
                    Level.Remove(tduck);
                    Level.Remove(tduck.captureDuck);
                    Level.Remove(tduck.captureDuck.ragdoll);
                    Level.Remove(tduck.captureDuck._ragdollInstance);
                }
                foreach (AltarDuckTaken adc in Level.CheckRectAll<AltarDuckTaken>(new Vec2(x - 8f, y - 8f), new Vec2(x + 8f, y + 8f)))
                    if (adc._theDuck != null && tduck.captureDuck == adc._theDuck)
                    {
                        check = false;
                        Level.Remove(tduck);
                        Level.Remove(tduck.captureDuck);
                        Level.Remove(tduck.captureDuck.ragdoll);
                        Level.Remove(tduck.captureDuck._ragdollInstance);
                    }
            }
            foreach (Golduck golduck in Level.CheckRectAll<Golduck>(this.topLeft - new Vec2(-2f, 11f), this.bottomRight - new Vec2(2f, 18f)))
            {
                if (isServerForObject)
                {
                    check = true;
                    contains = guns[Rando.Int(superObjects.Count - 1)];
                    Gun g = Activator.CreateInstance(contains, Editor.GetConstructorParameters(contains)) as Gun;
                    Level.Add(g);
                    g.infinite = true;
                    g.position = position - new Vec2(0f, 20f);
                    Level.Add(new AltarDuckTaken(x, y, gd: golduck, gun: g));
                    Level.Remove(golduck);
                }
                foreach (AltarDuckTaken adc in Level.CheckRectAll<AltarDuckTaken>(new Vec2(x - 8f, y - 8f), new Vec2(x + 8f, y + 8f)))
                    if (adc._theGolduck != null && golduck == adc._theGolduck)
                    {
                        check = false;
                        Level.Remove(golduck);
                    }
            }
            if (check)
            {
                sprite.SetAnimation("bloody");
                netSFX_sacrifice.Play();
            }

            base.Update();
        }
    }
}