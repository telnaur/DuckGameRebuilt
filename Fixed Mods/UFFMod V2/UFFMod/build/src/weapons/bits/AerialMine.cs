using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.UFFMod
{
    [BaggedProperty("canSpawn", false)]
    internal class AerialMine : PhysicsObject
    {
        public StateBinding _balloonStateBinding = new StateBinding("_balloon");
        public StateBinding _travelTimeStateBinding = new StateBinding("_travelTime");
        public StateBinding netSFX_explodeStateBinding = new NetSoundBinding("netSFX_explode");

        public NetSoundEffect netSFX_explode = new NetSoundEffect(new string[1]
        {
            "explode"
        });

        public Balloon _balloon;
        public int _travelTime;

        private SpriteMap sprite;

        public AerialMine(float xpos, float ypos)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\aerialMine"), 11, 11);
            sprite.AddAnimation("bleep", 0.25f, true, 1, 1, 1, 1, 1, 1, 1, 0);
            sprite.SetAnimation("bleep");
            graphic = sprite;
            center = new Vec2(5f, 5f);
            _collisionSize = new Vec2(11f, 11f);
            _collisionOffset = new Vec2(-5f, -5f);
        }

        public override void Update()
        {
            Block block = Level.CheckCircle<Block>(position, 6f, this);
            Duck duck = Level.CheckCircle<Duck>(position, 6f, this);
            RagdollPart ragdoll = Level.CheckCircle<RagdollPart>(position, 6f, this);
            if (isServerForObject && _travelTime >= 10 && (duck != null || block != null || ragdoll != null || grounded))
                Explode();
            else
            {
                if (_travelTime < 10)
                    _travelTime++;
                base.Update();
            }
        }

        private void Explode()
        {
            netSFX_explode.Play();
            for (int i = 0; i < 6; i++)
                Level.Add(new GlobalExplosion(x + Rando.Float(-32f, 32f), y + Rando.Float(-32f, 32f)));
            foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(position, 48f))
            {
                if (physicsObject == this)
                    continue;
                if (physicsObject.owner == null)
                    Fondle(physicsObject);
                physicsObject.Destroy(new DTImpact(this));
            }
            if (_balloon != null)
            {
                if (_balloon.owner == null)
                    Fondle(_balloon);
                _balloon.Destroy(new DTImpact(this));
            }
            Level.Remove(this);
        }
    }
}
