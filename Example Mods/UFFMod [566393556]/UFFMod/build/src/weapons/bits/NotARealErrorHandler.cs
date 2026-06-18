using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.UFFMod
{
    internal class NotARealErrorHandler : Thing
    {
        public StateBinding _updateStateBinding = new StateBinding("_update");
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        public int _update;

        private Color bgColor;
        private List<Type> physicsObjects;
        private List<Type> autoBlocks;
        private List<Type> backgroundTiles;

        public NotARealErrorHandler(float xpos, float ypos)
            : base(xpos, ypos)
        {
            bgColor = Level.current.backgroundColor;
            Level.current.backgroundColor = Color.Black;
            _update = 0;
        }

        private List<System.Type> GetAutoBlocks(EditorGroup group)
        {
            return Enumerable.ToList<System.Type>(Enumerable.Where<System.Type>((IEnumerable<System.Type>)Editor.ThingTypes, (Func<System.Type, bool>)(t =>
            {
                if (t.IsAbstract || !t.IsSubclassOf(typeof(AutoBlock)) || t.GetCustomAttributes(typeof(EditorGroupAttribute), false).Length == 0)
                    return false;
                IReadOnlyPropertyBag bag = ContentProperties.GetBag(t);
                return bag.GetOrDefault<bool>("canSpawn", true) && (!Network.isActive || !bag.GetOrDefault<bool>("noRandomSpawningOnline", false)) && ((!Network.isActive || bag.GetOrDefault<bool>("isOnlineCapable", true)) && (Main.isDemo || !bag.GetOrDefault<bool>("onlySpawnInDemo", false)));
            })));
        }

        private List<System.Type> GetBackgroundTiles(EditorGroup group)
        {
            return Enumerable.ToList<System.Type>(Enumerable.Where<System.Type>((IEnumerable<System.Type>)Editor.ThingTypes, (Func<System.Type, bool>)(t =>
            {
                if (t.IsAbstract || !t.IsSubclassOf(typeof(BackgroundTile)) || t.GetCustomAttributes(typeof(EditorGroupAttribute), false).Length == 0)
                    return false;
                IReadOnlyPropertyBag bag = ContentProperties.GetBag(t);
                return bag.GetOrDefault<bool>("canSpawn", true) && (!Network.isActive || !bag.GetOrDefault<bool>("noRandomSpawningOnline", false)) && ((!Network.isActive || bag.GetOrDefault<bool>("isOnlineCapable", true)) && (Main.isDemo || !bag.GetOrDefault<bool>("onlySpawnInDemo", false)));
            })));
        }

        public static void Scrangle(Thing thing)
        {
            if (thing.owner == null)
                Thing.Fondle(thing, DuckNetwork.localConnection);
            thing.angle = Rando.Float(2f * (float)Math.PI);
        }

        private void ScrambleBlock(Block block)
        {
            Type typeOfObjectToAdd = autoBlocks[NetRand.Int(autoBlocks.Count - 1)];

            AutoBlock autoBlock = Activator.CreateInstance(typeOfObjectToAdd, Editor.GetConstructorParameters(typeOfObjectToAdd)) as AutoBlock;
            Fondle(autoBlock);
            autoBlock.position = block.position;
            autoBlock.frame = Rando.Int(15);
            Scrangle(autoBlock);

            Level.Remove(block);
            if (isServerForObject)
                Level.Add(autoBlock);
        }

        public override void Update()
        {
            Layer.Parallax.darken = 1.3f;

            if (_update == 28)
            {
                physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
                autoBlocks = GetAutoBlocks(Editor.Placeables);
                backgroundTiles = GetBackgroundTiles(Editor.Placeables);

                foreach (Thing thing in Level.current.things)
                {
                    if (thing is BlockGroup)
                    {
                        IList<Block> blocksToScramble = new List<Block>();
                        foreach (Block block in ((BlockGroup)thing).blocks)
                            if (block is AutoBlock)
                                blocksToScramble.Add(block);
                        ((BlockGroup)thing).Wreck();
                        foreach (Block block in blocksToScramble)
                            ScrambleBlock(block);
                    }
                    else if (thing is AutoBlock)
                        ScrambleBlock((AutoBlock)thing);
                    else if (thing is BackgroundTile)
                    {
                        Type typeOfObjectToAdd = backgroundTiles[NetRand.Int(backgroundTiles.Count - 1)];

                        BackgroundTile backgroundTile = Activator.CreateInstance(typeOfObjectToAdd, Editor.GetConstructorParameters(typeOfObjectToAdd)) as BackgroundTile;
                        Fondle(backgroundTile);
                        backgroundTile.position = thing.position;
                        backgroundTile.frame = Rando.Int(15);
                        Scrangle(backgroundTile);

                        Level.Remove(thing);
                        if (isServerForObject)
                            Level.Add(backgroundTile);
                    }
                    else if (thing is Duck || thing is Ragdoll || thing is RagdollPart || (thing is IPlatform && !(thing is PhysicsObject)) || thing is AutoPlatform)
                        Scrangle(thing);
                    else if (thing is PhysicsObject)
                    {
                        PhysicsObject item = thing as PhysicsObject;
                        if (item is Equipment && ((Equipment)item).equippedDuck != null)
                            continue;
                        else if (item is Holdable && ((Holdable)item).duck != null)
                            continue;
                        else
                        {
                            Type typeOfObjectToAdd = physicsObjects[NetRand.Int(physicsObjects.Count - 1)];

                            PhysicsObject physicsObject = Activator.CreateInstance(typeOfObjectToAdd, Editor.GetConstructorParameters(typeOfObjectToAdd)) as PhysicsObject;
                            Fondle(physicsObject);
                            physicsObject.position = item.position;

                            Level.Remove(item);
                            if (isServerForObject)
                                Level.Add(physicsObject);
                        }
                    }
                }
            }

            if (_update < 20 || (_update >= 24 && _update < 44) || (_update >= 48 && _update < 52) || _update >= 56)
            {
                Layer.Background.darken = 1.3f;
                Layer.Game.darken = 1.3f;
                Layer.Glow.darken = 1.3f;
            }
            else
            {
                Layer.Background.darken = 0f;
                Layer.Game.darken = 0f;
                Layer.Glow.darken = 0f;
            }


            if (_update < 60)
                _update++;
            else
            {
                Level.current.backgroundColor = bgColor;
                Level.Remove(this);
            }
        }
    }
}