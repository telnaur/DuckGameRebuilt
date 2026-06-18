using System;
using System.Collections.Generic;

namespace DuckGame.HaloWeapons
{
    public class DropWallTile : SyncedPositionBlock
    {
        private readonly List<BulletHit> _hits = new List<BulletHit>();
        private readonly Color _inactiveColor = new Color(144, 139, 139);
        private readonly Color _goodConditionColor = new Color(227, 211, 99);
        private readonly Color _badConditionColor = new Color(227, 111, 99);
        private readonly int _initialHealth = 5;

        [Binding] private int _health;

        private EventHandler _break;
        private Vec2 _lastBulletEnterPosition;

        public DropWallTile(float x, float y) : base(x, y)
        {
            graphic = Resources.LoadSprite("dropWallTile.png");
            depth = -0.5f;

            _health = _initialHealth;

            graphic.CenterOrigin();
            SetCollosionBox(new Vec2(6f, 3f));
        }

        [Binding] public bool Active { get; set; }

        public bool BreakEventIsEmpty => _break is null;

        public event EventHandler Break
        {
            add
            {
                _break += value;
            }

            remove
            {
                _break -= value;
            }
        }

        [Binding]
        public float Height
        {
            get
            {
                return height;
            }

            set
            {
                SetCollosionBox(new Vec2(width, value));
            }
        }

        public override sbyte Direction
        {
            get
            {
                return base.Direction;
            }

            set
            {
                graphic.flipH = value < 1;
                base.Direction = value;
            }
        }

        private Color Color => Active ? Lerp.ColorSmooth(_goodConditionColor, _badConditionColor, 1f - (float)_health / _initialHealth) : _inactiveColor;

        private Color DarkerColor
        {
            get
            {
                Color color = Color;
                color.a = 255;

                return Lerp.Color(color, Color.Black, 0.4f);
            }
        }

        public override bool Hit(Bullet bullet, Vec2 hitPosition)
        {
            for (int i = 0; i < 7; i++)
            {
                Level.Add(new DropWallParticle(x + Rando.Float(width), y + Rando.Float(height))
                {
                    hSpeed = Rando.Float(-3f, 3f),
                    vSpeed = -Rando.Float(1f),

                    Color = DarkerColor
                });
            }

            Vec2 direction = bullet.travelDirNormalized;
            float directionX = direction.x;

            if (Active && Direction > 0 && directionX < 0f || Direction < 0 && directionX > 0f)
            {
                if (_health <= 0)
                {
                    if (isServerForObject)
                    {
                        Level.Remove(this);
                        _break?.Invoke(this, EventArgs.Empty);
                    }

                    return false;
                }

                if (isServerForObject)
                    _health -= 1;

                return true;
            }

            _lastBulletEnterPosition = FixBulletPosition(hitPosition);

            return false;
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPosition)
        {
            _hits.Add(new BulletHit(_lastBulletEnterPosition, FixBulletPosition(exitPosition)));
        }

        public override void Terminate()
        {
            for (int i = 0; i < 8; i++)
            {
                Level.Add(new GlassParticle(left + i * width / 8f, y)
                {
                    hSpeed = GetGlassParticleSpeed(),
                    vSpeed = GetGlassParticleSpeed(),

                    Color = DarkerColor
                });
            }
        }

        public override void Draw()
        {
            graphic.color = Color;

            Graphics.Draw(graphic, x + width / 2f, y + 1f);
            Graphics.DrawRect(new Rectangle(position, position + collisionSize), Color.Black, depth, false);

            foreach (BulletHit hit in _hits)
                Graphics.DrawLine(hit.Enter, hit.Exit, DarkerColor);
        }

        private void SetCollosionBox(Vec2 size)
        {
            graphic.yscale = size.y - 2f;
            collisionSize = size;
        }

        private Vec2 FixBulletPosition(Vec2 position)
        {
            float x = Maths.Clamp(position.x, left, right);
            float y = Maths.Clamp(position.y, top, bottom);

            return new Vec2(x, y);
        }

        private float GetGlassParticleSpeed()
        {
            return Rando.Float(-2f);
        }

        private readonly record struct BulletHit(Vec2 Enter, Vec2 Exit);
    }
}
