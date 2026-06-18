using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    [EditorGroup("stuff|props|Deathcrates++")]
    class DoomCrate : DeathCrate
    {
        private SpriteMap _sprite;

        private bool _didActivation = false;
        private int xtremeSettingIndex;

        public DoomCrate(float xpos, float ypos) : base(xpos, ypos)
        {
            this._editorName = "Doom Crate";
            this._sprite = new SpriteMap(Mod.GetPath<DeathcratesPlusPlus>("crates\\doomcrate"), 16, 19, false);
            base.graphic = this._sprite;
            SpriteMap arg_FE_0 = this._sprite;
            string arg_FE_1 = "idle";
            float arg_FE_2 = 1f;
            bool arg_FE_3 = true;
            int[] frames = new int[1];
            arg_FE_0.AddAnimation(arg_FE_1, arg_FE_2, arg_FE_3, frames);
            this._sprite.AddAnimation("activate", 0.35f, false, new int[]
            {
                1,
                2,
                3,
                4,
                4,
                5,
                4,
                4,
                5,
                6,
                6,
                6,
                6,
                6,
                6,
                6,
                6,
                5,
                7,
                7,
                7,
                7,
                7,
                7,
                7,
                7,
                5,
                8,
                8,
                8,
                8,
                8,
                8,
                8,
                8,
                5,
                9,
                9,
                5
            });
            this._sprite.SetAnimation("idle");
            xtremeSettingIndex = Rando.Int(10) + 22;
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            with.Fondle(this);
            if (from == ImpactedFrom.Top && with.totalImpactPower + base.totalImpactPower > 0.1f && this._sprite.currentAnimation == "idle")
            {
                this.activated = true;
                this._sprite.SetAnimation("activate");
                SFX.Play("click", 1f, 0f, 0f, false);
                this.collisionOffset = new Vec2(-8f, -8f);
                this.collisionSize = new Vec2(16f, 15f);
            }
            base.OnSolidImpact(with, from);
        }

        public override void Update()
        {
            if (this.activated && this._sprite.currentAnimation != "activate")
			{
				this._sprite.SetAnimation("activate");
			}
            if (this._sprite.imageIndex == 6 && this._beeps == 0)
            {
                SFX.Play("singleBeep", 1f, 0f, 0f, false);
                this._beeps += 1;
            }
            if (this._sprite.imageIndex == 7 && this._beeps == 1)
            {
                SFX.Play("singleBeep", 1f, 0f, 0f, false);
                this._beeps += 1;
            }
            if (this._sprite.imageIndex == 8 && this._beeps == 2)
            {
                SFX.Play("singleBeep", 1f, 0f, 0f, false);
                this._beeps += 1;
            }
            if (this._sprite.imageIndex == 5 && this._beeps == 3)
            {
                SFX.Play("doubleBeep", 1f, 0.2f, 0f, false);
                this._beeps += 1;
            }
            if (base.isServerForObject && this._sprite.currentAnimation == "activate" && this._sprite.finished && !this._didActivation)
            {
                this._didActivation = true;
                this.setting.Activate(this, true);
                Send.Message(new NMActivateDeathCrate(this.settingIndex, this));
            }
            this.settingIndex = (byte)xtremeSettingIndex;
            base.Update();
        }
    }
}
