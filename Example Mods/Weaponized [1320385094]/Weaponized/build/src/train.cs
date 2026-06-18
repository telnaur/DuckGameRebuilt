using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    internal class train : Block
    {
        private Vec2 _travel;

        public Vec2 travel
        {
            get
            {
                return this._travel;
            }
            set
            {
                this._travel = value;
            }
        }
        private bool toRight = false;
        private float timer = 0.6f;
        private float life = 6f;
        private bool sound = false;
        private SpriteMap _sprite;
        private bool dont = false;
        private bool drawn = false;
        public train(float xpos, float ypos, Vec2 travel)
          : base(xpos, ypos)
        {
            this._travel = travel;
            this._sprite = new SpriteMap(GetPath("trainBig"), 568, 46, false);
            this.graphic = (Sprite)this._sprite;
            this._sprite.AddAnimation("idle", 1f, true, 0, 1);
            this._sprite.SetAnimation("idle");
            this.center = new Vec2(284f, 23f);
            this._collisionSize = new Vec2(568f, 46f);
            this._collisionOffset = new Vec2(-284f, -23f);
            this.depth = (Depth)(1f);
            this.layer = Layer.Foreground;
        }

        public override void Update()
        {
            if (sound == false)
            {
                SFX.Play(GetPath("trainSoundCom"), 0.9f, 0.0f, 0.0f, false);
                sound = true;
            }
            train train = this;
            train.position.x += this._travel.x * 3.8f;
            if (toRight == false && timer <= 0f)
            {
                //level.camera.x += 1.4f;
                level.camera.y += 1.4f;
                toRight = true;
                timer = 0.6f;
            }
            else if (timer <= 0f)
            {
                //level.camera.x -= 1.4f;
                level.camera.y -= 1.4f;
                toRight = false;
                timer = 0.6f;
            }
            timer -= 0.1f;
            life -= 0.01f;
            if (life <= 0)
            {
                if (toRight)
                {
                    level.camera.y -= 1.4f;
                }
                Level.Remove((Thing)this);
            }
            foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(this.topLeft + new Vec2(-10f, -1f), this.topLeft + new Vec2(0, 1f)))
            {
                physicsObject.vSpeed = -0.01f;
            }
            foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(this.topRight + new Vec2(0, -1f), this.topRight + new Vec2(10f, 1f)))
            {
                physicsObject.vSpeed = -0.01f;
            }
            foreach (Duck duck in Level.CheckRectAll<Duck>(this.topLeft + new Vec2(-10f, -1f), this.topLeft + new Vec2(0, 1f)))
            {
                duck.vSpeed = -0.01f;
            }
            foreach (Duck duck in Level.CheckRectAll<Duck>(this.topRight + new Vec2(0, -1f), this.topRight + new Vec2(10f, 1f)))
            {
                duck.vSpeed = -0.01f;
            }
            foreach (Duck duck in Level.CheckRectAll<Duck>(this.topLeft + new Vec2(1f, 0f), this.topRight + new Vec2(-1f, 0f)))
            {
                foreach (Block block in Level.CheckRectAll<Block>(duck.topLeft + new Vec2(2f, 5f), duck.topRight + new Vec2(-2f, 5f)))
                {
                    dont = true;
                    if(duck._trapped == null)
                    duck.GoRagdoll();
                }   
            }
            foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(this.topLeft + new Vec2(1f, 0f), this.topRight + new Vec2(-1f, 0f)))
            {
                Vec2 vec2 = new Vec2(physicsObject.center.x, this.y - 38f);
                foreach (Block block in Level.CheckRectAll<Block>(physicsObject.topLeft + new Vec2(2f, 0f), physicsObject.topRight + new Vec2(-2f, 0f)))
                {
                    dont = true;
                }
                if (dont == false && ((physicsObject is RagdollPart) || (physicsObject is Duck) || (physicsObject is TrappedDuck)))
                {
                    physicsObject.position += this._travel * 3.8f;
                    physicsObject.vSpeed = -0.03f;
                }
                else if (dont == false)
                {
                    physicsObject.sleeping = false;
                    physicsObject.vSpeed = -2f;
                }
                dont = false;
            }
            foreach (RagdollPart ragdoll in Level.CheckRectAll<RagdollPart>(this.topLeft + new Vec2(-1f, 8f), this.bottomRight + new Vec2(1f, 1f)))
            {
                ragdoll.Destroy((DestroyType)new DTImpact((Thing)this));
                if (_travel.x > 0)
                    ragdoll.hSpeed = 10f;
                else
                    ragdoll.hSpeed = -10f;
            }
            foreach (Duck duck in Level.CheckRectAll<Duck>(this.topLeft + new Vec2(-1f, 8f), this.bottomRight + new Vec2(1f, 1f)))
            {
                if (_travel.x > 0)
                    duck.hSpeed = 10f;
                else
                    duck.hSpeed = -10f;
                duck.Destroy((DestroyType)new DTImpact((Thing)this));
            }
            base.Update();
        }
        public override void Draw()
        {
            if (_travel.x > 0 && this.drawn == false)
            {
                this.drawn = true;
                this.graphic.flipH = true;
            }
            base.Draw();
        }
    }
}

