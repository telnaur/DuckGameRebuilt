using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Eternal")]
    class SebasBook : Gun
    {
        private SpriteMap sprite;

        private Sprite _halo;

        private float _haloAlpha;

        private SinWave _haloWave = 0.05f;

        public SebasBook(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Sebas Book";
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("SebasBook"),17, 12);
            graphic = sprite;
            center = new Vec2(8f, 5f);
            collisionOffset = new Vec2(-9f, -7f);
            collisionSize = new Vec2(17f, 12f);
            _halo = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("SebasHalo"), 0f, 0f);
            _halo.CenterOrigin();
            _barrelOffsetTL = new Vec2(7f, 3f);
            ammo = 600;
            _ammoType = new ATToxic();
            _ammoType.accuracy = 0.3f;
            _numBulletsPerFire = 3;
            this._fireSound = "missile";
            _fullAuto = true;
            _fireWait = 6f;
            _kickForce = 0f;
            _holdOffset = new Vec2(4f, -1f);

        }
		public override void Update()
		{

			_haloAlpha = Lerp.Float(this._haloAlpha, (base.duck != null && base.ammo > 0) ? 1f : 0f, 0.05f);
			base.Update();
		}
		public override void Draw()
		{
            if (this.owner != null && this._haloAlpha > 0.01f)
			{
				this._halo.alpha = this._haloAlpha * 0.4f + this._haloWave * 0.2f;
				this._halo.depth = -0.2f;
				this._halo.xscale = (this._halo.yscale = 0.95f + this._haloWave * 0.05f);
				this._halo.angle += 0.01f;
				Graphics.Draw(this._halo, this.owner.x, this.owner.y);
			}
			base.Draw();
		}

    }
}
