using System;
using System.Linq;

namespace DuckGame.UFFMod
{
    public class Ghost : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _parentLanternStateBinding = new StateBinding("_parentLantern");
        public StateBinding _controlDuckStateBinding = new StateBinding("_controlDuck");
        public StateBinding _targetDuckStateBinding = new StateBinding("_targetDuck");
        public StateBinding _targetPosStateBinding = new CompressedVec2Binding("_targetPos");
        public StateBinding _waitForPlayerStateBinding = new StateBinding("_waitForPlayer");
        public StateBinding _fadingStateBinding = new StateBinding("_fading");
        public StateBinding _flippedStateBinding = new StateBinding("_flipped");
        public StateBinding _startSpawnedStateBinding = new StateBinding("_startSpawned");
        public StateBinding _spawnCountStateBinding = new StateBinding("_spawnCount");
        public StateBinding _finishedStateBinding = new StateBinding("_finished");

        public GhostLantern _parentLantern;
        public Duck _controlDuck;
        public Duck _targetDuck;
        public Vec2 _targetPos;
        public bool _waitForPlayer;
        public bool _fading;
        public bool _flipped;
        public bool _startSpawned;
        public int _spawnCount;
        public int _finished;

        protected SpriteMap sprite;
        protected bool reaper;
        protected bool followCamAdded;

        public Ghost(float xpos, float ypos, bool startSpawned = false, bool waitForPlayer = false, GhostLantern parentLantern = null)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\hazards\\ghost"), 20, 24);
            sprite.AddAnimation("haunt", 0.125f, true, 0, 1, 2, 3);
            sprite.SetAnimation("haunt");
            graphic = sprite;
            center = new Vec2(10f, 12f);
            collisionOffset = new Vec2(-10f, -12f);
            collisionSize = new Vec2(20f, 24f);
            depth = 0.5f;
            _startSpawned = startSpawned;
            _waitForPlayer = waitForPlayer;
            _parentLantern = parentLantern;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                alpha = 0.5f;
            else
                alpha = 0f;

            base.Initialize();
        }

        public override void Terminate()
        {
            FollowCam followCam = Level.current.camera as FollowCam;
            if (followCam != null)
                followCam.Remove(this);

            base.Terminate();
        }

        public override void Update()
        {
            if (!_startSpawned && _spawnCount < 180)
            {
                _flipped = flipHorizontal;
                _spawnCount++;
                return;
            }

            if (_waitForPlayer)
                return;

            if (!followCamAdded)
            {
                FollowCam followCam = Level.current.camera as FollowCam;
                if (followCam != null)
                {
                    followCam.Add(this);
                    followCamAdded = true;
                }
            }

            if (_finished < 5)
                _fading = !reaper && Level.CheckCircle<GhostLantern>(position, 160f) == null;
            else
                _fading = true;

            if (!_fading)
            {
                if (alpha < 0.5f)
                    alpha += 0.025f;
                else if (_controlDuck == null)
                {
                    bool finishedCheck = false;
                    float shortestDistance = 99999f;
                    foreach (Thing thing in Level.current.things)
                    {
                        Duck d = thing as Duck;
                        RagdollPart r = thing as RagdollPart;
                        if (d != null && !d.dead && !d.destroyed && (d.position - position).length < shortestDistance)
                        {
                            shortestDistance = (d.position - position).length;
                            _targetPos = d.position;
                            _targetDuck = d;
                            finishedCheck = true;
                        }
                        else if (r != null && r._doll != null && r._doll._duck != null && !r._doll._duck.dead && !r._doll._duck.destroyed && (r.position - position).length < shortestDistance)
                        {
                            shortestDistance = (r.position - position).length;
                            _targetPos = r.position;
                            _targetDuck = r._doll._duck;
                            finishedCheck = true;
                        }
                    }

                    if (_targetPos != null)
                    {
                        Vec2 positionDiff = _targetPos - position;
                        hSpeed += positionDiff.normalized.x / 8f;
                        vSpeed += positionDiff.normalized.y / 8f;
                        LimitSpeeds();

                        if (_targetDuck != null && !_targetDuck.dead && !_targetDuck.destroyed)
                            UpdatePosition();

                        _flipped = positionDiff.x < 0f;
                    }
                    else
                        _flipped = hSpeed < 0f;

                    CheckCollision();

                    if (!finishedCheck)
                        _finished++;
                }
                else
                {
                    bool finishedCheck = false;
                    foreach (Thing thing in Level.current.things)
                    {
                        Duck d = thing as Duck;
                        RagdollPart r = thing as RagdollPart;
                        if ((d != null && !d.dead && !d.destroyed) || (r != null && r._doll != null && r._doll._duck != null && !r._doll._duck.dead && !r._doll._duck.destroyed))
                        {
                            finishedCheck = true;
                            break;
                        }
                    }

                    if (_controlDuck.inputProfile.Down(Triggers.Up))
                        vSpeed -= 0.125f;
                    if (_controlDuck.inputProfile.Down(Triggers.Down))
                        vSpeed += 0.125f;
                    if (_controlDuck.inputProfile.Down(Triggers.Left))
                        hSpeed -= 0.125f;
                    if (_controlDuck.inputProfile.Down(Triggers.Right))
                        hSpeed += 0.125f;

                    LimitSpeeds();
                    UpdatePosition();

                    vSpeed = MathHelper.Lerp(vSpeed, 0f, 0.02f);
                    hSpeed = MathHelper.Lerp(hSpeed, 0f, 0.02f);

                    if (isServerForObject)
                        _flipped = _controlDuck.inputProfile.Down(Triggers.Left) || (hSpeed < 0f && !_controlDuck.inputProfile.Down(Triggers.Right));

                    CheckCollision();

                    if (!finishedCheck)
                        _finished++;
                }
            }
            else if (alpha > 0f)
                alpha -= 0.025f;
            else if (_parentLantern != null)
            {
                _parentLantern.GhostSpawned = false;
                Level.Remove(this);
            }

            base.Update();
        }

        public void Exorcise()
        {
            if (isServerForObject)
                for (int i = 0; i < 6; i++)
                {
                    GlobalSmoke smoke = new GlobalSmoke(x + Rando.Float(-8f, 8f), y + Rando.Float(-8f, 8f), Rando.Float(24f, 32f));
                    Level.Add(smoke);
                    smoke.xscale = smoke.yscale = Rando.Float(0.3f, 0.4f);
                    smoke.vSpeed = -Rando.Float(0.4f, 1.2f);
                }
            if (_parentLantern != null)
                _parentLantern.Unlit = true;
            Level.Remove(this);
        }

        private void LimitSpeeds()
        {
            if (hSpeed > 1f)
                hSpeed = 1f;
            if (hSpeed < -1f)
                hSpeed = -1f;
            if (vSpeed > 1f)
                vSpeed = 1f;
            if (vSpeed < -1f)
                vSpeed = -1f;
        }

        private void UpdatePosition()
        {
            x += hSpeed;
            y += vSpeed;
        }

        private void CheckCollision()
        {
            foreach (Duck d in Level.CheckCircleAll<Duck>(position, 12f))
            {
                if (Collision.Circle(position, 4f, d))
                    d.Kill(new DTImpact(this));
                if (d.gun != null && d.gun is GoodBook && d.grounded && d.inputProfile.Down(Triggers.Shoot))
                    Exorcise();
            }
            foreach (RagdollPart r in Level.CheckCircleAll<RagdollPart>(position, 4f))
                if (r._doll != null && r._doll._duck != null)
                    r._doll._duck.Kill(new DTImpact(this));
        }

        public override void Draw()
        {
            if (Level.current is Editor)
                sprite.flipH = flipHorizontal;
            else
                sprite.flipH = _flipped;

            base.Draw();
        }
    }
}
