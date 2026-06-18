using System;
using System.Linq;

namespace DuckGame.OstrichMod
{
	[EditorGroup("OstrichMod | BioLogic")]
	public class Caralinosa : Gun
	{
		public StateBinding _targetStateBinding = new StateBinding("_target", -1, false);

		public StateBinding _drawPositionStateBinding = new CompressedVec2Binding("_drawPosition", 2147483647);

		public StateBinding _chargingStateBinding = new StateBinding("_charging", -1, false);

		public StateBinding _coolingDownStateBinding = new StateBinding("_coolingDown", -1, false);

		public StateBinding _cooldownStateBinding = new StateBinding("_cooldown", -1, false);

		public StateBinding _chargeTimerStateBinding = new StateBinding("_chargeTimer", -1, false);

		public StateBinding netSFX_medusaStateBinding = new NetSoundBinding("netSFX_medusa");

		public NetSoundEffect netSFX_medusa = new NetSoundEffect(new string[]
		{
			Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("sounds/Mimic")
		});

		public PhysicsObject _target;

		public Vec2 _drawPosition;

		public bool _charging;

		public bool _coolingDown;

		public int _cooldown;

		public int _chargeTimer;

		private SpriteMap sprite;

		private Sprite medusaBit;

		private Tex2D beam;

		private Tex2D laserTexture;

		public Caralinosa(float xpos, float ypos) : base(xpos, ypos)
		{
			this._editorName = "Caralinosa";
			this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Caralinosa"), 18, 11, false);
			base.graphic = this.sprite;
			this._center = new Vec2(9f, 5.5f);
			this._collisionOffset = new Vec2(-9f, -5.5f);
			this._collisionSize = new Vec2(18f, 11f);
			this._barrelOffsetTL = new Vec2(18f, 6f);
			this._holdOffset = new Vec2(4f, 0f);
			this.ammo = 1;
			this._weight = 3f;
			this.medusaBit = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("CaralinosaBit"), 0f, 0f);
			this.medusaBit.CenterOrigin();
			this.beam = Content.Load<Tex2D>(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("CaralinosaBeam.png"));
			this.laserTexture = Content.Load<Tex2D>("pointerLaser");
		}

		public override void Update()
		{
			if (this.owner != null)
			{
				foreach (Duck d in Level.CheckCircleAll<Duck>(Offset(barrelOffset), 2f))
					if (Level.CheckLine<Block>(position, d.position, d) == null && d != this.owner)
						d.Kill(new DTImpale(null));

				float num = this.angle + ((this.offDir < 0) ? 3.14159274f : 0f);
				foreach (PhysicsObject current in Level.CheckCircleAll<PhysicsObject>(this.Offset(base.barrelOffset) + 135f * new Vec2((float)Math.Cos((double)num), (float)Math.Sin((double)num)), 135f))
				{
					if (current != this && current != this.owner && current.owner == null && current.solid && current.thickness > 0.4f && !(current is Holdable) && !(current is IPlatform) && !(current is Equipment) && !(current is Gun) && (current is Duck || (current.collisionSize.length <= 256f && current.collisionSize.x <= 26f && current.collisionSize.y <= 26f)) && Level.CheckLine<Block>(this.Offset(base.barrelOffset), current.position, current) == null && (this._target == null || (this.Offset(base.barrelOffset) - current.position).length < (this.Offset(base.barrelOffset) - this._target.position).length))
					{
						this._target = current;
					}
				}
				if (this._target != null)
				{
					if (!Level.CheckCircleAll<PhysicsObject>(this.Offset(base.barrelOffset) + 135f * new Vec2((float)Math.Cos((double)num), (float)Math.Sin((double)num)), 135f).Contains(this._target) || Level.CheckLine<Block>(this.Offset(base.barrelOffset), this._target.position, this._target) != null)
					{
						this._target = null;
						this._chargeTimer = 0;
					}
					else if (this._chargeTimer >= 0 && !this._coolingDown)
					{
						if (this._charging)
						{
							if (this._chargeTimer < 20)
							{
								this._chargeTimer++;
							}
							else
							{
								if (base.isServerForObject)
								{
									this.netSFX_medusa.Play(0.6f, 0f);
									this._drawPosition = this._target.position;
									this.Metamorphosis(this._target);
									this._target = null;
								}
								this._coolingDown = true;
								this._chargeTimer = 0;
							}
						}
						else
						{
							this._chargeTimer = 0;
						}
					}
				}
				else
				{
					this._chargeTimer = 0;
				}
			}
			else
			{
				this._charging = false;
				if (this._chargeTimer > 0)
				{
					this._chargeTimer = 0;
				}
			}
			if (this._coolingDown)
			{
				if (this._cooldown < 20)
				{
					this._cooldown++;
				}
				else
				{
					this._cooldown = 0;
					this._coolingDown = false;
				}
			}
			base.Update();
		}

		private void Metamorphosis(PhysicsObject physicsObject)
		{
			Duck duck = physicsObject as Duck;
			if (duck != null)
			{
				Vec2 position = duck.position;
				Level.Add(new StunHandler(duck, 60, showDaze: true));
				Vec2 vec2 = -this.barrelVector * 6;
				duck.hSpeed += vec2.x;
				return;
			}
		}

		public override void OnReleaseAction()
		{
			this._charging = false;
		}

		public override void OnPressAction()
		{
			if (this.owner != null)
			{
				this._charging = true;
			}
		}

		public override void Fire()
		{
		}

		public override void Draw()
		{
			if (this._coolingDown)
			{
				float num = 1f - (float)this._cooldown / 20f;
				this.medusaBit.alpha = num;
				Sprite expr_31 = this.medusaBit;
				expr_31.xscale = (expr_31.yscale = 0.75f);
				Graphics.Draw(expr_31, this.Offset(base.barrelOffset).x, this.Offset(base.barrelOffset).y, 0.7f);
				Graphics.Draw(this.medusaBit, this._drawPosition.x, this._drawPosition.y, 0.7f);
				Graphics.DrawTexturedLine(this.beam, this.Offset(base.barrelOffset), this._drawPosition, Color.White * num, 0.3f, 0.6f);
			}
			base.Draw();
		}
	}
}
