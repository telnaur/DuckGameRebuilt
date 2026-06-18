using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.UFFMod
{
    // partially based off decompiled grenade code

    [EditorGroup("uff|stuff|props")]
    public class TNT : Crate
    {
        private SpriteMap sprite;
        private bool exploded;
        
        public TNT(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "TNT";

            // sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\TNT"), 16, 16);
            graphic = sprite;

            // misc. settings
            _maxHealth = 1f;
            _hitPoints = 1f;
        }

        public override void Update()
        {
            if (!exploded && (destroyed || _hitPoints <= 0))
                Explode();

            base.Update();
        }

        private void Explode()
        {
            if (exploded)
                return;

            exploded = true;

            Level.Add(new ExplosionPart(x, y));
            for (int i = 0; i < 8; i++)
            {
                float deg = i * 45f + Rando.Float(-10f, 10f);
                float displacement = Rando.Float(18f, 36f);
                Level.Add(new ExplosionPart(x + (float)Math.Cos((double)Maths.DegToRad(deg)) * displacement, y - (float)Math.Sin((double)Maths.DegToRad(deg)) * displacement));
            }
            SFX.Play("explode");

            Level.Remove(this);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if(bullet.isLocal && owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);

            return base.Hit(bullet, hitPos);
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            _destroyed = true;

            foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(position, 52f))
            {
                if(physicsObject.owner == null)
                    Fondle(physicsObject);
                if (!physicsObject.destroyed && physicsObject != this && (Level.CheckLine<Block>(position, physicsObject.position, physicsObject) == null))
                {
                    if ((physicsObject.position - position).length <= 40f)
                        physicsObject.Destroy(new DTImpact(this));
                    else
                    {
                        physicsObject.sleeping = false;
                        physicsObject.vSpeed -= 2f;
                    }
                }
            }

            Explode();

            return true;
        }
    }
}
