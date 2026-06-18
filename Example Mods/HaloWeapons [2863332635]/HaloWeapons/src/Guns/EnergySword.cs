using System.Text;

namespace DuckGame.HaloWeapons
{
    [NoSkins]
    [EditorGroup(EditorGroups.Guns)]
    [BaggedProperty("previewPriority", true)]
    [GunGameLevel(10)]
    public class EnergySword : HaloWeapon
    {
        private readonly Vec2 _maxVelocityGained = new Vec2(9f, -1.75f);

        private readonly float _punchRadius = 12f;
        private readonly float _maxPunchTime = 0.2f;
        private readonly float _maxChargeOffsetX = -5f;
        private readonly float _maxPunchOffsetX = 10f;   

        [Binding] private Vec2 _offset;

        private Vec2 _defaultHoldOffet = new Vec2(1f);

        private float _additionalTiltTimer = 0.1f;
        private float _punchTimer;
        private float _charge;

        public EnergySword(float x, float y) : base(x, y)
        {
            ammo = 10;

            _holdOffset = _defaultHoldOffet;
        }

        private bool WeaponRaised => duck is not null && (duck._hovering || duck.holdObstructed);
        private bool CanOperate => !WeaponRaised && isServerForObject && _wait <= 0f && ammo > 0;

        public override void Update()
        {
            base.Update();

            if (isServerForObject)
            {
                if (WeaponRaised)
                {
                    ResetValues();
                }
                else
                {
                    if (_charge >= 1f)
                    {
                        _additionalTiltTimer -= 0.01f;

                        if (_additionalTiltTimer <= 0f)
                        {
                            _offset += new Vec2(GetAdditionalTiltValue(), GetAdditionalTiltValue());
                            _additionalTiltTimer = 0.1f;
                        }
                    }
                    else if (_offset.x > 0f)
                    {
                        LerpOffset(Vec2.Zero, 0.06f);
                    }
                }

                if (_punchTimer > 0f)
                {
                    _punchTimer -= 0.01f;

                    foreach (MaterialThing thing in Level.CheckCircleAll<MaterialThing>(position, _punchRadius))
                    {
                        if (!CanDestroy(thing))
                            continue;

                        if (thing.Destroy(new DTImpact(this)))
                        {
                            ammo -= 1;

                            if (ammo <= 0)
                            {
                                if (isServerForObject)
                                {
                                    DuckNetwork.SendToEveryone(new NMEnergySwordNoAmmo(this));
                                    OnRanOutOfAmmo();
                                }

                                _punchTimer = 0f;

                                break;
                            }
                        }
                    }
                }
            }

            handOffset = _offset;
            _holdOffset = _defaultHoldOffet + _offset;
        }

        public override void OnHoldAction()
        {
            if (!CanOperate)
                return;

            LerpOffset(new Vec2(_maxChargeOffsetX, 0f), 0.1f);

            if (_charge < 1f)
                _charge += 0.035f;
        }

        public override void OnReleaseAction()
        {
            if (!CanOperate)
                return;

            _offset = new Vec2(_maxPunchOffsetX, 0f);

            if (duck is not null)
            {
                _punchTimer = _maxPunchTime * _charge;

                float dashMultiplier = _charge >= 0.3f ? _charge : 0f;
                float maxVelocityGainedY = _maxVelocityGained.y;
                duck.velocity += new Vec2(_maxVelocityGained.x * _offDir, duck.grounded ? maxVelocityGainedY : maxVelocityGainedY * 2f - duck.velocity.y) * dashMultiplier;
            }

            _charge = 0f;
            _wait = 4f;
        }

        public override void Thrown()
        {
            base.Thrown();

            ResetValues();
            _punchTimer = 0f;
        }

        public override void Fire()
        {

        }

        public void OnRanOutOfAmmo()
        {
            SetCollisionBox(4f, 7f);
            _defaultHoldOffet = -new Vec2(6f, 2f);

            SpriteMap.frame += 1;

            var builder = new StringBuilder("ting");

            if (Rando.Int(1) == 0)
                builder.Append("2");

            SFX.Play(builder.ToString());

            Level.Add(SmallSmoke.New(position.x, position.y, depth.value + 0.1f));

        }

        protected override Sprite CreateGraphics(string spritePath)
        {
            return new SpriteMap(spritePath, 22, 13);
        }

        private bool CanDestroy(MaterialThing thing)
        {
            if (thing == this || thing == owner)
                return false;

            if (thing is Holdable holdable && holdable.hoverSpawner is not null)
                return false;

            if (owner is Duck duck && thing is Equipment equipment && duck.HasEquipment(equipment))
                return false;

            return true;
        }

        private float GetAdditionalTiltValue()
        {
            return Rando.Float(-1.05f, 1.05f);
        }

        private void LerpOffset(Vec2 lerpTo, float multiplier)
        {
            _offset = Lerp.Vec2(_offset, lerpTo, Vec2.Distance(_offset, lerpTo) * multiplier);
        }

        private void ResetValues()
        {
            _offset = Vec2.Zero;
            _charge = 0f;
        }
    }
}
