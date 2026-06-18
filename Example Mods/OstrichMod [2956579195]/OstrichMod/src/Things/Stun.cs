using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.OstrichMod
{
    public class Stun : Thing
    {

        Duck duck;
        Ragdoll Rag;
        int f = 0;
        public StateBinding _frames = new StateBinding("f",-1,false,false);

        public Stun(Duck duck) : base(0f)
        {
            this.duck = duck;
        }
        public override void Initialize()
        {
          duck.immobilized = true;

          duck.GoRagdoll();
          SFX.Play(GetPath("sounds/stuuned"));
          Rag = duck.ragdoll;
         // duck.Scream();
          base.Initialize();
        }
        public override void Update()
        {
          duck.GoRagdoll();
          Rag._makeActive = false;


            f++;
            if(duck.dead)
              Level.Remove(this);
            if(f >= 150)
            {
              if(this.Rag.holdingOwner != null)
              {
              Duck d =  this.Rag.holdingOwner as Duck;
              d.ThrowItem(true);

              }
              //Rag._makeActive = true;
              //this.Rag.UpdateUnragdolling();
              Level.Remove(this);
            }
            base.Update();

        }
    }
}
