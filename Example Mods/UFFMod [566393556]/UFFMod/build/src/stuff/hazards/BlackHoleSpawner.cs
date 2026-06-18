namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|hazards")]
    public class BlackHoleSpawner : Thing
    {
        // partially based off decompiled LaserSpawner

        public EditorProperty<float> delay = new EditorProperty<float>(1f, null, 0f, 100f, 0.25f);
        public EditorProperty<float> initialDelay = new EditorProperty<float>(0f, null, 0f, 100f, 0.25f);
        public EditorProperty<bool> instant = new EditorProperty<bool>(true);
        public EditorProperty<int> maxSpawns = new EditorProperty<int>(-1, null, -1f, 100f, 1f, "INF");
        public EditorProperty<int> fireAngle = new EditorProperty<int>(0, null, 0f, 360f, 1f);
        public EditorProperty<float> fireSpeed = new EditorProperty<float>(1f, null, 0f, 10f, 0.25f);

        private float spawnDelay;
        private int timesSpawned;

        public BlackHoleSpawner(float xpos, float ypos)
            : base(xpos, ypos)
        {
            graphic = new Sprite("laserSpawner", 0f, 0f);
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(12f, 12f);
            collisionOffset = new Vec2(-6f, -6f);
            depth = -0.6f;
            _visibleInGame = false;
            _canFlip = false;
        }

        public override void Initialize()
        {
            if (instant)
                spawnDelay = delay;
        }

        public override void Update()
        {
            if (Level.current.simulatePhysics)
                spawnDelay += 0.0166666f;
            if (Level.current.simulatePhysics && Network.isServer && (timesSpawned < maxSpawns || maxSpawns == -1) && spawnDelay >= delay)
            {
                if (initialDelay > 0f)
                    initialDelay -= 0.0166666f;
                else
                {
                    BlackHole bH = new BlackHole(position.x, position.y);
                    Level.Add(bH);
                    bH._fireSpeed = fireSpeed;
                    bH._fireAngle = fireAngle;
                    bH._released = true;
                    spawnDelay = 0f;
                    timesSpawned++;
                }
            }
        }

        public override void DrawHoverInfo()
        {
            Graphics.DrawLine(position, position + new Vec2(1f, -1f) * Maths.AngleToVec(Maths.DegToRad(fireAngle)) * (fireSpeed * 5f), Color.Red, 2f, (Depth)1f);
        }

        public override void Draw()
        {
            angleDegrees = fireAngle;
            base.Draw();
        }
    }
}
