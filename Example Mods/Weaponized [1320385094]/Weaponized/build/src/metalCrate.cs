using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Stuff|Props")]
    [BaggedProperty("isInDemo", true)]
    public class metalCrate : Holdable, IPlatform
    {
        public StateBinding _destroyedBinding = new StateBinding("_destroyed", -1, false, false);
        public StateBinding _hitPointsBinding = new StateBinding("_hitPoints", -1, false, false);
        public StateBinding _damageMultiplierBinding = new StateBinding("damageMultiplier", -1, false, false);
        public float damageMultiplier = 1f;
        private SpriteMap _sprite;
        private float _burnt;

        public metalCrate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._maxHealth = 45f;
            this._hitPoints = 45f;
            this._sprite = new SpriteMap(GetPath("metalCrate"), 16, 16, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.thickness = 4f;
            this.weight = 8f;
            this.buoyancy = 0f;
            this.depth = -0.5f;
            this._holdOffset = new Vec2(2f, 0.0f);
            this.flammable = 0f;
            this.collideSounds.Add("metalRebound");
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._editorName = "Metal Crate";
            this.editorTooltip = "After millions of broken crates the company decided to make them a little more durable.";
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            this._hitPoints = 0.0f;
            Level.Remove((Thing)this);
            SFX.Play("crateDestroy", 1f, 0.0f, 0.0f, false);
            Vec2 vec2 = Vec2.Zero;
            if (type is DTShot)
                vec2 = (type as DTShot).bullet.travelDirNormalized;
            for (int index = 0; index < 6; ++index)
            {
                Thing thing = (Thing)metalDebris.New(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                thing.hSpeed = (float)(((double)Rando.Float(1f) > 0.5 ? 1.0 : -1.0) * (double)Rando.Float(3f) + (double)Math.Sign(vec2.x) * 0.5);
                thing.vSpeed = -Rando.Float(1f);
                Level.Add(thing);
            }
            for (int index = 0; index < 5; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-6f, 6f), this.y + Rando.Float(-6f, 6f));
                smallSmoke.hSpeed += (float)(double)Rando.Float(-0.3f, 0.3f);
                smallSmoke.vSpeed -= (float)(double)Rando.Float(0.1f, 0.2f);
                Level.Add((Thing)smallSmoke);
            }
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if ((double)this._hitPoints <= 0.0)
                return base.Hit(bullet, hitPos);
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            for (int index = 0; (double)index < 1.0 + (double)this.damageMultiplier / 2.0; ++index)
            {
                Thing thing = (Thing)metalDebris.New(hitPos.x, hitPos.y);
                thing.hSpeed = (float)(-(double)bullet.travelDirNormalized.x * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                thing.vSpeed = (float)(-(double)bullet.travelDirNormalized.y * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929)) - Rando.Float(2f);
                Level.Add(thing);
            }
            SFX.Play("metalRebound", 1f, 0.0f, 0.0f, false);
            if (bullet.isLocal && TeamSelect2.Enabled("EXPLODEYCRATES", false))
            {
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
                if (this.duck != null)
                    this.duck.ThrowItem(true);
                this.Destroy((DestroyType)new DTShot(bullet));
                Level.Add((Thing)new GrenadeExplosion(this.x, this.y));
            }
            if (bullet.isLocal)
            {
                this._hitPoints -= (float)(double)this.damageMultiplier;
                this.damageMultiplier += 2f;
            }
            if ((double)this._hitPoints <= 0.0)
                this.Destroy((DestroyType)new DTShot(bullet));
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            for (int index = 0; (double)index < 1.0 + (double)this.damageMultiplier / 2.0; ++index)
            {
                Thing thing = (Thing)metalDebris.New(exitPos.x, exitPos.y);
                thing.hSpeed = (float)((double)bullet.travelDirNormalized.x * 3.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                thing.vSpeed = (float)((double)bullet.travelDirNormalized.y * 3.0 * ((double)Rando.Float(1f) + 0.300000011920929) - ((double)Rando.Float(2f) - 1.0));
                Level.Add(thing);
            }
        }

        public override void Update()
        {
            base.Update();
            if ((double)this.damageMultiplier > 1.0)
                this.damageMultiplier -= 0.2f;
            else
                this.damageMultiplier = 1f;
            this._sprite.frame = (int)Math.Floor((1.0 - (double)this._hitPoints / (double)this._maxHealth) * 4.0);
            if ((double)this._hitPoints <= 0.0 && !this._destroyed)
                this.Destroy((DestroyType)new DTImpact((Thing)this));
            if (this._onFire && (double)this._burnt < 0.899999976158142)
            {
                float num = 1f - this.burnt;
                if ((double)this._hitPoints > (double)num * (double)this._maxHealth)
                    this._hitPoints = num * this._maxHealth;
                this._sprite.color = new Color(num, num, num);
            }

        }
    }
}