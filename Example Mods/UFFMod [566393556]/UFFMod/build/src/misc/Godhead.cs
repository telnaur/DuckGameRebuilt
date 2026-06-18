using System.Linq;

namespace DuckGame.UFFMod
{
    public class Godhead : Thing
    {
        public StateBinding _theDuckStateBinding = new StateBinding("_theDuck");

        public Duck _theDuck;

        public Godhead(Duck theDuck)
        {
            _theDuck = theDuck;
        }

        public override void Terminate()
        {
            if (_theDuck != null)
            {
                _theDuck.flammable = 1f;
                _theDuck.gravMultiplier = 1f;
                _theDuck.immobilized = false;
                _theDuck.remoteControl = false;
            }

            base.Terminate();
        }

        public override void Update()
        {
            if (_theDuck != null)
            {
                // floatiness && permanent crouch
                if (_theDuck.inputProfile.Down(Triggers.LeftTrigger))
                {
                    _theDuck.hSpeed = MathHelper.Lerp(_theDuck.hSpeed, 0f, 0.08f);
                    _theDuck.vSpeed = MathHelper.Lerp(_theDuck.vSpeed, 0f, 0.08f);
                    _theDuck.gravMultiplier = 0f;
                    _theDuck.crouch = true;
                    _theDuck.immobilized = true;
                    _theDuck.remoteControl = true;
                }
                else
                {
                    _theDuck.gravMultiplier = 1f;
                    _theDuck.immobilized = false;
                    _theDuck.remoteControl = false;
                }

                // flight
                if (_theDuck.inputProfile.Down(Triggers.Left))
                    _theDuck.hSpeed = MathHelper.Lerp(_theDuck.hSpeed, -8f, 0.12f);
                if (_theDuck.inputProfile.Down(Triggers.Right))
                    _theDuck.hSpeed = MathHelper.Lerp(_theDuck.hSpeed, 8f, 0.12f);
                if (_theDuck.inputProfile.Down(Triggers.Up))
                    _theDuck.vSpeed = MathHelper.Lerp(_theDuck.vSpeed, -8f, 0.12f);
                if (_theDuck.inputProfile.Down(Triggers.Down))
                    _theDuck.vSpeed = MathHelper.Lerp(_theDuck.vSpeed, 8f, 0.12f);

                // fire protection
                if (_theDuck._equipment.Count > 0)
                    foreach (Equipment e in _theDuck._equipment)
                        if (e.heat > 0f)
                            e.heat = 0f;

                if (_theDuck.holdObject != null && _theDuck.holdObject.heat > 0f)
                    _theDuck.holdObject.heat = 0f;

                _theDuck.flammable = 0f;
                _theDuck.onFire = false;

                // immortality
                if (_theDuck.dead)
                    _theDuck.Ressurect();
            }
        }
    }
}
