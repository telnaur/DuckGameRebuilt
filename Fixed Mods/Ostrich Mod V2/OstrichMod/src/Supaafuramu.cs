using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | FireAndWater")]
    class Supaafuramu : Gun
    {
        public sbyte _loadProgress = 100;

        public float _loadAnimation = 1f;

        public StateBinding _loadProgressBinding = new StateBinding("_loadProgress", -1, false);

        protected SpriteMap _loaderSprite;

        public Supaafuramu(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Supaafuramu";
            this.ammo = 6;
            this._ammoType = new ATMin();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("Supaafuramu"), 27, 8);
            this.center = new Vec2(14f, 3f);
            this.collisionOffset = new Vec2(-14f, -3f);
            this.collisionSize = new Vec2(27f, 8f);
            this._barrelOffsetTL = new Vec2(27f, 1f);
            this._holdOffset = new Vec2(10f, 2f);
            this._fullAuto = false;
            this._fireWait = 6;
            this._kickForce = 4f;
            this._fireSound = GetPath("SFX/firewandnoise");
            this._manualLoad = true;
            this._loaderSprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("SupaafuramuLoader"), 8, 8, false);
            this._loaderSprite.center = new Vec2(7f, 2f);
        }
		public override void Update()
		{
			base.Update();
			if (this._loadAnimation == -1f)
			{
				SFX.Play("shotgunLoad", 1f, 0f, 0f, false);
				this._loadAnimation = 0f;
			}
			if (this._loadAnimation >= 0f)
			{
				if (this._loadAnimation == 0.5f && this.ammo != 0)
				{
					this._ammoType.PopShell(base.x, base.y, (int)(-(int)this.offDir));
				}
				if (this._loadAnimation < 1f)
				{
					this._loadAnimation += 0.1f;
				}
				else
				{
					this._loadAnimation = 1f;
				}
			}
			if (this._loadProgress >= 0)
			{
				if (this._loadProgress == 50)
				{
					this.Reload(false);
				}
				if (this._loadProgress < 100)
				{
					this._loadProgress += 10;
					return;
				}
				this._loadProgress = 100;
			}
		}
		public override void OnPressAction()
		{
			if (this.loaded)
			{
				if (this.ammo > 0)
				{
					this.ammo--;
					this.kick = 4f;
					if (this.receivingPress || !this.isServerForObject)
						return;
					Level.Add((Thing)SmallFire.New(this.barrelPosition.x, this.barrelPosition.y, Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).x * Rando.Float(8f, 10f), Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).y * Rando.Float(8f, 10f), false, (MaterialThing)null, true, (Thing)this, false));
					Level.Add((Thing)SmallFire.New(this.barrelPosition.x, this.barrelPosition.y, Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).x * Rando.Float(8f, 10f), Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).y * Rando.Float(8f, 10f), false, (MaterialThing)null, true, (Thing)this, false));
					Level.Add((Thing)SmallFire.New(this.barrelPosition.x, this.barrelPosition.y, Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).x * Rando.Float(8f, 10f), Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).y * Rando.Float(8f, 10f), false, (MaterialThing)null, true, (Thing)this, false));
					Level.Add((Thing)SmallFire.New(this.barrelPosition.x, this.barrelPosition.y, Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).x * Rando.Float(8f, 10f), Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).y * Rando.Float(8f, 10f), false, (MaterialThing)null, true, (Thing)this, false));
					Level.Add((Thing)SmallFire.New(this.barrelPosition.x, this.barrelPosition.y, Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).x * Rando.Float(8f, 10f), Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).y * Rando.Float(8f, 10f), false, (MaterialThing)null, true, (Thing)this, false));
					Level.Add((Thing)SmallFire.New(this.barrelPosition.x, this.barrelPosition.y, Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).x * Rando.Float(8f, 10f), Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f)).y * Rando.Float(8f, 10f), false, (MaterialThing)null, true, (Thing)this, false));

					SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f, 0f, false);
				}
				this._loadProgress = -1;
				this._loadAnimation = -0.01f;
				return;
			}
			if (this._loadProgress == -1)
			{
				this._loadProgress = 0;
				this._loadAnimation = -1f;
			}
		}

		public override void Draw()
		{
			base.Draw();
			Vec2 vec = new Vec2(13f, -2f);
			float num = (float)System.Math.Sin((double)(this._loadAnimation * 3.14f)) * 3f;
			base.Draw(this._loaderSprite, new Vec2(vec.x - 8f - num, vec.y + 4f), 1);
		}

	}
}

