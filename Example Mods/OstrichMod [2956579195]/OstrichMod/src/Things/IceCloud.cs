using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.OstrichMod
{
    public class IceCloud : Thing
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


        public IceCloud(float xval, float yval, float stayTime = 0.6f) : base(xval, yval)
        {
            velocity = new Vec2(Rando.Float(-5, 5), Rando.Float(-5, 5));
            xscale = Rando.Float(0.005f, 0.005f);
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
            graphic1.color = Color.Aquamarine;
            graphic1.alpha = 0.5f;
            graphicList.Add(graphic1);

            Sprite graphic2 = new Sprite("smokeBack", 0.0f, 0.0f);
            graphic2.depth = (Depth)0.1f;
            graphic2.CenterOrigin();
            graphic2.color = Color.Aquamarine;
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

            if(xscale < 0.00001)
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
            float radius = xscale * 10f;

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
                Level.Remove(with);
            }
            foreach (Holdable thing in Level.CheckCircleAll<Holdable>(position, radius))
            {
                thing.Extinquish();
                thing.heat -= 0.25f;
                if (thing.heat <= -1)
                {
                    Level.Remove(thing);
                    Level.Add(new IceBlock(thing.position.x, thing.position.y));
                }
            }

            foreach (Duck duck in ducks)
            {
                if(!duck.dead)
                {
                    duck.velocity = new Vec2(duck.velocity.x * 0.7f, duck.velocity.y < 0 ? duck.velocity.y : duck.velocity.y);
                }
            }
        }
    }
}
