using System.Collections.Generic;
using System.Linq;
using System;

namespace DuckGame.UFFMod
{
    // base hittable box class, based off decompiled ItemBox

    public class BaseBox : Block, IPathNodeBlocker
    {
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _boxStateBinding = new StateBinding("_hit");
        public StateBinding _chargingBinding = new StateBinding("charging", 9);
        public StateBinding _netDisarmIndexBinding = new StateBinding("netDisarmIndex");
        public StateBinding _netHitSoundBinding = (StateBinding)new NetSoundBinding("_netHitSound");

        public NetSoundEffect _netHitSound = new NetSoundEffect(new string[1]
        {
          "hitBox"
        })
        {
            volume = 1f
        };

        public EditorProperty<int> maxUses = new EditorProperty<int>(-1, null, -1f, 100f, 1f, "INF");
        public EditorProperty<int> rechargeTime = new EditorProperty<int>(500, null, 0f, 3000f, 10f);

        public float startY = -99999f;
        protected List<PhysicsObject> _aboveList = new List<PhysicsObject>();
        public bool _canBounce = true;
        public byte netDisarmIndex;
        public byte localNetDisarm;
        public float bounceAmount;
        public bool _hit;
        public int charging;
        protected int timesUsed;
        protected SpriteMap sprite;

        public bool canBounce
        {
            get
            {
                return _canBounce;
            }
        }

        public BaseBox(float xpos, float ypos)
            : base(xpos, ypos)
        {
            graphic = null;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            depth = 0.5f;
            timesUsed = 0;
            _canFlip = false;
        }

        public void Pop(MaterialThing with)
        {
            Bounce(with);
            if (!_hit && (timesUsed < maxUses || maxUses == -1f))
                Activate(with);
        }

        public void Bounce(MaterialThing with)
        {
            if (!_canBounce)
                return;
            bounceAmount = 8f;
            _canBounce = false;
            if (Network.isActive)
                ++netDisarmIndex;
            else
            {
                _aboveList = Enumerable.ToList<PhysicsObject>(Level.CheckRectAll<PhysicsObject>(this.topLeft + new Vec2(1f, -4f), this.bottomRight + new Vec2(-1f, -12f)));
                foreach (PhysicsObject physicsObject in this._aboveList)
                {
                    if (physicsObject.grounded || physicsObject.vSpeed >= 0f)
                    {
                        Fondle(physicsObject);
                        physicsObject.y -= 2f;
                        physicsObject.vSpeed = -3f;
                        Duck duck = physicsObject as Duck;
                        if (duck != null)
                            duck.Disarm(this);
                    }
                }
            }
        }

        public virtual void Activate(MaterialThing with)
        {
            timesUsed++;
            _netHitSound.Play(1f, 0f);
            charging = rechargeTime;
            _hit = true;
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (from != ImpactedFrom.Bottom || !with.isServerForObject)
                return;
            with.Fondle(this);
            Pop(with);
        }

        public virtual void UpdateCharging()
        {
            if (!isServerForObject || (maxUses != -1 && timesUsed == maxUses))
                return;
            if (charging > 0)
                --charging;
            else
            {
                charging = 0;
                _hit = false;
            }
        }

        public override void Update()
        {
            _aboveList.Clear();
            if (startY < -9999.0)
                startY = y;
            sprite.frame = (_hit || timesUsed == maxUses) ? 1 : 0;
            if (netDisarmIndex != localNetDisarm)
            {
                localNetDisarm = netDisarmIndex;
                _aboveList = Enumerable.ToList<PhysicsObject>(Level.CheckRectAll<PhysicsObject>(topLeft + new Vec2(1f, -4f), bottomRight + new Vec2(-1f, -12f)));
                foreach (PhysicsObject physicsObject in _aboveList)
                {
                    if (isServerForObject && physicsObject.owner == null)
                        Fondle(physicsObject);
                    if (physicsObject.isServerForObject && (physicsObject.grounded || physicsObject.vSpeed >= 0f))
                    {
                        physicsObject.y -= 2f;
                        physicsObject.vSpeed = -3f;
                        Duck duck = physicsObject as Duck;
                        if (duck != null)
                            duck.Disarm(this);
                    }
                }
            }
            UpdateCharging();
            if (bounceAmount > 0.0f)
                bounceAmount -= 0.8f;
            else
                bounceAmount = 0.0f;
            y -= bounceAmount;
            if (_canBounce)
                return;
            if (y < startY)
                y += (0.8f + (Math.Abs(y - startY) * 0.4f));
            if (y > startY)
                y -= (0.8f - (Math.Abs(y - startY) * 0.4f));
            if (Math.Abs(y - startY) >= 0.8)
                return;
            _canBounce = true;
            y = startY;
        }
    }
}
