using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    // [EditorGroup("uff|stuff|props")]
    [BaggedProperty("canSpawn", false)]
    public class Chandelier : PhysicsObject, IPlatform
    {
        public StateBinding _lockedPositionStateBinding = new CompressedVec2Binding("_lockedPosition");
        public StateBinding _ropeMadeStateBinding = new StateBinding("_ropeMade");

        public Vec2 _lockedPosition;
        public bool _ropeMade;

        public EditorProperty<int> variant = new EditorProperty<int>(0, null, 0f, 2f, 1f);

        private SpriteMap sprite;

        public Chandelier(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // general settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\chandelier"), 48, 24);
            graphic = sprite;
            center = new Vec2(24f, 12f);
            collisionOffset = new Vec2(-24f, -3f);
            collisionSize = new Vec2(48f, 15f);
            depth = 0.5f;
            thickness = 10f;
            weight = 8f;
            gravMultiplier = 0f;
            physicsMaterial = variant > 0 ? PhysicsMaterial.Metal : PhysicsMaterial.Wood;
        }

        public override void Initialize()
        {
            sprite.frame = variant;

            base.Initialize();

            _lockedPosition = position;
        }

        public override void Update()
        {
            if (!_ropeMade && isServerForObject)
            {
                Vec2 hitPos;
                Level.CheckRay<Block>(position - new Vec2(0, 8f), position - new Vec2(0f, 520f), out hitPos);
                float topY = hitPos.y;
                List<ChandelierRope> nodes = new List<ChandelierRope>();
                //nodes = AddRope(nodes, topY, this);
                int i = 0;
                for (float f = topY + 8f; f <= y - 8f; f += 8f)
                {
                    nodes = AddRope(nodes, f);
                    i++;
                }
                _ropeMade = true;
            }

            position = _lockedPosition;
            hSpeed = 0f;
            vSpeed = 0f;

            base.Update();

            gravMultiplier = 0f;
            _grounded = true;
        }

        private List<ChandelierRope> AddRope(List<ChandelierRope> nodes, float nodeY)
        {
            /*ChandelierRope chandelierRope = new ChandelierRope(x, nodeY, variant);
            nodes.Add(chandelierRope);
            Level.Add(chandelierRope);*/
            return nodes;
        }

        public override void Draw()
        {
            sprite.frame = variant;

            base.Draw();
        }
    }

    public class ChandelierRope : PhysicsObject
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _lockedPositionStateBinding = new CompressedVec2Binding("_lockedPosition");
        public StateBinding _parentStateBinding = new StateBinding("_parent");
        public StateBinding _variantStateBinding = new StateBinding("_variant");

        public Vec2 _lockedPosition;
        public Thing _parent;
        public int _variant;

        private SpriteMap sprite;

        public ChandelierRope(float xpos, float ypos, int variant, Thing parent, bool last)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\chandelierRope"), 4, 8);
            sprite.frame = variant;
            sprite.CenterOrigin();
            graphic = sprite;
            center = new Vec2(2f, 4f);
            _parent = parent;
            _variant = variant;
            depth = 0.5f;
            thickness = 10f;
        }

        public override void Initialize()
        {
            base.Initialize();

            _lockedPosition = position;
        }

        public override void Update()
        {
            position = _lockedPosition;
            hSpeed = 0f;
            vSpeed = 0f;

            base.Update();

            gravMultiplier = 0f;
        }

        public override void Terminate()
        {
            if (!(_parent is Chandelier))
                Level.Remove(_parent);
        }

        public override bool Destroy(DestroyType type = null)
        {
            Level.Remove(this);
            if (!(_parent is Chandelier))
                Level.Remove(_parent);
            return true;
        }

        public override void Draw()
        {
            sprite.frame = _variant;

            base.Draw();
        }
    }
}
