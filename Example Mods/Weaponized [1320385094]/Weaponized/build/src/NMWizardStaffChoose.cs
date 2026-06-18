using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMod.src
{
    public class NMWizardStaffChoose : NMEvent
    {
        public Duck duck;
        public Vec2 pos;
        public byte frame;

        public NMWizardStaffChoose()
        {
        }

        public NMWizardStaffChoose(Duck chosenDuck, Vec2 staffPos, byte pFrame)
        {
            this.duck = chosenDuck;
            this.pos = staffPos;
            this.frame = pFrame;
        }

        public override void Activate()
        {
            wizardStaff.ChoosePlayer(this.duck, this.pos, (int)this.frame, true);
            base.Activate();
        }
    }
}
