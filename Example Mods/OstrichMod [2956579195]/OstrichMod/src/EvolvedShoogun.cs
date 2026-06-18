using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Western")]
    public class EvolvedShogun : Gun
    {
        private SpriteMap sprite;

        public EvolvedShogun(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Evolved_Shogun"),33, 7);
            graphic = sprite;
            this._center = new Vec2(16f, 4f);
            this._barrelOffsetTL = new Vec2(34f, 0f);
            this._collisionSize = new Vec2(33f, 8f);
            this._collisionOffset = new Vec2(-16f, -4f);
            this._holdOffset = new Vec2(4f, 3f);

            // weapon settings
            this.ammo = 16;
            this._fullAuto = true;
            this._ammoType = (AmmoType)new ATShotgun();
            this._numBulletsPerFire = 2;
            this._fireSound = GetPath("sounds/mafia");
            this._kickForce = 2f;
            this.loseAccuracy = 0.2f;
            this.maxAccuracyLost = 0.5f;

            this._editorName = "Evolved Shogun";
        }
    }
}
   
