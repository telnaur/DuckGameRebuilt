using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|explosives")]
    public class ConcussionGrenade : Grenade
    {
        public StateBinding _realTimerStateBinding = new StateBinding("_realTimer");
        public StateBinding _detonationTriggerStateBinding = new StateBinding("_detonationTrigger");
        public StateBinding _frameStateBinding = new StateBinding("spriteFrame");

        public float _realTimer;
        public int _detonationTrigger;

        protected SpriteMap sprite;

        public byte spriteFrame
        {
            get
            {
                if (sprite == null)
                    return (byte)0;
                return (byte)sprite._frame;
            }
            set
            {
                if (sprite == null)
                    return;
                sprite._frame = (int)value;
            }
        }

        public ConcussionGrenade(float xpos, float ypos) :
            base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\concussionGrenade"), 8, 10);
            graphic = sprite;
            center = new Vec2(4f, 5f);
            _holdOffset = new Vec2(-2f, 2f);

            _detonationTrigger = 0;
            _realTimer = 1f;

            _editorName = "Concussion Grenade";
        }

        public override void Update()
        {
            _timer = 99f;
            base.Update();

            PinUpdate();
            
            if (_realTimer <= 0f && _detonationTrigger == 0)
            {
                Shockwave();
                Explosion();

                _detonationTrigger++;
                _destroyed = true;
                Level.Remove(this);
            }

            sprite.frame = _pin ? 0 : 1;
        }

        protected virtual void PinUpdate()
        {
            if (!_pin)
            {
                sprite.frame = 1;
                _realTimer -= 0.01f;
            }
            else
                sprite.frame = 0;
        }

        protected virtual void Explosion()
        {
            // IList<Duck> duckList = new List<Duck>();
            foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(position, 96f))
                if (Level.CheckLine<Block>(position, physicsObject.position, physicsObject) == null)
                {
                    if (physicsObject.owner == null)
                        Fondle(physicsObject);
                    Vec2 propulsion = (physicsObject.position - position).normalized * 6f;
                    physicsObject.hSpeed = propulsion.x;
                    physicsObject.vSpeed = propulsion.y;

                    if (physicsObject is Duck)
                        Level.Add(new StunHandler(physicsObject as Duck, 120, showDaze: true));

                    /*
                    RagdollPart ragdollPart = physicsObject as RagdollPart;
                    if (ragdollPart != null && ragdollPart._doll != null && ragdollPart._doll._duck != null && !duckList.Contains(ragdollPart._doll._duck))
                    {
                        Level.Add(new StunHandler(ragdollPart._doll._duck, 120, showDaze: true));
                        duckList.Add(ragdollPart._doll._duck);
                    }
                    */
                }
        }

        protected virtual void Shockwave()
        {
            if (isServerForObject)
                Level.Add(new GlobalPulse(x, y));
            SFX.Play("explode");
        }

        public override void OnNetworkBulletsFired(Vec2 pos)
        {
            // do nothing
        }
    }
}
