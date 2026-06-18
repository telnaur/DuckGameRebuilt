using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.OstrichMod
{
    public class ToxicSmoke : Thing
    {
        public float Timer {
            get;
            set;
        }

        public bool inflamable = true;
        Vec2 move;
        float angleIncrement;
        float scaleDecrement;
        float fastGrowTimer;


        public ToxicSmoke(float xval, float yval, float stayTime = 1f) : base(xval, yval)
        {
            velocity = new Vec2(Rando.Float(-5, 5), Rando.Float(-5, 5));
            xscale = Rando.Float(0.15f, 0.30f);
            yscale = xscale;
            angle = Maths.DegToRad(Rando.Float(360f));
            fastGrowTimer = Rando.Float(0.6f, 0.9f);
            Timer = stayTime;
            angleIncrement = Maths.DegToRad(Rando.Float(2f) - 1f);
            scaleDecrement = Rando.Float(0.001f, 0.002f);

            GraphicList graphicList = new GraphicList();

            Sprite graphic1 = new Sprite("smoke", 0.0f, 0.0f);
            graphic1.depth = (Depth)1f;
            graphic1.CenterOrigin();
            graphic1.color = Color.DarkOliveGreen;
            graphic1.alpha = 0.5f;
            graphicList.Add(graphic1);

            Sprite graphic2 = new Sprite("smokeBack", 0.0f, 0.0f);
            graphic2.depth = (Depth)0.1f;
            graphic2.CenterOrigin();
            graphic2.color = Color.DarkOliveGreen;
            graphic2.alpha = 0.5f;
            graphicList.Add(graphic2);

            graphic = graphicList;

            center = new Vec2(0.0f, 0.0f);
            depth = (Depth)1f;
        }

        public override void Update()
        {

            angle += angleIncrement;

            if(Timer > 0)
            {
                Timer -= 0.01f;
            }
            else
            {
                xscale -= scaleDecrement;
                scaleDecrement += 0.0001f;
            }

            if(fastGrowTimer > 0)
            {
                fastGrowTimer -= 0.05f;
                xscale += 0.05f;
            }

            yscale = xscale;

            velocity *= new Vec2(0.9f, 0.9f);

            if(xscale < 0.100000001490116)
            {
                Level.Remove(this);
            }

            ToxicOnDucks();
        }

        public override void Draw()
        {
            base.Draw();
        }

        public virtual void ToxicOnDucks()
        {
            List<Duck> ducks = new List<Duck>();
            float radius = xscale * 9f;

            //Ducks
            foreach (Duck duck in Level.CheckCircleAll<Duck>(position, radius))
            {
                if(!ducks.Contains(duck))
                {
                    ducks.Add(duck);
                }
            }

            //Ragdolls
            foreach(Ragdoll ragdoll in Level.CheckCircleAll<Ragdoll>(position, radius))
            {
                if(!ducks.Contains(ragdoll._duck))
                {
                    ducks.Add(ragdoll._duck);
                }
            }
            foreach (SmallFire with in Level.CheckCircleAll<SmallFire>(position, radius))
            {
                Level.Add(new GrenadeExplosion(x, y));
                Level.Remove(this);
            }
            if (inflamable == true)
            {
                foreach (Bullet b in Level.CheckCircleAll<Bullet>(position, radius))
                {
                    Level.Remove(this);
                    Level.Add(new GrenadeExplosion(x, y));
                }
            }

            foreach (Duck duck in ducks)
            {
                if(!duck.dead && !ToxicOnDuck.ToxicDucks.ContainsKey(duck))
                {
                    Level.Add(new ToxicOnDuck(duck));
                }
            }
        }
    }
}
