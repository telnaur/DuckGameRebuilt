using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Western")]
    public class Shoogun : Gun
    {
        public StateBinding _loadStateStateBinding = new StateBinding("_loadState");

        public int _loadState;

        private float angleOffset;

        private SpriteMap sprite;

        public Shoogun(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Shoogun"),33, 8);
            graphic = sprite;
            this._center = new Vec2(16f, 4f);
            this._barrelOffsetTL = new Vec2(34f, 1f);
            this._collisionSize = new Vec2(33f, 8f);
            this._collisionOffset = new Vec2(-16f, -4f);
            this._holdOffset = new Vec2(4f, 3f);

            // weapon settings
            this.ammo = 6;
            this._ammoType = (AmmoType)new ATShotgun();
            this._numBulletsPerFire = 2;
            this._fireSound = GetPath("sounds/mafia");
            this._kickForce = 2f;

            // defaults
            _loadState = 0;
        }

        public override void Update()
        {
            if (_loadState == -1 || _loadState >= 1)
                _hasTrigger = false;
            else
                _hasTrigger = true;
            if (_loadState == 1)
            {
                if (angleOffset > -0.7)
                    angleOffset = MathHelper.Lerp(angleOffset, -0.8f, 0.12f);
                else
                    _loadState++;
            }
            else if (_loadState == 2)
            {
                if (handOffset.x < 2)
                {
                    handOffset.x += 0.08f;
                    handOffset.y -= 0.24f;
                }
                else
                    _loadState++;
            }
            else if (_loadState >= 3 && _loadState != 5)
            {
                if (handOffset.x > 0)
                {
                    handOffset.x -= 0.08f;
                    handOffset.y += 0.24f;
                }
                else
                    _loadState++;
                if (angleOffset < 0)
                    angleOffset = MathHelper.Lerp(angleOffset, 0f, 0.24f);
                else
                    _loadState++;
            }
            else if (_loadState == 5)
                _loadState = 0;
            base.Update();
        }

        public override void Draw()
        {
            if (_loadState == 0 || _loadState >= 3)
                sprite.frame = 1;
            else
                sprite.frame = 0;

            float angle = this.angle;
            if ((int)this.offDir > 0)
                this.angle = this.angle - this.angleOffset;
            else
                this.angle = this.angle + this.angleOffset;
            base.Draw();
            this.angle = angle;
        }


        public override void CheckIfHoldObstructed()
        {
            if (duck != null && _loadState != 0)
                duck.holdObstructed = false;
        }

        public override void OnPressAction()
        {
            if (duck == null)
                return;
            if(this.ammo > 0)
            {
                if (_loadState == 0)
                {
                    base.OnPressAction();
                    _loadState = -1;
                    SmallSmoke smallSmoke = SmallSmoke.New(this.barrelPosition.x, this.barrelPosition.y);
                    smallSmoke.scale = new Vec2(0.3f, 0.3f);
                    smallSmoke.hSpeed = Rando.Float(-0.1f, 0.1f);
                    smallSmoke.vSpeed = -Rando.Float(0.05f, 0.2f);
                    smallSmoke.alpha = 0.6f;
                    Level.Add((Thing)smallSmoke);
                }
                else if (_loadState == -1)
                {
                    _loadState = 1;
                }
            }
            else if(this.ammo <= 0)
            {
                DoAmmoClick();
            }
        }
    }
}
   
