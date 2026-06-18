using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace DuckGame.HaloWeapons
{
    public class LightBeam : Beam
    {
        private readonly SpriteMap _muzzle = new SpriteMap(Paths.GetSpritePath("beamMuzzle.png"), 5, 9);
        private readonly float _maxLength = 100f;

        private readonly Texture2D[] _textures = new Texture2D[]
        {
            Resources.LoadTexture("lightBeam1.png"),
            Resources.LoadTexture("lightBeam2.png"),
            Resources.LoadTexture("lightBeam3.png")
        };

        private int _textureIndex;
        private int _spawnParticlesTimer = 5;

        public LightBeam(float x, float y) : base(x, y) 
        {
            Thickness = 0.5f;
            Range = 5f;
            Penetration = 6f;

            _muzzle.center = new Vec2(_muzzle.width - 1f, _muzzle.height / 2f);
            _muzzle.AddAnimation("idle", 1f, true, new int[]
            {
                0,
                1,
                2,
                1
            });
            _muzzle.SetAnimation("idle");
        }

        public override void Update()
        {
            base.Update();

            _textureIndex++;

            if (_textureIndex >= _textures.Length)
                _textureIndex = 0;

            Texture = _textures[_textureIndex];

            float muzzleAngle = angle > 0f && angle < Maths.PI ? angle - Maths.PI : angle;

            if (CurrentImpacting.Count() > 0 && CurrentLength <= Vec2.Distance(position, CurrentImpacting.First().position))
            {
                _spawnParticlesTimer--;

                if (_spawnParticlesTimer <= 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vec2 particlePosition = TravelEnd.Rotate(Rando.Float(-0.05f, 0.05f), position);

                        Level.Add(new BeamHitParticle(particlePosition.x, particlePosition.y, muzzleAngle - Maths.PI));
                    }

                    _spawnParticlesTimer = 5;
                }
            }
            else
            {
                _spawnParticlesTimer = 5;
            }

            _muzzle.angle = muzzleAngle;

            if (Range >= _maxLength)
                return;

            Range += 10f;
        }

        public override void Draw()
        {
            base.Draw();

            Graphics.Draw(_muzzle, TravelEnd.x, TravelEnd.y, depth.value + 0.1f);
        }
    }
}
