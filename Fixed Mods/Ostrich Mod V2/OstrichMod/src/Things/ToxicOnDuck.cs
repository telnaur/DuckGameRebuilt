using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.OstrichMod
{
    public class ToxicOnDuck : Thing
    {
        public static Dictionary<Duck, bool> ToxicDucks = new Dictionary<Duck, bool>();
        public float Timer
        {
            get;
            set;
        }
        Duck duck;

        public ToxicOnDuck(Duck duck, float stayTime = 0.15f) : base(0f, 0f)
        {
            this.duck = duck;
            Timer = stayTime;

            ToxicDucks[duck] = true;
        }

        public override void Update()
        {
            base.Update();

            if(duck.dead)
            {
                Level.Remove(this);
            }
            else
            {

                if(Timer > 0)
                {
                    Timer -= 0.01f;
                }
                else
                {
                    KillDuck();
                }

            }
        }

        public virtual void KillDuck()
        {
            if(!duck.dead)
            {
                duck.Scream();
                duck.Kill(new DTToxic(this));
            }
        }
    }
}
