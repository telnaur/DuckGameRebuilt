using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    public class BlackHole : Thing, ITeleport
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _theGunStateBinding = new StateBinding("_theGun");
        public StateBinding _theCubeStateBinding = new StateBinding("_theCube");
        public StateBinding _fireSpeedStateBinding = new CompressedFloatBinding("_fireSpeed");
        public StateBinding _fireAngleStateBinding = new CompressedFloatBinding("_fireAngle");
        public StateBinding _releasedStateBinding = new StateBinding("_released");
        public StateBinding _vanishStateBinding = new StateBinding("_vanish");
        public StateBinding _timerStateBinding = new StateBinding("_timer");

        public GravityGun _theGun;
        public DarkMatterCube _theCube;
        public float _fireSpeed;
        public float _fireAngle;
        public bool _released;
        public bool _vanish;
        public int _timer;

        // uncomment marked sections to re-enable graviton particles
        // private IList<PhysicsObject> hits = new List<PhysicsObject>();
        private Sound buzz;
        private SpriteMap sprite;
        private SpriteMap suction;
        private bool increasing;
        private int rotationDirection;

        public BlackHole(float xpos, float ypos, GravityGun gGun = null, DarkMatterCube dmc = null, int timer = -1)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\blackHole"), 64, 64);
            suction = new SpriteMap(Mod.GetPath<UffMod>("weapons\\blackSuction"), 72, 72);
            suction.xscale = suction.yscale = 0.5f;
            suction.CenterOrigin();
            graphic = sprite;
            center = new Vec2(32f, 32f);
            xscale = yscale = 0f;
            _released = false;
            increasing = true;
            depth = 1f;
            rotationDirection = Rando.Int(1) == 1 ? 1 : -1;
            _theCube = dmc;
            _theGun = gGun;
            _timer = timer;
        }

        public override void Initialize()
        {
            buzz = SFX.Play(Mod.GetPath<UffMod>("SFX\\noiz"), looped: true);
            base.Initialize();
        }

        public override void Terminate()
        {
            buzz.Stop();
            base.Terminate();
        }
        
        public override void Update()
        {
            if (_vanish || (isServerForObject && _theGun != null && _theGun._currentState == 0 && !_released))
            {
                Level.Remove(this);
                return;
            }
            
            if (_released)
            {
                if (x > Level.current.bottomRight.x + 200 || x < Level.current.topLeft.x - 200 || y > Level.current.bottomRight.y + 200 || y < Level.current.topLeft.y - 200)
                    Level.Remove(this);

                x += _fireSpeed * (float)Math.Cos((Math.PI * _fireAngle) / 180);
                y += _fireSpeed * (float)Math.Sin((Math.PI * _fireAngle) / 180);

                foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(position, 240f))
                {
                    float distance = (physicsObject.position - position).length;
                    if (!(physicsObject is IForcedMovementImmunity) && !(physicsObject is Duck && ((Duck)physicsObject).HasEquipment(typeof(IForcedMovementImmunity))) && physicsObject.active && physicsObject.visible)
                    {
                        Duck d = physicsObject as Duck;
                        if (d != null)
                        {
                            d.crouch = false;
                            d.sliding = false;
                        }
                        if (physicsObject is Grenade)
                            ((Grenade)physicsObject).PressAction();
                        // uncomment this, the field above, and the section below to enable graviton particles
                        // bear in mind that their code has changed since then to work with the dark matter cube
                        /*if (!hits.Contains(physicsObject))
                            hits.Add(physicsObject);*/
                        float angleToObject = (float)Math.Atan2(physicsObject.y - y, physicsObject.x - x);
                        physicsObject.hSpeed -= (4f * xscale / (float)Math.Sqrt(distance / 2f)) * (float)Math.Cos(angleToObject);
                        physicsObject.vSpeed -= (4f * xscale / (float)Math.Sqrt(distance / 2f)) * (float)Math.Sin(angleToObject);
                    }
                    if (distance < 16f)
                        physicsObject.Destroy(new DTImpact(this));
                }

                // uncomment to enable graviton particles
                /*IList<PhysicsObject> removeList = new List<PhysicsObject>();
                foreach (PhysicsObject physicsObject in hits)
                    if (!physicsObject.destroyed && (physicsObject.position - position).length <= 240f)
                    {
                        if (isServerForObject && Rando.Int(49) == 0)
                            Level.Add(new Graviton(physicsObject.x, physicsObject.y, this));
                    }
                    else
                        removeList.Add(physicsObject);
                foreach (PhysicsObject physicsObject in removeList)
                    hits.Remove(physicsObject);*/

                if (_timer != 0)
                {
                    if (increasing)
                    {
                        if (xscale < 0.58f)
                            xscale = yscale = MathHelper.Lerp(xscale, 0.6f, 0.06f);
                        else
                        {
                            xscale = yscale = MathHelper.Lerp(xscale, 0.5f, 0.06f);
                            increasing = false;
                        }
                    }
                    else
                    {
                        if (xscale > 0.52f)
                            xscale = yscale = MathHelper.Lerp(xscale, 0.5f, 0.06f);
                        else
                        {
                            xscale = yscale = MathHelper.Lerp(xscale, 0.6f, 0.06f);
                            increasing = true;
                        }
                    }
                    Suction(0.5f);
                }
                else
                    Suction();
                

                if (_timer > 0)
                    _timer--;
                else if (_timer == 0)
                {
                    if (xscale > 0.02f)
                        xscale = yscale = MathHelper.Lerp(xscale, 0f, 0.06f);
                    else
                        Level.Remove(this);
                }
            }
            else
                Suction();

            suction.alpha = 1f - ((1f/2f) * suction.xscale) > 1f ? 1f : 1f - ((1f/2f) * suction.xscale);
            suction.CenterOrigin();
            sprite.CenterOrigin();

            base.Update();
        }

        private void Suction(float modifier = 0f)
        {
            if (suction.xscale > 0.02f + modifier)
                suction.xscale = suction.yscale = MathHelper.Lerp(suction.xscale, modifier, 0.16f);
            else
                suction.xscale = suction.yscale = 0.5f + modifier;
        }

        public override void Draw()
        {
            Graphics.Draw((Sprite)suction, x, y, 0.9f);
            base.Draw();
        }
    }
}
