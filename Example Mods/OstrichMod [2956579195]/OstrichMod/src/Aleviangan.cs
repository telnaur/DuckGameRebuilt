using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | BioLogic")]
    class Aleviangan : Gun
    {
        private SpriteMap sprite;

        public Aleviangan(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Aleviangan";
            this.ammo = 30;
            this._ammoType = new ATFuciel();
            this._type = "gun";
            this._fullAuto = true;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Aleviangan"),29, 15);
            this.sprite.AddAnimation("Aleviangan", 0.1f, true, new int[] { 0, 1 });
            this.sprite.SetAnimation("Aleviangan");
            base.graphic = this.sprite;
            this.center = new Vec2(16f, 9f);
            this.collisionOffset = new Vec2(-16f, -9f);
            this.collisionSize = new Vec2(29f, 15f);
            this._barrelOffsetTL = new Vec2(30f, 8f);
            this._holdOffset = new Vec2(0f, 0.0f);
            this._fireSound = GetPath("Sounds/Nico.wav");
            this.weight = 5f;
            this._editorName = "Aleviangan";
        }

    }
}
