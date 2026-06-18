using System.Collections.Generic;
using System.Linq;

namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Guns)]
    [BaggedProperty("previewPriority", true)]
    [GunGameLevel(14)]
    public class Needler : HaloWeapon, IHomingWeapon
    {
        private readonly SpriteMap _sight = Resources.LoadSpriteMap("needlerSight.png", 31, 31);
        private readonly string _targetLockSoundPath = Paths.GetSoundPath("needlerTargetLock.wav");
        private readonly int _maxAmmo = 40;

        private readonly Vec2[] _spikeOffsets = new Vec2[]
        {
            new Vec2(-6f, -5f),
            new Vec2(-2f, -4f),
            new Vec2(2f, -4f),
            new Vec2(6f, -2f)
        };

        private Duck _target;

        public Needler(float x, float y) : base(x, y)
        {
            ammo = _maxAmmo;

            _barrelOffsetTL = new Vec2(17f, 9f);
            _holdOffset = -new Vec2(3f, 1f);
            _ammoType = new ATHomingBullet();
            _bulletColor = _ammoType.bulletColor;
            _fireWait = 0.8f;
            _fullAuto = true;
            _fireSound = Paths.GetSoundPath("needlerFire.wav");

            _sight.CenterOrigin();
            _sight.AddAnimation("aim", 0.6f, false, Enumerable.Range(0, 10).ToArray());
            _sight.AddAnimation("scope", 0.04f, true, new int[]
            {
                10,
                9
            });
        }

        public Duck Target => _target;

        public override void Update()
        {
            base.Update();

            if (isServerForObject)
            {
                Duck newTarget = GetTarget();

                if (newTarget != _target)
                {
                    SetTarget(newTarget);
                    DuckNetwork.SendToEveryone(new NMSetNeedlerTarget(this, newTarget));
                }
            }

            if (_target is not null && _sight.currentAnimation == "aim")
            {
                if (_sight.finished)
                {
                    _sight.SetAnimation("scope");
                    return;
                }

                _sight.alpha = (_sight.frame + 1f) / 11f;
            }
        }

        public override void Draw()
        {
            base.Draw();

            if (_target is not null)
                Graphics.Draw(_sight, _target.x, _target.y + 4f, _target.depth.value + 0.1f);
        }

        public override void Fire()
        {
            base.Fire();

            int maxFrame = SpriteMap.texture.width / SpriteMap.width - 1;
            int frame = (int)(maxFrame - ammo / (float)_maxAmmo * maxFrame);
            
            if (SpriteMap.frame != frame && frame > 0)
            {
                SpriteMap.frame = frame;
                Vec2 spikePosition = Offset(_spikeOffsets[frame - 1]);

                Level.Add(new NeedlerSpike(spikePosition.x, spikePosition.y)
                {
                    hSpeed = Rando.Float(0.1f, 0.5f) * -offDir,
                    vSpeed = -Rando.Float(0.1f, 0.5f)
                });
            }
        }

        public void SetTarget(Duck target)
        {
            _target = target;

            if (target is not null)
            {
                _sight.frame = 0;
                _sight.SetAnimation("aim");

                SFX.Play(_targetLockSoundPath);
            }
        }

        protected override Sprite CreateGraphics(string spritePath)
        {
            return new SpriteMap(spritePath, 19, 16);
        }

        private Duck GetTarget()
        {
            if (duck is null)
                return null;

            foreach (Duck enemy in Level.current.things.OfType<Duck>().OrderBy(duck => Distance(duck)))
            {
                if (enemy == duck || enemy.dead)
                    continue;

                if (enemy.x > x && offDir < 0 || enemy.x < x && offDir > 0)
                    continue;

                IEnumerable<MaterialThing> things = Level.CheckLineAll<MaterialThing>(Offset(barrelOffset), enemy.position);

                if (things.Where(thing => thing.thickness > _ammoType.penetration).Any())
                    continue;

                if (Vec2.Distance(Offset(barrelOffset), enemy.position) > _ammoType.range)
                    continue;

                return enemy;
            }

            return null;
        }
    }
}
