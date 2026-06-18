using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Stuff|Props")]

    public class radioBlock : Holdable, IPlatform
    {
        private SpriteMap _sprite;
        private bool On = true;
        public float _timer = 1f;
        private float channel = 0f;
        protected List<Duck> _aboveList = new List<Duck>();

        public radioBlock(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap(GetPath("radio"), 22, 14, false);
            this._sprite.AddAnimation("off", 0.5f, false, 0);
            this._sprite.AddAnimation("on", 0.25f, 1 != 0, 0, 0, 0, 1);
            this._sprite.SetAnimation("on");
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(11f, 7f);
            this.collisionOffset = new Vec2(-11f, -7f);
            this.collisionSize = new Vec2(22f, 14f);
            this.depth = -0.5f;
            this.thickness = 2f;
            this.weight = 6f;
            this.collideSounds.Add("tinymotion");
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._editorName = "Radio";
            this.editorTooltip = "If you manage to catch it you can change the ingame music to your liking.";
        }
        public override void Removed()
        {
            Music.RandomTrack("Content/Audio/Music/InGame");
            Music.SwitchSongs();
            base.Removed();
        }
        public override void OnPressAction()
        {
            /*this.channel += 1f;
            if (this.channel == 1f)
            {
                SFX.Play("switchchannel", 1f, 0f, 0.0f, false);
                Music.Load(GetPath("countryRoads.ogg"));
                Music.PlayLoaded();
                this._sprite.SetAnimation("on");
                this.On = true;
                return;
            }
            if (this.channel == 2f)
            {
                SFX.Play("switchchannel", 1f, 0f, 0.0f, false);
                Music.Load(GetPath("russia.ogg"));
                Music.PlayLoaded();
            }
            if (this.channel == 3f)
            {
                SFX.Play("switchchannel", 1f, 0f, 0.0f, false);
                Music.Load(GetPath("takeOnMe.ogg"));
                Music.PlayLoaded();
            }
            if (this.channel == 4f)
            {
                SFX.Play("switchchannel", 1f, 0f, 0.0f, false);
                Music.RandomTrack("Content/Audio/Music/InGame");
                Music.SwitchSongs();
                this.On = false;
                this._sprite.SetAnimation("off");
                this.channel = 0f;
            }*/
            SFX.Play("switchchannel", 1f, 0f, 0.0f, false);
            Music.RandomTrack("Content/Audio/Music/InGame");
            Music.SwitchSongs();
            base.OnPressAction();
        }

        public override void Update()
        {
            if (this.On)
                this._timer -= 0.016f;
            if (this._timer < 0.0)
            {
                /*Duck duck = owner as Duck;
                if (owner != null)
                {
                    duck.ThrowItem(true);
                }*/
                this.vSpeed = Rando.Float(-1.8f, -2f);
                this.hSpeed = Rando.Float(-1.6f, 1.6f);
                this._timer = 1f;
                this._aboveList = Level.CheckRectAll<Duck>(this.topLeft + new Vec2(1f, -10f), this.bottomRight + new Vec2(-1f, -23f)).ToList<Duck>();
                foreach (Duck above in this._aboveList)
                {
                    if (above.grounded || (double)above.vSpeed > 0.0 || (double)above.vSpeed == 0.0)
                    {
                        above.y -= 2f;
                        above.vSpeed = -3f;
                    }
                }
            }
            base.Update();
        }

        public override void EditorUpdate()
        {
            this._sprite.SetAnimation("off");
            base.EditorUpdate();
        }
    }
}
