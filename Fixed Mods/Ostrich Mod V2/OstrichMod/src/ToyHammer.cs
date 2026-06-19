using System;
using System.Collections.Generic;
using System.Linq;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Virgo")]

    public class ToyHammer : Holdable
    {
      int f = 0;
      private SpriteMap _swordSwing;
      float angleAbsolute = 0;
      // Ducks already stunned during the current swing, so each target is stunned once
      // instead of spawning a networked Stun every frame (the per-frame ghost flood
      // crashed online games the moment you connected).
      private readonly HashSet<Duck> _stunned = new HashSet<Duck>();
      public StateBinding _frames = new StateBinding("f",-1,false,false);
      public StateBinding _angleAbsolute = new StateBinding("angleAbsolute",-1,false,false);


        public ToyHammer(float xpos, float ypos) : base(xpos, ypos)
        {
          this.graphic = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("ToyHammer"),0, 0);
         // this.center = new Vec2(6,2);
         // this.collisionOffset = new Vec2(-6, -2);
          this._canRaise = false;
          this._editorName = "Toy Hammer";
          this.collisionSize = new Vec2(13, 28);
          this.handOffset = new Vec2(2, 0);
          this.center =new Vec2(7, 17);
          this.angleAbsolute = -0.1f;
          this.collisionOffset =new Vec2(-7,-18);
          this._swordSwing = new SpriteMap("swordSwipe", 32, 32, false);
          this._swordSwing.AddAnimation("swing", 0.6f, false, new int[4]
          {
            0,
            1,
            1,
            2
          });
          this._swordSwing.currentAnimation = "swing";
          this._swordSwing.speed = 0f;
          this._swordSwing.center = new Vec2(9f, 25f);
          //this.center =new Vec2(0,3f);

        }
        public override void Draw()
        {
          if (this._swordSwing.speed > 0.0)
      		{
      			if (base.duck != null)
      			{
              if(this.offDir == 1)
      				    this._swordSwing.flipH = false;
              else
                this._swordSwing.flipH = true;
      			}
      			this._swordSwing.alpha = 0.4f;
      			this._swordSwing.position = base.position;
      			this._swordSwing.depth = base.depth + 1;
      			this._swordSwing.Draw();
      		}
         // Vec2 pos1 = new Vec2(this.position.x,this.position.y );
        //  Graphics.DrawLine(pos1,new Vec2(pos1.x, pos1.y - 25),Color.Red,2);
         // Graphics.DrawLine(pos1,new Vec2(pos1.x + 10, pos1.y),Color.Blue,2);

          base.Draw();
        }
        public override void Update()
        {
         // this.handAngle = -0.1f * this.offDir;
          if (this._swordSwing.finished)
        		{
        			this._swordSwing.speed = 0f;
        		}

            if(this.duck == null)
            {
              f=0;
              this.angleAbsolute = -0.1f;
            }
            if(this.duck != null)
            {
              this.handAngle = angleAbsolute * offDir;
              if (this.duck.inputProfile.Pressed(Triggers.Shoot))
               {
                 if(f == 0)
                 {
                  f += 1;
                  _stunned.Clear();
                  SFX.Play("swipe", Rando.Float(0.8f, 1f), Rando.Float(-0.1f, 0.1f), 0f, false);
                  this._swordSwing.speed = 1f;
                  this._swordSwing.frame = 0;
                 }
             }

             if(f > 0)
             {
               f++;
               if(f < 10)
               {
                // Only the weapon's authority spawns the networked Stun Things; otherwise
                // every client floods the level with ghosts -> desync/crash online.
                if (isServerForObject)
                {
                  foreach(Duck dicko in Level.current.CollisionLineAll<Duck>(this.position, this.position + new Vec2((float)Math.Sqrt(Math.Pow(22, 2) - Math.Pow(Math.Sin(this.handAngle) * 22, 2)) * this.offDir, (float)Math.Sin(this.handAngle) * 22)))
                  {
                    if(!dicko.dead && dicko != this.duck && _stunned.Add(dicko))
                    {
                      Level.Add(new Stun(dicko));
                    }
                  }
                  foreach(RagdollPart dicko in Level.current.CollisionLineAll<RagdollPart>(this.position, this.position + new Vec2((float)Math.Sqrt(Math.Pow(22, 2) - Math.Pow(Math.Sin(this.handAngle) * 22, 2)) * this.offDir, (float)Math.Sin(this.handAngle) * 22)))
                  {
                    // Ragdoll parts can have a null doll/duck (loose parts, despawning) -> NRE.
                    Duck ragDuck = dicko._doll != null ? dicko._doll._duck : null;
                    if(ragDuck != null && !ragDuck.dead && ragDuck != this.duck && _stunned.Add(ragDuck))
                    {
                      Level.Add(new Stun(ragDuck));
                    }
                  }
                }
                this.angleAbsolute += 0.2f;
               }
               if(f > 50)
                 this.angleAbsolute = -0.1f;
               if(f > 80)
                  f =0;
             }


          }
          base.Update();

        }



    }
}
