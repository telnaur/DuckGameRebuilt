using System;
using System.Linq;

namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Guns)]
    public class SpikeGrenade : HaloGrenade
    {
        [Binding] private bool _stuck;
        [Binding] private Vec2 _stickPosition;

        private StickThing _stickThing;
        private sbyte _stickThingInitialDirection;
        private float _stickThingInitialAngle;

        private float _stickAngle;

        public SpikeGrenade(float x, float y) : base(x, y)
        {
            Timer = 1.5f;
            ActivateSound = Paths.GetSoundPath("spikeGrenadeActivate.wav");
            ExplosionSound = Paths.GetSoundPath("spikeGrenadeExplosion.wav");

            _ammoType = new ATSpikeGrenadeSharpnel();
            _bouncy = 0f;
        }

        private bool CanStick => Activated && hoverSpawner is null;

        public override void Update()
        {
            base.Update();

            if (isServerForObject)
            {
                if (_stickThing is not null)
                {
                    UpdatePosition();

                    if (_stickThing.Removed)
                    {
                        updatePhysics = true;
                        canPickUp = true;
                        _stuck = false;

                        _stickThing = null;
                    }
                }
                else
                {
                    if (!_grounded && duck is null)
                        _framesSinceThrown++;
                    else
                        _framesSinceThrown = 0;

                    if (_framesSinceThrown > 30 && _lastThrownBy is MaterialThing thing)
                        clip.Remove(thing);
                }
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnSoftImpact(with, from);

            if (with is Duck)
                OnCollide(with);
        }

        public override void SolidImpact(MaterialThing with, ImpactedFrom from)
        {
            base.SolidImpact(with, from);

            OnCollide(with);
        }

        public override void Thrown()
        {
            base.Thrown();

            if (_lastThrownBy is MaterialThing thing)
                clip.Add(thing);
        }

        public override void Draw()
        {
            if (_stuck && _stickThing is not null && !isServerForObject)
                UpdatePosition();

            base.Draw();
        }

        public void Stick(MaterialThing stickTo)
        {
            if (stickTo is null)
                return;

            sleeping = true;

            updatePhysics = false;
            canPickUp = false;
            velocity = Vec2.Zero;

            _stickThing = GetStickThing(stickTo, position);
            _stickThingInitialDirection = _stickThing.Direction;
            _stickThingInitialAngle = _stickThing.Angle;

            Vec2 thingPosition = _stickThing.Position;

            _stickAngle = Maths.PointDirectionRad(position, thingPosition);

            if (isServerForObject)
            {
                _stickPosition = thingPosition - position;
                _stuck = true;
            }
        }

        protected override void Activate()
        {
            base.Activate();

            PopShell();
        }

        protected override void Explode()
        {
            if (isServerForObject)
            {
                float twoPI = Maths.PI * 2f;

                for (float angle = 0f; angle <= twoPI; angle += twoPI / 15f)
                {
                    float direction = angle + Rando.Float(-0.2f, 0.2f);

                    float sin = (float)Math.Sin(direction);
                    float cos = (float)Math.Cos(direction);

                    Bullet sharpnel = _ammoType.GetBullet(x + cos * 5f, y + -sin * 5f, owner, Maths.RadToDeg(direction), this);
                    firedBullets.Add(sharpnel);
                    Level.Add(sharpnel);
                }

                if (Network.isActive)
                {
                    DuckNetwork.SendToEveryone(new NMFireGun(this, firedBullets, bulletFireIndex, false));
                    firedBullets.Clear();
                }

                foreach (Window window in Level.CheckCircleAll<Window>(position, _ammoType.range / 2f))
                    if (Level.CheckLine<Block>(position, window.position) is null)
                        window.Destroy(new DTImpact(this));
            }

            Graphics.FlashScreen();

            base.Explode();
        }

        private StickThing GetStickThing(MaterialThing thing, Vec2 stickPosition)
        {
            if (thing is Duck duck)
                return new StickDuck(duck, stickPosition);

            return new StickThing(thing);
        }

        private void OnCollide(MaterialThing with)
        {
            if (!isServerForObject || _stuck || !CanStick)
                return;

            Stick(with);
            Send.Message(new NMSpikeGrenadeStick(this, with));
        }

        private void UpdatePosition()
        {
            _stickThing.Update();

            Vec2 stickThingPosition = _stickThing.Position;

            float xOffset = _stickPosition.x;
            float directionOffset = -(_stickThing.Angle - _stickThingInitialAngle);

            if (_stickThing.Direction != _stickThingInitialDirection)
            {
                xOffset = -xOffset;
                directionOffset = -directionOffset;

                angle = -_stickAngle;
            }
            else
            {
                angle = _stickAngle;
            }

            angle += directionOffset;

            Vec2 rawPosition = stickThingPosition - new Vec2(xOffset, _stickPosition.y);

            position = rawPosition.Rotate(directionOffset, stickThingPosition);
            depth = _stickThing.Depth + 0.1f;
        }

        private class StickThing
        {
            private readonly MaterialThing _thing;

            public StickThing(MaterialThing thing)
            {
                _thing = thing;
            }

            public virtual Vec2 Position => _thing.position;
            public virtual bool Removed => IsRemoved(_thing);
            public virtual float Angle => _thing.angle;

            public float Depth => _thing.depth.value;
            public sbyte Direction => _thing.offDir;

            protected MaterialThing Thing => _thing;

            public virtual void Update()
            {

            }

            protected bool IsRemoved(Thing thing)
            {
                return !Level.current.things.Contains(thing);
            }
        }

        private sealed class StickDuck : StickThing
        {
            private readonly DuckBone _bone;

            public StickDuck(Duck duck, Vec2 stickPosition) : base(duck)
            {
                DuckSkeleton skeleton = duck.skeleton;

                DuckBone[] bones = new DuckBone[]
                {
                    skeleton.head,
                    skeleton.upperTorso,
                    skeleton.lowerTorso
                };

                _bone = bones.OrderBy(bone => Vec2.Distance(bone.position, stickPosition)).First();

                Update();
            }

            public override Vec2 Position => _bone.position;
            public override bool Removed => base.Removed && IsRemoved(Duck.ragdoll);
            public override float Angle => _bone.orientation;

            private Duck Duck => Thing as Duck;

            public override void Update()
            {
                Duck.UpdateSkeleton();
            }
        }
    }
}
