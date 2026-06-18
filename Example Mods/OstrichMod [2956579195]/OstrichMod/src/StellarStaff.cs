using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Magic")]
    class StellarStaff : Gun
    {
		public StateBinding _loadStateBinding = new StateBinding("_loadState", -1, false);

		public StateBinding _angleOffsetBinding = new StateBinding("_angleOffset", -1, false);

		public StateBinding _netLoadBinding = new NetSoundBinding("_netLoad");

		public NetSoundEffect _netLoad = new NetSoundEffect(new string[]
		{
			"loadSniper"
		});

		public int _loadState = -1;

		public int _loadAnimation = -1;

		public float _angleOffset;

        private SpriteMap sprite;

        public StellarStaff(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "StellarStaff";
            this.ammo = 3;
            this._ammoType = (AmmoType)new AT9mm();
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("StellarStaff"),54, 12);
            base.graphic = this.sprite;
            this.center = new Vec2(20f, 6f);
            this.collisionOffset = new Vec2(-15f, -5f);
            this.collisionSize = new Vec2(54f, 12f);
            this._barrelOffsetTL = new Vec2(54f, 5f);
            this._kickForce = 8f;
            this._holdOffset = new Vec2(0f, 2f);
            this._fireSound = GetPath("SFX/pewpew");
            this._fireWait = 10f;
			this.laserSight = true;
			this._laserOffsetTL = new Vec2(32f, 5f);
			this._manualLoad = true;
			this._ammoType.range = 0f;
		}
		public override void Update()
		{
			base.Update();
			if (this._loadState > -1)
			{
				if (this.owner == null)
				{
					if (this._loadState == 3)
					{
						this.loaded = true;
					}
					this._loadState = -1;
					this._angleOffset = 0f;
					this.handOffset = Vec2.Zero;
				}
				if (this._loadState == 0)
				{
					if (Network.isActive)
					{
						if (base.isServerForObject)
						{
							this._netLoad.Play(1f, 0f);
						}
					}
					else
					{
						SFX.Play(GetPath("sounds/sawdown"));
					}
					this._loadState++;
				}
				else if (this._loadState == 1)
				{
					if (this._angleOffset < 0.16f)
					{
						this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.2f, 0.15f);
					}
					else
					{
						this._loadState++;
					}
				}
				else if (this._loadState == 2)
				{
					this.handOffset.x = this.handOffset.x + 0.4f;
					if (this.handOffset.x > 4f)
					{
						this._loadState++;
						this.Reload(true);
						this.loaded = false;
					}
				}
				else if (this._loadState == 3)
				{
					this.handOffset.x = this.handOffset.x - 0.4f;
					if (this.handOffset.x <= 0f)
					{
						this._loadState++;
						this.handOffset.x = 0f;
					}
				}
				else if (this._loadState == 4)
				{
					if (this._angleOffset > 0.04f)
					{
						this._angleOffset = MathHelper.Lerp(this._angleOffset, 0f, 0.15f);
					}
					else
					{
						this._loadState = -1;
						this.loaded = true;
						this._angleOffset = 0f;
					}
				}
			}
			if (this.loaded && this.owner != null && this._loadState == -1)
			{
				this.laserSight = true;
				return;
			}
			this.laserSight = false;
		}

		public override void OnPressAction()
		{
			if (this.loaded)
			{
                if (this.ammo > 0)
                {
					if (this.isServerForObject)
					{
						SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f, 0f, false);
						Vec2 pos = this.Offset(this.barrelOffset);
						Vec2 target = this.Offset(this.barrelOffset + new Vec2(1200f, 0.0f)) - pos;
						DeathBeam deathBeam = new DeathBeam(pos, target);
						deathBeam.isLocal = this.isServerForObject;
						Level.Add((Thing)deathBeam);
					}
				}
				base.OnPressAction();
				return;
			}
			if (this.ammo > 0 && this._loadState == -1)
			{
				this._loadState = 0;
				this._loadAnimation = 0;
			}
		}

		public override void Draw()
		{
			float angle = this.angle;
			if (this.offDir > 0)
			{
				this.angle -= this._angleOffset;
			}
			else
			{
				this.angle += this._angleOffset;
			}
			base.Draw();
			this.angle = angle;
		}
	}
}

