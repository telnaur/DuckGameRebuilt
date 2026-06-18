using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.TarGunMod
{
    public class TarParticle : Thing
    {
        private const int kMaxObjects = 256;
        private static readonly TarParticle[] _objects = new TarParticle[TarParticle.kMaxObjects];
        private static int _lastActiveObject = 0;
        public SpriteMap _sprite;
        private float _life = 1f;
        private float rotationSpeedMultiplier = Rando.Float(0.85f, 1.15f);
        private float lifeTake = 0.05f;
        private bool rotatingClockwise = (Rando.Int(0, 1) == 1);
        private float travelSpeed;

        public static TarParticle New(float xpos, float ypos, float hSpeed, float vSpeed, float scaleMultiplier = 1f)
        {
            TarParticle tarParticle;
            if (TarParticle._objects[TarParticle._lastActiveObject] == null)
            {
                tarParticle = new TarParticle();
                TarParticle._objects[TarParticle._lastActiveObject] = tarParticle;
            }
            else
            {
                tarParticle = TarParticle._objects[TarParticle._lastActiveObject];
            }

            TarParticle._lastActiveObject = (TarParticle._lastActiveObject + 1) % TarParticle.kMaxObjects;
            tarParticle.Init(xpos, ypos);
            tarParticle.ResetProperties();
            tarParticle.hSpeed = hSpeed;
            tarParticle.vSpeed = vSpeed;
            tarParticle._sprite.globalIndex = (int)Thing.GetGlobalIndex();
            tarParticle.globalIndex = Thing.GetGlobalIndex();
            tarParticle._sprite.scale *= scaleMultiplier * Rando.Float(0.85f,1.25f);
            return tarParticle;
        }

        public static TarParticle New(float xpos, float ypos, float scaleMul = 1f)
        {
            TarParticle tarParticle;
            if (TarParticle._objects[TarParticle._lastActiveObject] == null)
            {
                tarParticle = new TarParticle();
                TarParticle._objects[TarParticle._lastActiveObject] = tarParticle;
            }
            else
            {
                tarParticle = TarParticle._objects[TarParticle._lastActiveObject];
            }

            TarParticle._lastActiveObject = (TarParticle._lastActiveObject + 1) % TarParticle.kMaxObjects;
            tarParticle.Init(xpos, ypos);
            tarParticle.ResetProperties();
            tarParticle.hSpeed = Rando.Float(-0.15f, 0.15f);
            tarParticle.vSpeed = Rando.Float(-0.15f, 0.1f);
            tarParticle.depth = (Depth)0.8f;
            return tarParticle;
        }

        private TarParticle()
          : base()
        {
            this._sprite = new SpriteMap(GetPath("tarBlast"), 16, 16, true)
            {
                frame = Rando.Int(2) + 15
            };
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
        }

        private void Init(float xpos, float ypos)
        {
            this._life = 1f;
            this.position.x = xpos;
            this.position.y = ypos;
            this._sprite.angleDegrees = Rando.Float(360f);
            this._sprite.scale = new Vec2(Rando.Float(0.6f, 1f));
            this._sprite.depth = (Depth)1f;
            this._sprite.center = center;
            this._life += Rando.Float(0.2f);
            this.depth = (Depth)0.8f;
            this.alpha = 1f;
            this.layer = Layer.Blocks;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            this.xscale = 1f;
            this.yscale = this.xscale;
            this.vSpeed += 0.05f;
            this.hSpeed *= 0.975f;
            this._life -= this.lifeTake;
            if ((double)this._life < 0.0)
            {
                this.alpha -= 0.01f;
            }
            if (this.alpha <= 0.05f)
                Level.Remove((Thing)this);
            this.x += this.hSpeed;
            this.y += this.vSpeed;

            travelSpeed = (float)Math.Sqrt(((this.hSpeed * this.hSpeed) + (this.vSpeed * this.vSpeed)));
            if (this.hSpeed != 0) rotatingClockwise = this.hSpeed > 0;
            this._sprite.angleDegrees += rotatingClockwise ?
                  (float)Math.Sqrt(travelSpeed) * rotationSpeedMultiplier
                : (float)Math.Sqrt(travelSpeed) * -rotationSpeedMultiplier;
        }

        public override void Draw()
        {
            this._sprite.alpha = this.alpha;
            Graphics.Draw((Sprite)this._sprite, this.x, this.y);
        }
    }
}
