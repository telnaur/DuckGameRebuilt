using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame.TarGunMod.CustomFlags;

namespace DuckGame.TarGunMod
{
    public class TarBlast : PhysicsObject, IPlatform
    {
        //public StateBinding netDuckImpactBinding = (StateBinding)new NetSoundBinding("netDuckImpact");
        //public NetSoundEffect netDuckImpact = new NetSoundEffect(new string[1] { Mod.GetPath<TarGunMod>("spdBlast") }) { volume = 1f };

        //public StateBinding netSolidImpactBinding = (StateBinding)new NetSoundBinding("netSolidImpact");
        //public NetSoundEffect netSolidImpact = new NetSoundEffect(new string[1] { "swallow" }) { volume = 1f };

        public StateBinding _hitDuckBinding = new StateBinding("HitDuck");
        public bool HitDuck;
        public int ParticleTimer = Rando.Int(0,120);
        public int AliveFor = 0;

        private readonly SpriteMap _sprite;
        private readonly int _numSplats;
        private readonly float size;
        private bool rotatingClockwise;

        public TarBlast(float xpos, float ypos, float sizeMultiplier = 1, int numParticleSplats = 8)
          : base(xpos, ypos)
        {
            size = Rando.Float(0.875f,1.125f) * sizeMultiplier;
            this.owner = owner;

            this._sprite = new SpriteMap(GetPath("tarBlast"), 16, 16, true);
            this._sprite.AddAnimation("flying", 10 / 60f, true, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            this._sprite.AddAnimation("ground", 10 / 60f, true, 14, 13, 12, 12, 13);
            this.graphic = (Sprite)this._sprite;
            this._sprite.SetAnimation("flying");
            this.scale = new Vec2(size);

            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(12f, 12f) * size;
            this.collisionOffset = new Vec2(-12f, -12f) * (size/2);
            this.depth = (Depth)(0.5f);

            this.thickness = 2f;
            this.weight = 1.5f;
            this.buoyancy = 1f;
            this.flammable = 0.3f;
            this.breakForce = 9999999f;
            this.gravMultiplier = 0.7f * size;
            this._numSplats = numParticleSplats;
            this.collideSounds.Add("swallow");
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (HitDuck)
            {
                SFX.Play(GetPath("spdBlast"));
                for (int index = 0; index < this._numSplats; ++index)
                {
                    Level.Add((Thing)TarParticle.New(this.x - this.hSpeed, this.y - this.vSpeed, Rando.Float(10f) - 5f, Rando.Float(10f) - 7.5f, 1.75f*size));
                }
            }
            else
            {
                SFX.Play("swallow");
                for (int index = 0; index < this._numSplats / 2; ++index)
                {
                    Level.Add((Thing)TarParticle.New(this.x - this.hSpeed, this.y - this.vSpeed, Rando.Float(4f) - 2f, Rando.Float(4f) - 2.5f, 1f*size));
                }
            }
            Level.Remove((Thing)this);
            return true;
        }

        public override void Update()
        {
            AliveFor++;
            if (this.hSpeed != 0 || this.vSpeed != 0)
            {
                float travelSpeed = (float)Math.Sqrt(this.hSpeed * this.hSpeed + this.vSpeed * this.vSpeed);
                if (this.hSpeed != 0) rotatingClockwise = this.hSpeed > 0;
                this.angleDegrees += rotatingClockwise ? travelSpeed * 2 : -travelSpeed * 2;

                this._sprite.SetAnimation("flying");

                ParticleTimer -= 1 + (int)Math.Round(travelSpeed * travelSpeed / Rando.Float(2f, 6f));
            }
            else
            {
                this.Destroy((DestroyType)new DTImpact((Thing)null));
                // deprecated ground state
                //this.angleDegrees = 0f;
                //this._sprite.SetAnimation("ground");
            }

            if (ParticleTimer <= 0)
            {
                Level.Add((Thing)TarParticle.New(this.x + Rando.Float(-8f, 8f), this.y + Rando.Float(-8f, 8f)));
                ParticleTimer = 120;
            }
            base.Update();
        }

        [NetworkAction]
        private void GetFlagInstance(Duck duck, float size)
        {
            DrenchedInTar.GetInstance(duck, (int)(125*size), (int)Math.Round(size*3));
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is TarBlast)
            {
                return;
            }

            if (Duck.GetAssociatedDuck(with) != null || with is RagdollPart)
            {
                Duck duck = (Duck)(with is RagdollPart ? ((RagdollPart)with).doll.captureDuck : Duck.GetAssociatedDuck(with));
                
                if (duck.holdObject is TarGun && AliveFor < 3)
                {
                    //#SFX.Play("ting2", 0.25f, Rando.Float(0.2f) - 0.6f);
                    //DevConsole.Log(String.Format("returned ({0})", AliveFor));
                    return;
                }

                HitDuck = true;
                if (isServerForObject)
                {
                    SyncNetworkAction(GetFlagInstance, duck, size);
                }

                this.Destroy((DestroyType)new DTImpact((Thing)null));
            }
            if (with is PhysicsObject)
            {
                Fondle(with);
                with.hSpeed = this.hSpeed / 4f;
                with.vSpeed = this.vSpeed / 4f;
                with.vSpeed -= 0.1f;

                //if (!(with is Gun || with is Equipment))
                //{
                //    this.hSpeed *= 0.99f / Math.Max(with.weight / 3f, 1f);
                //    this.vSpeed *= 0.99f / Math.Max(with.weight / 3f, 1f);
                //}
            }
        }
        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            this.Destroy((DestroyType)new DTImpact((Thing)null));
        }
    }
}
