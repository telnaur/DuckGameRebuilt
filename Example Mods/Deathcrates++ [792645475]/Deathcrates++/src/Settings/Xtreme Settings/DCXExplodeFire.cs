using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class DCXExplodeFire : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            float cx = c.x;
            float cy = c.y - 2f;

            IEnumerable<Ragdoll> ragdolledDucks = Level.CheckCircleAll<Ragdoll>(new Vec2(cx, cy), 2048);
            foreach(Ragdoll duck in ragdolledDucks)
            {
                duck.Unragdoll();
            }

            IEnumerable<Duck> allDucks = Level.CheckCircleAll<Duck>(new Vec2(cx, cy), 2048);

            Level.Add(new ExplosionPart(cx, cy, true));
            int num = 6;
            if (Graphics.effectsLevel < 2)
            {
                num = 3;
            }
            for (int i = 0; i < num; i++)
            {
                float dir = (float)i * 60f + Rando.Float(-10f, 10f);
                float dist = Rando.Float(12f, 20f);
                ExplosionPart ins = new ExplosionPart(cx + (float)(System.Math.Cos((double)Maths.DegToRad(dir)) * (double)dist), cy - (float)(System.Math.Sin((double)Maths.DegToRad(dir)) * (double)dist), true);
                Level.Add(ins);
            }

            IEnumerable<TrappedDuck> nettedDucks = Level.CheckCircleAll<TrappedDuck>(new Vec2(cx, cy), 2048);
            foreach(TrappedDuck duck in nettedDucks)
            {
                FireExtinguisher fe = new FireExtinguisher(duck.position.x, duck.position.y);
                Level.Add(fe);
            }
            foreach (Duck duck in allDucks)
            {
                FireExtinguisher fe = new FireExtinguisher(duck.position.x, duck.position.y);
                Level.Add(fe);
                duck.GiveHoldable(fe);
            }

            if (server)
            {
                for (int j = 0; j < 16; j++)
                {
                    Level.Add(SmallFire.New(c.x - 6f + Rando.Float(12f), c.y - 8f + Rando.Float(4f), -6f + Rando.Float(12f), 2f - Rando.Float(8.5f), false, null, true, c, false));
                }
                Level.Add(new Burner(20, cx, cy));
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode", 1f, 0f, 0f, false);
        }
    }

    class Burner : Thing
    {
        int time;
        Vec2 pos;
        public Burner(int _time, float xpos, float ypos)
        {
            time = _time;

            pos = new Vec2(xpos, ypos);
        }

        void BurnDucks()
        {
            foreach(PhysicsObject duck in GetDucks())
            {
                duck.Burn(duck.position, this);
            }

            foreach(Ragdoll ragduck in Level.CheckCircleAll<Ragdoll>(new Vec2(pos), 2048))
            {
                ragduck.LitOnFire(this);
            }
        }

        List<PhysicsObject> GetDucks()
        {
            return Level.CheckCircleAll<TrappedDuck>(pos, 2048).Concat<PhysicsObject>(Level.CheckCircleAll<Duck>(pos, 2048)).ToList();
        }

        public override void Update()
        {
            time--;
            if(time <= 0)
            {
                BurnDucks();
				Level.Remove(this);
            }
            base.Update();
        }
    }
}