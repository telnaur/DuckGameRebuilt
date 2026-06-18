using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class potionEffect : Thing
    {
        public Duck duck;

        public potionEffect(Duck duck)
          : base(0.0f, 0.0f, (Sprite)null)
        {
            this.duck = duck;
        }

        public override void Update()
        {
            if (duck != null)
            {
                if (!this.duck.dead)
                {
                    if (isServerForObject)
                    {
                        if (!duck.inputProfile.Down("DOWN"))
                        {
                            duck.sliding = false;
                            duck.crouch = false;
                        }
                        else if ((duck.inputProfile.Down("JUMP") && duck.grounded) && (this.duck.sliding || this.duck.crouch))
                        {
                            Equipment equipment = duck.GetEquipment(typeof(Jetpack));
                            if (equipment == null)
                            {
                                duck.sliding = false;
                                duck.crouch = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
