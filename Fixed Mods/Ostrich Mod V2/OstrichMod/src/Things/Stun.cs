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
        // Without this, ghost copies on remote clients had a null target and crashed in
        // Initialize/Update (the constructor that sets 'duck' never runs for ghosts).
        public StateBinding _duckStateBinding = new StateBinding("duck");

        public Stun(Duck duck) : base(0f)
        {
            this.duck = duck;
        }
        public override void Initialize()
        {
          if (duck == null)
          {
            Level.Remove(this);
            return;
          }
          // Do NOT set duck.immobilized here — GoRagdoll already blocks movement
          // (CanMove() returns false when ragdoll != null), and setting immobilized
          // on the Stun ghost causes remote clients to permanently freeze the duck
          // because Unragdoll() is gated by isServerForObject and never clears it.

          duck.GoRagdoll();
          SFX.Play(GetPath("sounds/stuuned"));
          Rag = duck.ragdoll;
          base.Initialize();
        }
        public override void Update()
        {
            // Target can be null on a ghost before sync, or after the duck despawns.
            if (duck == null || duck.dead)
            {
              Level.Remove(this);
              return;
            }

            duck.GoRagdoll();
            if (Rag == null)
              Rag = duck.ragdoll;
            if (Rag != null)
              Rag._makeActive = false;

            f++;
            if(f >= 150)
            {
              if(Rag != null && Rag.holdingOwner != null)
              {
                Duck d = Rag.holdingOwner as Duck;
                if (d != null)
                  d.ThrowItem(true);
              }
              // Let the duck stand up on the next player input.
              if (Rag != null)
                Rag._makeActive = true;
              Level.Remove(this);
            }
            base.Update();

        }

        public override void Removed()
        {
            // Safety cleanup: runs both when the server removes the Stun and when
            // the network manager removes the ghost on remote clients (where
            // Unragdoll() is never called because it's gated by isServerForObject).
            if (duck != null)
                duck.immobilized = false;
            base.Removed();
        }
    }
}
