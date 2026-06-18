using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    [EditorGroup("stuff|props|Deathcrates++")]
    class SpecifiedCrate : DeathCrate
    {
        private SpriteMap _sprite;
        private bool _didActivation = false;

        int customSettingIndex = -1;

        public EditorProperty<int> customSettingInput = new EditorProperty<int>(-1, null, -1, 16, 1, null, false, false);
        public EditorProperty<int> extremeSetting = new EditorProperty<int>(0, null, 0, 1, 1, null, false, false);

        Dictionary<int, int> inputToIndex = new Dictionary<int, int>()
        {
            {-1,-1}, {0,0}, {1,1}, {2,2}, {3,3}, {4,4}, {5,5}, {6,6}, {7,7}, {8,8}, {9,10}, {10,11}, {11,33}, {12,35}, {13,37}, {14,39}, {15,41}
        };

        Dictionary<int, int> normalToExtreme = new Dictionary<int, int>()
        {
            {-1,-1}, {0,22}, {1,23}, {2,24}, {3,25}, {4,26}, {5,27}, {6,28}, {7,29}, {8,30}, {10,31}, {11,32}
        };

        public SpecifiedCrate(float xpos, float ypos) : base(xpos, ypos)
        {
            this._editorName = "Specified Death Crate";
            this._sprite = new SpriteMap(Mod.GetPath<DeathcratesPlusPlus>("crates\\specifieddeathcrate"), 16, 19, false);
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

            if (customSettingInput != -1 && customSettingIndex == -1 && inputToIndex.ContainsKey(customSettingInput))
            {
                customSettingIndex = inputToIndex[customSettingInput];

                if (extremeSetting == 1 && customSettingInput <= 10)
                {
                    customSettingIndex = normalToExtreme[customSettingIndex];
                }
            }

            if (customSettingIndex == -1)
            {
                customSettingIndex = Rando.Int(42);
            }
            this.settingIndex = (byte)customSettingIndex;
            base.Update();
        }
    }
}
