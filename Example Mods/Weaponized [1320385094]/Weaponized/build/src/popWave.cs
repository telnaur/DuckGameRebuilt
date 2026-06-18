using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class popWave : Thing
    {
        private List<Thing> _hits = new List<Thing>();
        private float _alphaSub;
        private float _speed;
        private float _speedv;

        public popWave(float xpos, float ypos, int dir, float alphaSub, float speed, float speedv, Duck own)
          : base(xpos, ypos, (Sprite)null)
        {
            this.offDir = (sbyte)dir;
            this.graphic = new Sprite(GetPath("popForce"), 0.0f, 0.0f);
            this.center = new Vec2((float)this.graphic.w, (float)this.graphic.h);
            this._alphaSub = alphaSub;
            this._speed = speed;
            this._speedv = speedv;
            this._collisionSize = new Vec2(6f, 30f);
            this._collisionOffset = new Vec2(-3f, -15f);
            this.graphic.flipH = (int)this.offDir <= 0;
            this.owner = (Thing)own;
        }

        public override void Update()
        {
            if ((double)this.alpha > 0.100000001490116)
            {
                foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(this.topLeft, this.bottomRight))
                {
                    if (!this._hits.Contains((Thing)physicsObject) && physicsObject != this.owner)
                    {
                        if (this.owner != null)
                            Thing.Fondle((Thing)physicsObject, this.owner.connection);
                        Grenade grenade = physicsObject as Grenade;
                        if (grenade != null)
                            grenade.PressAction();
                        physicsObject.hSpeed = (float)(((double)this._speed - 3.0) * (double)this.offDir * 1.5 + (double)this.offDir * 4.0) * this.alpha;
                        physicsObject.vSpeed = (this._speedv - 4.5f) * this.alpha;
                        physicsObject.clip.Add(this.owner as MaterialThing);
                        if (!physicsObject.destroyed)
                            physicsObject.Destroy((DestroyType)new DTImpact((Thing)this));
                        this._hits.Add((Thing)physicsObject);
                    }
                }
                foreach (Door door in Level.CheckRectAll<Door>(this.topLeft, this.bottomRight))
                {
                    if (this.owner != null)
                        Thing.Fondle((Thing)door, this.owner.connection);
                    if (!door.destroyed)
                        door.Destroy((DestroyType)new DTImpact((Thing)this));
                }
                foreach (Window window in Level.CheckRectAll<Window>(this.topLeft, this.bottomRight))
                {
                    if (this.owner != null)
                        Thing.Fondle((Thing)window, this.owner.connection);
                    if (!window.destroyed)
                        window.Destroy((DestroyType)new DTImpact((Thing)this));
                }
            }
            this.x += (float)((double)this.offDir * (double)this._speed);
            this.y += (float)(double)this._speedv;
            this.alpha -= (float)(double)this._alphaSub;
            if ((double)this.alpha > 0.0)
                return;
            Level.Remove((Thing)this);
        }
    }
}