using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Misc")]
    public class wizardStaff : Gun
    {
        public StateBinding _chosenDuckBinding = new StateBinding("chosenDuck", -1, false, false);
        private SpriteMap _sprite;
        public Duck chosenDuck;
        private List<Duck> ducksInRange = new List<Duck>();

        public wizardStaff(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._sprite = new SpriteMap(GetPath("wizardStaff"), 11, 40, false);
            this._sprite.AddAnimation("idle", 1f, true, new int[1]);
            this._sprite.AddAnimation("held", 0.1f, true, 0, 1, 2, 3);
            this._sprite.AddAnimation("empty", 1f, true, 4);
            this._sprite.SetAnimation("idle");
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(6f, 20f);
            this.collisionOffset = new Vec2(-6f, -20f);
            this.collisionSize = new Vec2(11f, 40f);
            this._fireSound = GetPath("wizardShort");
            this._holdOffset = new Vec2(-2f, -10f);
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._barrelOffsetTL = new Vec2(5f, Rando.Float(7f, 7f));
            this._editorName = "Wizard Staff";
            this.editorTooltip = "'Crash' a random duck to desktop with this highly advanced magical technology.";
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
            {
                this._sprite.SetAnimation("held");
            }
            base.Initialize();
        }

        public override void OnPressAction()
        {
            if (this.owner == null || !this.isServerForObject)
                return;
            if (this.ammo > 0)
            {
                foreach (Duck duck in Level.CheckCircleAll<Duck>(this.position, 10000f))
                {
                    if (!duck.dead)
                    {
                        ducksInRange.Add(duck);
                    }
                }
                if (ducksInRange.Any())
                {
                    chosenDuck = ducksInRange.ElementAt(Rando.Int(0, ducksInRange.Count() - 1));
                    if (chosenDuck != null)
                    {
                        this.ammo--;
                        //this.PlayFireSound();
                        SFX.PlaySynchronized(GetPath("wizardShort"), 0.95f, 0.0f, 0.0f, false);
                        //SFX.Play(GetPath("wizardShort"), 0.95f, 0.0f, 0.0f, false);// w update sprawdz czy ammo zmalalo i zagraj ten dzwiek
                        wizardStaff.ChoosePlayer(this.chosenDuck, this.barrelPosition, this._sprite.frame, false);
                    }
                }
            }
        }

        public static void ChoosePlayer(Duck chosenDuck, Vec2 staffPos, int pFrame, bool pIsNetMessage)
        {
            for (int j = 0; j < 10; j++)
            {
                if (j < 3)
                {
                    Level.Add(SmallSmoke.New(staffPos.x + Rando.Float(-0.5f, 0.5f), staffPos.y + Rando.Float(-0.5f, 0.5f)));
                }
                Level.Add(SmallSmoke.New(chosenDuck.x + Rando.Float(-5f, 5f), chosenDuck.y + Rando.Float(-5f, 5f)));
            }
            Level.Add((Thing)new blueScreen(chosenDuck.x, chosenDuck.y, true));
            chosenDuck.y = 10000f;

            SFX.Play("glassBreak", 1f, Rando.Float(-0.2f, 0.2f), 0.0f, false);
            for (int index = 0; index < 6; ++index)
            {
                wizardStaffDebris thing = wizardStaffDebris.New(staffPos.x + Rando.Float(-0.5f, 0.5f), staffPos.y + Rando.Float(-0.5f, 0.5f));
                thing.hSpeed = (float)(((double)Rando.Float(1f) > 0.5 ? 1.0 : -1.0) * (double)Rando.Float(2f) + (double)Math.Sign(staffPos.x) * 0.4);
                thing.vSpeed = -Rando.Float(1f);
                Level.Add((Thing)thing);
            }
            if (pIsNetMessage)
                return;
            Send.Message((NetMessage)new NMWizardStaffChoose(chosenDuck, staffPos, (byte)pFrame));
        }

        public override void DoDraw()
        {
            if (this.ammo <= 0)
            {
                this._sprite.SetAnimation("empty");
            }
            base.DoDraw();
        }
    }
}