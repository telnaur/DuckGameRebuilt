using System;
using System.Linq;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|melee")]
    public class Spear : Gun, IPlatform
    {
        public StateBinding _groundedStateBinding = new StateBinding("_grounded");
        public StateBinding gravMultiplierStateBinding = new StateBinding("gravMultiplier");
        public StateBinding _leakedStateBinding = new StateBinding("_leaked");
        public StateBinding _currentStateStateBinding = new StateBinding("_currentState");
        public StateBinding _savedDuckStateBinding = new StateBinding("_savedDuck");

        public float _leaked;
        public int _currentState;
        public Duck _savedDuck;

        public EditorProperty<bool> ice = new EditorProperty<bool>(false, null, 0f, 1f, 1f, null, false, false);

        private SpriteMap normalSprite;
        private SpriteMap iceSprite;
        private SpriteMap sprite;

        public Spear(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            normalSprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\spear"), 31, 7);
            iceSprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\spearIce"), 31, 7);
            sprite = normalSprite;
            graphic = sprite;
            _center = new Vec2(16f, 4f);
            _barrelOffsetTL = new Vec2(30f, 3f);
            _collisionSize = new Vec2(26f, 4f);
            _collisionOffset = new Vec2(-15f, -2f);
            _holdOffset = new Vec2(2f, 2f);

            // weapon settings
            ammo = 1;
            weight = 5.2f;
            thickness = 1f;
            depth = -0.5f;
            _hasTrigger = false;
            physicsMaterial = PhysicsMaterial.Wood;

            // defaults
            _leaked = 0f;
            _currentState = 0;
        }

        public override void Draw()
        {
            if (ice)
                sprite = iceSprite;
            else
                sprite = normalSprite;
            graphic = sprite;
            base.Draw();
        }

        public override void Update()
        {
            if (duck != null)
                _savedDuck = duck;

            base.Update();
            
            if (ice)
            {
                physicsMaterial = PhysicsMaterial.Default;
                if (heat >= 0.3f)
                {
                    heat = MathHelper.Lerp(heat, 0f, 0.08f);
                    if (_leaked % 0.04 >= 0.03)
                    {
                        FluidStream waterStream;
                        waterStream = new FluidStream(Rando.Float(x - 8f, x + 8f), y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)), 4f, new Vec2());
                        FluidData data = Fluid.Water;
                        data.amount = 0.01f;
                        waterStream.Feed(data);
                    }
                    _leaked += 0.01f;
                }
                if (_leaked >= 1f)
                    Level.Remove(this);
            }                  
            else
            {
                sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\spear"), 31, 7);
                flammable = 0.4f;
            }

            if (_currentState == -1)
            {
                if (duck != null || velocity.length > 0.6f)
                {
                    _currentState = 0;
                    gravMultiplier = 1f;
                }
                else
                {
                    _vSpeed = 0f;
                    _hSpeed = 0f;
                    gravMultiplier = 0f;
                    grounded = true;
                }
            }
            if (_currentState == 1)
            {
                foreach (Duck d in Level.CheckCircleAll<Duck>(Offset(barrelOffset), 2f))
                    if (Level.CheckLine<Block>(position, d.position, d) == null)
                        d.Kill(new DTImpale(null));
                foreach (Ragdoll r in Level.CheckCircleAll<Ragdoll>(Offset(barrelOffset), 6f))
                    if (Level.CheckLine<Block>(position, r._duck.position, r._duck) == null)
                        r._duck.Kill(new DTImpale(null));

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

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnSolidImpact(with, from);
            if (_currentState != -1 
                && !(with is Gun) 
                && with.weight >= 5f 
                && !(with is Duck) 
                && !ice 
                && !(with is Door) 
                && !(with is VerticalDoor) 
                && with is Block 
                && from == (offDir > 0 ? ImpactedFrom.Right : ImpactedFrom.Left))
            {
                _hSpeed = 0f;
                _vSpeed = 0f;
                gravMultiplier = 0f;
                grounded = true;
                _currentState = -1;
            }
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnImpact(with, from);
            if (_currentState != -1
                && impactPowerH >= 1f
                && (with is Duck || with is RagdollPart)
                && with != _savedDuck
                && ((offDir > 0 && from == ImpactedFrom.Right)
                || (offDir < 0 && from == ImpactedFrom.Left)))
            {
                if(with is Duck)
                    ((Duck)with).Kill(new DTImpale(with));
                else if (with is RagdollPart)
                    ((RagdollPart)with)._doll._duck.Kill(new DTImpale(with));
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
