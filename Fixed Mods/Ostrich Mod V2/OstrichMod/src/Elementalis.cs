using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | FireAndWater")]
    class Elementalis : Gun
    {
        private SpriteMap sprite;
        private int counter = 0;
        private bool obsidian = false;

        public Elementalis(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Elementalis";
            this.ammo = 12;
            this._ammoType = new ATBlueLacer();
            this._fireWait = 1f;
            this._type = "gun";
            this._fullAuto = false;
            base.graphic = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Elementalis_Aqua"),27, 10);
            this.center = new Vec2(13.5f, 5f);
            this.collisionOffset = new Vec2(-13.5f, -5f);
            this.collisionSize = new Vec2(27f, 10f);
            this._barrelOffsetTL = new Vec2(27f, -1f);
            this._holdOffset = new Vec2(0f, 0.0f);
            this._fireSound = GetPath("Sounds/revolver.wav");
            this.weight = 5f;
            this._editorName = "Elementalis";
        }
        public override void Fire()
        {
            base.Fire();
            if (counter == 0)
            {
                base.graphic = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Elementalis_Ignis"), 27, 10, false);
                this._ammoType = new ATRedLacer();
                counter = 1;
            }
            else
            {
                base.graphic = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Elementalis_Aqua"), 27, 10, false);
                this._ammoType = new ATBlueLacer();
                counter = 0;
            }
            if (this.ammo == 1 && obsidian == false)
            {
                counter = 2;
                base.graphic = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Elementalis_Obsidian"), 27, 10, false);
                this._ammoType = new ATPurpleLacer();
                this.ammoType.penetration = 3f;
                this._ammoType.accuracy = 0.2f;
                this._numBulletsPerFire = 30;
                obsidian = true;
                this._fireSound = GetPath("SFX/whargarble.wav");
            }


        }
    }
}
