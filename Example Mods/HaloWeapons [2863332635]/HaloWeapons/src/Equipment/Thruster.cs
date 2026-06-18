using System;

namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Equipment)]
    [BaggedProperty("previewPriority", true)]
    public class Thruster : Equipment
    {
        private readonly SpriteMap _base = Resources.LoadSpriteMap("thrusterBase.png", 5, 7);
        private readonly Sprite _belt = Resources.LoadSprite("thrusterBelt.png");

        private readonly BoostTracePart?[] _boostTraceParts = new BoostTracePart?[10];

        private readonly Vec2 _baseOffset = new Vec2(-6f, -5f);
        private readonly Vec2 _beltOffset = new Vec2(-5f, 2f);

        [Binding] private bool _boosting;

        private MaterialPaintBucket[] _paintBuckets;
        private SpriteMap _boostTrace;
        private Vec2 _lastCapturedPosition;

        private float _defaultMaxRunSpeed;
        private float _reduceBoostTraceTimer = 0.1f;
        private float _boostTimer = 8f;

        public Thruster(float x, float y) : base(x, y)
        {
            graphic = Resources.LoadSprite("thruster.png");

            collisionSize = new Vec2(11f, 10f);
            center = collisionSize / 2f;
            collisionOffset = -center;

            physicsMaterial = PhysicsMaterial.Wood;

            _holdOffset = new Vec2(2f, 1.5f);
        }

        public override void Update()
        {
            base.Update();

            if (_boosting)
            {
                if (isServerForObject && _boostTimer <= 0f || netEquippedDuck is null || netEquippedDuck.dead)
                {
                    if (!SetDefaultSpeed(netEquippedDuck))
                        SetDefaultSpeed(_prevOwner as Duck);

                    if (isServerForObject)
                        Level.Remove(this);

                    return;
                }

                if (isServerForObject)
                    _boostTimer -= 0.01f;

                int length = _boostTraceParts.Length;

                if (netEquippedDuck.ragdoll is null)
                {
                    Vec2 velocity = netEquippedDuck.velocity;
                    float minDistance = (Math.Abs(velocity.x) > Math.Abs(velocity.y) ? netEquippedDuck.width : netEquippedDuck.height) + 1f;

                    int lastIndex = length - 1;

                    float distance = Vec2.Distance(netEquippedDuck.position, _lastCapturedPosition);
                    Vec2 duckPosition = netEquippedDuck.position;

                    if (distance >= minDistance)
                    {
                        for (int i = lastIndex; i > 0; i--)
                            _boostTraceParts[i] = _boostTraceParts[i - 1];

                        _boostTraceParts[0] = new BoostTracePart(netEquippedDuck.spriteFrame, netEquippedDuck._sprite.currentAnimation, duckPosition, netEquippedDuck.offDir);
                        _lastCapturedPosition = duckPosition;
                    }
                    else if (distance <= minDistance / 4f && _boostTraceParts[0] is null)
                    {
                        _lastCapturedPosition = duckPosition;
                    }

                    if (_reduceBoostTraceTimer < 0f)
                    {
                        for (int j = lastIndex; j >= 0; j--)
                        {
                            if (_boostTraceParts[j] is not null)
                            {
                                _boostTraceParts[j] = null;
                                break;
                            }
                        }

                        _reduceBoostTraceTimer = 0.03f;
                    }
                    else
                    {
                        _reduceBoostTraceTimer -= 0.01f;
                    }
                }
                else
                {
                    Array.Clear(_boostTraceParts, 0, length);
                }
            }
            else if (isServerForObject && netEquippedDuck is not null && netEquippedDuck.inputProfile.Pressed("QUACK"))
            {
                StartBoosting();
                DuckNetwork.SendToEveryone(new NMThrusterStartBoosting(this));
            }

            _base.frame = _boosting ? 1 : 0;

            if (owner is not null)
            {
                SetSpriteProperties(_base);
                SetSpriteProperties(_belt);
            }
        }

        public override void Draw()
        {
            if (netEquippedDuck is null)
            {
                base.Draw();
            }
            else
            {
                Ragdoll ragdoll = netEquippedDuck.ragdoll;
                MaterialThing ownerThing = ragdoll is null ? netEquippedDuck : ragdoll.part2;
                float ownerDepth = ownerThing.depth.value;

                Vec2 basePosition = Offset(_baseOffset);
                Vec2 beltPosition = Offset(_beltOffset);

                Graphics.Draw(_base, basePosition.x, basePosition.y, ownerDepth - 0.1f);
                Graphics.Draw(_belt, beltPosition.x, beltPosition.y, ownerDepth + 1f);

                if (!_boosting || _boostTrace is null)
                    return;

                int length = _boostTraceParts.Length;

                for (int i = 0; i < length; i++)
                {
                    BoostTracePart? boostTrace = _boostTraceParts[i];

                    if (boostTrace is null)
                        break;

                    BoostTracePart trace = boostTrace.Value;

                    _boostTrace.SetAnimation(trace.Animation);
                    _boostTrace.frame = trace.Frame;
                    _boostTrace.flipH = trace.Direction < 0;

                    Vec2 position = trace.Position;

                    Graphics.material = _paintBuckets[i];

                    Graphics.Draw(_boostTrace, position.x, position.y, netEquippedDuck.depth.value - 0.1f);
                }

                Graphics.material = null;
            }
        }

        public void StartBoosting()
        {
            if (netEquippedDuck is null)
                return;

            _lastCapturedPosition = netEquippedDuck.position;

            if (isServerForObject)
            {
                _defaultMaxRunSpeed = netEquippedDuck.runMax;
                netEquippedDuck.runMax = 5f;
                _boosting = true;
            }

            int length = _boostTraceParts.Length;
            _paintBuckets = new MaterialPaintBucket[length];

            for (int i = 0; i < length; i++)
                _paintBuckets[i] = new MaterialPaintBucket(netEquippedDuck.persona.colorUsable * ((float)(length - i) / length));

            _boostTrace = netEquippedDuck._sprite.Clone() as SpriteMap;

            SFX.Play(Paths.GetSoundPath("thrusterActivate.wav"), 0.2f);
        }

        private void SetSpriteProperties(Sprite sprite)
        {
            sprite.flipH = owner.offDir < 1;
            sprite.angle = angle;
            sprite.scale = scale;
        }

        private bool SetDefaultSpeed(Duck duck)
        {
            if (duck is null)
                return false;


            duck.runMax = _defaultMaxRunSpeed;
            return true;
        }

        private readonly record struct BoostTracePart(int Frame, string Animation, Vec2 Position, sbyte Direction);
    }
}
