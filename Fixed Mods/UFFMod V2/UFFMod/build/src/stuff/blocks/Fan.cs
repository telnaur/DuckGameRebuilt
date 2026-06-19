using System.Collections.Generic;
using System.Linq;
using System;

namespace DuckGame.UFFMod
{
    public class Fan : Block, IDontMove, IPathNodeBlocker, IPlatform
    {
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _timerStateBinding = new StateBinding("_timer");
        public StateBinding _forceStateBinding = new StateBinding("_force");
        public StateBinding _decreasingStateBinding = new StateBinding("_decreasing");

        public float _timer;
        public float _force;
        public bool _decreasing;

        public EditorProperty<bool> continuous = new EditorProperty<bool>(true);
        public EditorProperty<float> duration = new EditorProperty<float>(2f, null, 0f, 10f, 0.1f);
        public EditorProperty<float> cooldown = new EditorProperty<float>(2f, null, 0f, 10f, 0.1f);
        public EditorProperty<float> skip = new EditorProperty<float>(0f, null, 0f, 20f, 0.1f);
        public EditorProperty<int> range = new EditorProperty<int>(8, null, 1f, 16f, 1f);

        protected SpriteMap sprite;

        private IList<PhysicsObject> blockers = new List<PhysicsObject>();
        private Vec2 rectTL;
        private Vec2 rectBR;
        private Vec2 speedMult;
        private float windRange;
        private int nextSpawn;

        public Fan(float xpos, float ypos)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\blocks\\yourNumberOneFan"), 16, 8);
            sprite.AddAnimation("brrr", 0.5f, true, 0, 1, 2, 3);
            sprite.SetAnimation("brrr");
            graphic = sprite;
            center = new Vec2(8f, 4f);
            depth = 0.5f;
            _force = 0f;
            _timer = 0f;
            nextSpawn = 0;
        }

        public override void Initialize()
        {
            _timer = skip % (duration + cooldown);
            windRange = range * 16f;

            base.Initialize();
        }

        private void SpawnParticle()
        {
            float spawnRandomness = Rando.Float(-8f, 8f);
            float endRandomness = Rando.Float(windRange, windRange + 32f);
            if (this is FanUp)
                Level.Add(new FanWind(x + spawnRandomness, y - 4f, new Vec2(x + spawnRandomness, y - endRandomness), blockers));
            else if (this is FanDown)
                Level.Add(new FanWind(x + spawnRandomness, y + 4f, new Vec2(x + spawnRandomness, y + endRandomness), blockers));
            else if (this is FanLeft)
                Level.Add(new FanWind(x - 4f, y + spawnRandomness, new Vec2(x - endRandomness, y + spawnRandomness), blockers));
            else if (this is FanRight)
                Level.Add(new FanWind(x + 4f, y + spawnRandomness, new Vec2(x + endRandomness, y + spawnRandomness), blockers));
            nextSpawn = Rando.Int(16, 21);
        }

        public override void Update()
        {
            if (!continuous)
                _timer = (_timer + (1 / 60f))
                    % (duration + cooldown);

            if (continuous || _timer <= duration)
            {
                sprite.speed = 1f;

                if (this is FanUp)
                {
                    rectTL = new Vec2(topLeft.x + 1f, topLeft.y - windRange);
                    rectBR = bottomRight - new Vec2(1f, 0f);
                    speedMult = new Vec2(0f, -1f);
                }
                else if (this is FanDown)
                {
                    rectTL = topLeft + new Vec2(1f, 0f);
                    rectBR = new Vec2(bottomRight.x - 1f, bottomRight.y + windRange);
                    speedMult = new Vec2(0f, 1f);
                }
                else if (this is FanLeft)
                {
                    rectTL = new Vec2(topLeft.x - windRange, topLeft.y + 1f);
                    rectBR = bottomRight - new Vec2(0f, 1f);
                    speedMult = new Vec2(-1f, 0f);
                }
                else if (this is FanRight)
                {
                    rectTL = topLeft + new Vec2(0f, 1f);
                    rectBR = new Vec2(bottomRight.x + windRange, bottomRight.y - 1f);
                    speedMult = new Vec2(1f, 0f);
                }

                blockers.Clear();
                foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(rectTL, rectBR))
                    if (physicsObject.thickness >= 4f && physicsObject.weight >= 7f)
                        blockers.Add(physicsObject);

                if (nextSpawn == 0)
                    SpawnParticle();
                else
                    nextSpawn--;

                foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(rectTL, rectBR))
                {
                    if (blockers.Contains(physicsObject)
                        || (!(physicsObject is Duck) && physicsObject.weight >= 7f)
                        || !(this is FanDown)
                        && ((physicsObject is Duck && ((Duck)physicsObject).HasEquipment(typeof(IForcedMovementImmunity)))
                        || physicsObject is IForcedMovementImmunity))
                        continue;
                    Duck d = physicsObject as Duck;
                    List<PhysicsObject> obstructors = blockers.Intersect(Level.CheckLineAll<PhysicsObject>(position, physicsObject.position)).ToList();
                    int numberOfObstructors = obstructors.Count();
                    bool holdBypass = false;
                    if (d != null)
                        holdBypass = (numberOfObstructors == 1 && obstructors[0] == d.holdObject);
                    if (Level.CheckLine<Block>(position, physicsObject.position, this) == null
                        && (numberOfObstructors == 0 || holdBypass)
                        && (d == null || d.holdObject == null || d.holdObject.thickness < 4f || d.holdObject.weight < 7f || this is FanDown))
                    {
                        float distance = (physicsObject.position - position).length;
                        Vec2 propulsion = (_force + 8f - distance / (4f * range)) * speedMult;
                        if (propulsion.x != 0f && Math.Abs(physicsObject.hSpeed) < Math.Abs(propulsion.x))
                            physicsObject.hSpeed = MathHelper.Lerp(physicsObject.hSpeed, propulsion.x, 0.06f);
                        if (propulsion.y != 0f && Math.Abs(physicsObject.vSpeed) < Math.Abs(propulsion.y) && (propulsion.y < 0f || !physicsObject.grounded))
                        {
                            if (d != null)
                            {
                                d.crouch = false;
                                d.sliding = false;
                            }
                            physicsObject.vSpeed = MathHelper.Lerp(physicsObject.vSpeed, propulsion.y, 0.06f);
                        }
                    }
                }

                if (!_decreasing)
                {
                    if (_force < 1.5f)
                        _force = MathHelper.Lerp(_force, 1.75f, 0.06f);
                    else
                    {
                        _force = MathHelper.Lerp(_force, -0.25f, 0.06f);
                        _decreasing = true;
                    }
                }
                else
                {
                    if (_force > 0f)
                        _force = MathHelper.Lerp(_force, -0.25f, 0.06f);
                    else
                    {
                        _force = MathHelper.Lerp(_force, 1.75f, 0.06f);
                        _decreasing = false;
                    }
                }
            }
            else
            {
                sprite.speed = 0f;
                nextSpawn = 0;
            }

            base.Update();
        }
    }

    internal class FanWind : Thing
    {
        private float timer;
        private Vec2 endPos;
        private SpriteMap sprite;
        private IList<PhysicsObject> blockers;

        public FanWind(float xpos, float ypos, Vec2 finish, IList<PhysicsObject> heavyObjects)
            : base(xpos, ypos)
        {
            depth = -0.6f;
            alpha = 1f;
            endPos = finish;
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\blocks\\fart"), 2, 2);
            graphic = sprite;
            center = new Vec2(1f, 1f);
            blockers = heavyObjects;
        }

        public override void Update()
        {
            if (timer >= 20f && sprite.frame == 0)
                sprite.frame = Rando.Int(1, 4);

            float angleToObject = (float)Math.Atan2(endPos.y - y, endPos.x - x);
            float distanceToObject = (endPos - position).length;
            Vec2 toPosition = new Vec2(x + (distanceToObject / 20f) * (float)Math.Cos(angleToObject),
                y + (distanceToObject / 20f) * (float)Math.Sin(angleToObject));

            IEnumerable<PhysicsObject> physicsLine = Level.CheckLineAll<PhysicsObject>(position, toPosition);
            IEnumerable<Block> blockLine = Level.CheckLineAll<Block>(position, toPosition);
            if (timer > 50f || (blockLine.Where(x => !(x is Fan)).Count() > 0 || (blockers.Except(physicsLine).Count() != blockers.Count())))
                Level.Remove(this);
            else
                position = toPosition;

            if (alpha > 0f)
                alpha -= 1f / 50f;
            timer++;
        }
    }

    [EditorGroup("uff|stuff|blocks|fans")]
    public class FanUp : Fan, IDontMove, IPathNodeBlocker, IPlatform
    {
        public FanUp(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _editorName = "Fan Up";
            collisionSize = new Vec2(16f, 8f);
            collisionOffset = new Vec2(-8f, -4f);
            hugWalls = WallHug.Floor;
        }
    }

    [EditorGroup("uff|stuff|blocks|fans")]
    public class FanDown : Fan, IDontMove, IPathNodeBlocker, IPlatform
    {
        public FanDown(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _editorName = "Fan Down";
            collisionSize = new Vec2(16f, 8f);
            collisionOffset = new Vec2(-8f, -4f);
            hugWalls = WallHug.Ceiling;
            angleDegrees = 180f;
        }
    }

    [EditorGroup("uff|stuff|blocks|fans")]
    public class FanLeft : Fan, IDontMove, IPathNodeBlocker, IPlatform
    {
        public FanLeft(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _editorName = "Fan Left";
            collisionSize = new Vec2(8f, 16f);
            collisionOffset = new Vec2(-4f, -8f);
            hugWalls = WallHug.Right;
            angleDegrees = -90f;
        }
    }

    [EditorGroup("uff|stuff|blocks|fans")]
    public class FanRight : Fan, IDontMove, IPathNodeBlocker, IPlatform
    {
        public FanRight(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _editorName = "Fan Right";
            collisionSize = new Vec2(8f, 16f);
            collisionOffset = new Vec2(-4f, -8f);
            hugWalls = WallHug.Left;
            angleDegrees = 90f;
        }
    }
}