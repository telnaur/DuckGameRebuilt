using System;
using DuckGame;
using System.Linq;
using System.Collections.Generic;

namespace DuckGame.BrutalDG
{
	internal class DuckGib : Thing
	{
		public DuckGib(float xpos, float ypos, DuckPersona persona, int frame, Vec2 speed, bool camping = false) : base(xpos, ypos)
		{
			this._sprite = new SpriteMap(BrutalDG._gibsSprite.FirstOrDefault(x => x.Key == persona).Value, 8, 8);
			this._collisionSize = new Vec2(8f, 8f);
			this._collisionOffset = new Vec2(-4f, -4f);
			this.center = new Vec2(4f, 4f);
			this.graphic = this._sprite;
			this.frame = frame;
			this.angleAdd = Rando.Float(-2.4f, 2.5f);
			this._hSpeed = speed.x;
			this._vSpeed = speed.y;
			base.depth = this.depth.value + Rando.Float(-0.5f, 0.51f);
			this.lifetime = BrutalOptionsData.gibslifetime * 64f;
			if (!camping)
			{
				this._blood = new SpriteMap(GetPath("duckketchupducks1blood"), 8, 8);
				this._blood.CenterOrigin();
				this._blood.frame = this.frame;
				if (BrutalOptionsData.bloodcolor == BrutalDG.blood.Count)
				{
					this._blood.color = BrutalDG.blood[Rando.Int(BrutalDG.blood.Count - 1)];
				}
			}
			else
            {
				this._sprite = new SpriteMap(GetPath("campingparticle"), 8, 8);
				this.graphic = this._sprite;
				this._noblood = true;
				if (frame > 15)
                {
					this.frame = Rando.Int(5, 11);
                }
				else
                {
					this.frame = Rando.Int(5);
				}
            }
		}
		
		public override void Update()
		{
			foreach (ForceWave wave in Level.current.things[typeof(ForceWave)])
			{
				if (Level.CheckRectAll<DuckGib>(wave.topLeft, wave.bottomRight).Contains(this))
				{
					var h = typeof(ForceWave).GetField("_speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(wave);
					var v = typeof(ForceWave).GetField("_speedv", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(wave);
					if (h != null && v != null)
					{
						this._noblood = true;
						this._hSpeed = (((float)h - 3f) * (float)wave.offDir * 1.5f + (float)wave.offDir * 4f) * wave.alpha * Rando.Float(0.4f, 0.7f);
						this._vSpeed = ((float)v + -4.5f) * wave.alpha * Rando.Float(0.6f, 0.8f);
					}
				}
			}
			if (!DuckGib.allgibs.Contains(this))
			{
				DuckGib.allgibs.Add(this);
			}
			if (Level.current.things[typeof(DuckGib)].Count() > BrutalOptionsData.gibsamount && BrutalOptionsData.gibsamount != 0)
			{
				DuckGib.allgibs.First().Remove();
			}
			if (BrutalOptionsData.bloodcolor < BrutalDG.blood.Count && this._blood != null)
			{
				this._blood.color = BrutalDG.blood[BrutalOptionsData.bloodcolor];
			}
			base.Update();
			if (this.lifetime < 44f)
			{
				this.lifetime -= 0.1f;
				if (this.lifetime <= 0.5f)
				{
					this.Remove();
				}
			}
			else
			{
				this.lifetime = BrutalOptionsData.gibslifetime * 140f;
			}
			foreach (Block block in Level.CheckPointAll<Block>(this.bottomLeft - new Vec2(0f, 4f)))
			{
				this._hSpeed *= -1f;
			}
			foreach (Block block in Level.CheckPointAll<Block>(this.bottomRight - new Vec2(0f, 4f)))
			{
				this._hSpeed *= -1f;
			}
			foreach (Block block in Level.CheckPointAll<Block>(this.topLeft + new Vec2(4f, 0f)))
			{
				if (this._vSpeed < 0f)
				{
					this._vSpeed *= -1.1f;
				}
			}
			bool flag = false;
			if (this._vSpeed > 0f)
			{
				foreach (Block block in Level.CheckPointAll<Block>(this.bottomLeft + new Vec2(4f, 0f)))
				{
					if (this._vSpeed > -0.4f && this._vSpeed < 0.4f)
					{
						return;
					}
					this._vSpeed *= -0.7f;
					flag = true;
				}
				if (!flag)
				{
					foreach (AutoPlatform platform in Level.CheckPointAll<AutoPlatform>(this.bottomLeft + new Vec2(4f, 0f)))
					{
						if (this._vSpeed > -0.4f && this._vSpeed < 0.4f)
						{
							flag = true;
							return;
						}
						this._vSpeed *= -0.7f;
						flag = true;
					}
				}
			}
			this._hSpeed = Lerp.Float(this._hSpeed, 0f, 0.01f);
			this._vSpeed = Lerp.Float(this._vSpeed, 10f, 0.1f);
			this.position.x += this._hSpeed;
			this.position.y += this._vSpeed;
			this.angleDegrees += this.angleAdd * this._hSpeed * 1.5f;
			if (!this._noblood)
			{
				this.AddBlood();
			}
		}
		
		public void AddBlood()
		{
			if (BrutalOptionsData.enableblood)
			{
				FluidData fluid = new FluidData(0f, this._blood.color.ToVector4() * 0.8f, 0.4f, null, 0f, 0.7f);
				fluid.amount = Rando.Float(0.0005f, 0.001f);
				float num = BrutalOptionsData.bloodamount;
				int num1 = Rando.Int(4) + 1;
				if (num >= 0.6f)
				{
					num1 = 3;
				}
				if (num1 == 2)
				{
					Level.Add(new KetchupParticle(this.x, this.y, new Vec2(-this._hSpeed + Rando.Float(-1f, 1f), -this._vSpeed + Rando.Float(-1f, 1f)), fluid, null, 2.5f)
					          {
					          	depth = base.depth + 1
					          });
				}
				if (num1 == 3)
				{
					for (int i = 0; i < (num * 10 - 3) / 2; i++)
					{
						Level.Add(new KetchupParticle(this.x, this.y, new Vec2(-this._hSpeed + Rando.Float(-1f, 1f), -this._vSpeed + Rando.Float(-1f, 1f)), fluid, null, 2.5f)
						          {
						          	depth = base.depth + 1
						          });
					}
				}
			}
		}
		
		public void Remove()
		{
			this.alpha -= 0.01f;
			if (this.alpha <= 0f)
			{
				DuckGib.allgibs.Remove(this);
				Level.Remove(this);
			}
		}
		
		public override void Draw()
		{
			if (this._blood != null)
			{
				this._blood.angleDegrees = this.angleDegrees;
				base.Draw(this._blood, 0f, 0f, 1);
			}
			base.Draw();
		}
		
		public static List<DuckGib> allgibs = new List<DuckGib>();
		
		private SpriteMap _sprite;
		
		private SpriteMap _blood;
		
		public float angleAdd;
		
		private float lifetime;
		
		private bool _noblood;
	}
}
