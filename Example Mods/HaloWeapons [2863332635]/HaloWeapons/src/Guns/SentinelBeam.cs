namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Guns)]
    [GunGameLevel(4)]
    public class SentinelBeam : HaloWeapon
    {
        private readonly Sound _beamSound = Resources.LoadSound("sentinelBeamFire.wav", looped: true);

        [Binding] private float _charge = 2.5f;

        private LightBeam _beam;

        public SentinelBeam(float x, float y) : base(x, y)
        {
            ammo = 1;
            weight = 8f;

            _holdOffset = new Vec2(6f, 1f);
            _barrelOffsetTL = new Vec2(25f, 6f);
            _fireWait = 4f;
            _wait = _fireWait;
        }

        private Vec2 BeamPosition => Offset(barrelOffset);

        public override void Update()
        {
            base.Update();

            if (isServerForObject && _charge <= 0f && grounded)
            {
                Level.Remove(this);
                return;
            }

            if (_beam is null)
                return;

            _beam.position = BeamPosition;
            _beam.angle = barrelAngle;
            _beam.offDir = offDir;

            if (_charge <= 0 || !_triggerHeld || owner is null)
            {
                RemoveBeam();
                return;
            }

            if (isServerForObject) 
                _charge -= 0.01f;
        }

        public override void Fire()
        {

        }

        public override void OnHoldAction()
        {
            if (_wait > 0 || _charge <= 0 || _beam is not null)
                return;

            Vec2 beamPosition = BeamPosition;

            _beam = new LightBeam(beamPosition.x, beamPosition.y)
            {
                angle = barrelAngle,
                isLocal = isServerForObject,
                responsibleProfile = responsibleProfile
            };

            Level.Add(_beam);
            _beam.Update();

            _beamSound.Play();
        }

        public override void Thrown()
        {
            base.Thrown();

            RemoveBeam();
        }

        public override void Terminate()
        {
            RemoveBeam();
        }

        private void RemoveBeam()
        {
            if (_beam is null)
                return;

            Level.Remove(_beam);
            _beam = null;
            _wait = _fireWait;

            _beamSound.Stop();
        }
    }
}
