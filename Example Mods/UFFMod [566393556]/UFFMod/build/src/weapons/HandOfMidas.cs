using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|melee")]
    public class HandOfMidas : Gun, IPlatform // why am I not just extending spear? because I'm lazy
    {
        public StateBinding _targetStateBinding = new StateBinding("_target");
        public StateBinding _currentStateStateBinding = new StateBinding("_currentState");

        public PhysicsObject _target;
        public int _currentState;

        private SpriteMap sprite;

        public HandOfMidas(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // editor name
            _editorName = "Hand of Midas";

            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\handOfMidas"), 36, 16);
            graphic = sprite;
            _center = new Vec2(18f, 8f);
            _barrelOffsetTL = new Vec2(35f, 5f);
            _collisionSize = new Vec2(32f, 4f);
            _collisionOffset = new Vec2(-18f, -2f);
            _holdOffset = new Vec2(2f, 2f);

            // weapon settings
            ammo = 1;
            weight = 5.2f;
            thickness = 1f;
            depth = -0.5f;
            _hasTrigger = false;
            flammable = 0.4f;
            physicsMaterial = PhysicsMaterial.Wood;
        }

        public override void Update()
        {
            base.Update();

            if (_currentState == 1)
            {
                foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(Offset(barrelOffset), 2f))
                    if (Level.CheckLine<Block>(position, physicsObject.position) == null && !(physicsObject is HandOfMidas))
                    {
                        if (physicsObject is Duck || physicsObject is RagdollPart)
                            _target = physicsObject;
                        else if (physicsObject is Gun && !((Gun)physicsObject).infinite)
                            MidasTouch(physicsObject);
                    }

                if (isServerForObject)
                    Level.Add(new WandPixieDust(Offset(barrelOffset).x + Rando.Float(-3f, 3f), Offset(barrelOffset).y + Rando.Float(-3f, 3f)));

                if (_holdOffset.x < 12f && Level.CheckLine<Block>(duck.position, position, this) == null)
                {
                    handOffset.x += 1f;
                    _holdOffset.x += 2f;
                }
                else
                    _currentState = 2;
            }
            else if (_currentState == 2)
            {
                if (isServerForObject && _target != null)
                    MidasTouch(_target);
                _target = null;
                _currentState = 3;
            }
            else if (_currentState == 3)
            {
                if (_holdOffset.x > 2f)
                {
                    handOffset.x -= 0.5f;
                    _holdOffset.x -= 1f;
                }
                else
                    _currentState = 0;
            }
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!(type is DTIncinerate))
                base.OnDestroy(type);
            else
            {
                Level.Remove(this);
                for (int index = 0; index < 8; ++index)
                {
                    Thing t = WoodDebris.New(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                    t.hSpeed = ((Rando.Float(1f) > 0.5f ? 1f : -1f) * Rando.Float(3f));
                    t.vSpeed = -Rando.Float(1f);
                    Level.Add(t);
                }
                return true;
            }
            return false;
        }

        public override void OnPressAction()
        {
            if (duck != null && _currentState == 0)
            {
                SFX.Play("swipe", Rando.Float(0.8f, 1f), Rando.Float(-0.1f, 0.1f), 0f, false);
                _currentState = 1;
            }
        }

        public override void Thrown()
        {
            SFX.Play(Mod.GetPath<UffMod>("SFX\\swish"), 1f, Rando.Float(-0.15f, 0.15f), 0f, false);
            handOffset.x = 0f;
            _holdOffset.x = 2f;
            _currentState = 0;
            base.Thrown();
        }

        private void MidasTouch(PhysicsObject physicsObject)
        {
            Duck hitDuck = physicsObject as Duck;
            RagdollPart ragdollPart = physicsObject as RagdollPart;
            Gun gun = physicsObject as Gun;
            if (hitDuck != null)
            {
                Vec2 goldPos = hitDuck.position;
                hitDuck.Kill(new DTImpact(this));
                Level.Add(new Golduck(goldPos.x, goldPos.y, hitDuck));
                Fondle(hitDuck);
                if (hitDuck.ragdoll != null)
                    Level.Remove(hitDuck.ragdoll);
                Level.Remove(hitDuck);
            }
            else if (ragdollPart != null)
            {
                if (!ragdollPart._doll._duck.dead)
                    ragdollPart._doll._duck.Kill(new DTImpact(this));
                Level.Add(new Golduck(ragdollPart.x, ragdollPart.y, ragdollPart._doll._duck));
                Fondle(ragdollPart._doll);
                Level.Remove(ragdollPart._doll);
            }
            else if (gun != null)
            {
                if (gun.owner == null)
                    Fondle(gun);
                gun.infinite = true;
            }
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner == null)
                return;
            duck.holdObstructed = false;
        }
    }
}
