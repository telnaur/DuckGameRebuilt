using System;
using DuckGame;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.BrutalDG
{
	internal class RagdollParts : Thing
	{
		public RagdollParts(RagdollPart r) : base()
		{
			if (r.part == 0)
			{
				this._over = new SpriteMap(BrutalDG._partsoverlaySprite.FirstOrDefault(x => x.Key == Persona.Number(r._persona)).Value, 32, 32);
				this._sprite = new SpriteMap(BrutalDG._parts1Sprite.FirstOrDefault(x => x.Key == Persona.Number(r._persona)).Value, 32, 32);
			}
			else
			{
				this._sprite = new SpriteMap(BrutalDG._parts2Sprite.FirstOrDefault(x => x.Key == Persona.Number(r._persona)).Value, 32, 32);
			}
			if (r.part == 0)
			{
				this._sprite.center = new Vec2(16f, 13f);
			}
			else if (r.part == 1)
			{
				this._sprite.center = new Vec2(16f, 13f);
			}
			else if (r.part == 3)
			{
				this._sprite.center = new Vec2(6f, 8f);
			}
			else
			{
				this._sprite.center = new Vec2(8f, 8f);
			}
			this.ragdoll = r;
			if (BrutalOptionsData.bloodcolor == BrutalDG.blood.Count && this._blood != null)
			{
				this._blood.color = BrutalDG.blood[Rando.Int(BrutalDG.blood.Count - 1)];
			}
			r.layer = RagdollParts._ragdollLayer;
			r.layer.visible = false;
			this._didrecreate = true;
			this._prevTex = r.graphic.texture;
			r.graphic.texture = BrutalDG._blank;
		}
		
		public void Remove()
		{
			if (this.ragdoll != null)
			{
				if (this.ragdoll.graphic != null && this._prevTex != null)
				{
					this.ragdoll.graphic.texture = this._prevTex;
				}
				this.ragdoll.layer = Layer.Game;
				this.ragdoll = null;
				Level.Remove(this);
			}
		}
		
		public override void Update()
		{
			List<Sword> swords = new List<Sword>();
			foreach (Sword sword in this._swords)
			{
				if (sword != null && sword._swing < 0.5f)
				{
					swords.Add(sword);
				}
			}
			foreach (Sword sword in swords)
			{
				this._swords.Remove(sword);
			}
			if (!BrutalOptionsData.enablegibs)
			{
				this.Remove();
			}
			if (this._sprite != null && this._sprite.texture != null && this._sprite.texture.nativeObject != null && (this._sprite.texture.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D) != null && (this._sprite.texture.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D).IsContentLost)
			{
				this.Recreate();
			}
			else if (this._sprite != null && this._sprite.texture != null && this.ragdoll != null && this.ragdoll._persona != null)
			{
				if (this.ragdoll._doll != null && !this.ragdoll._doll.inSleepingBag && !this._didrecreate)
                {
					if (this.ragdoll.part == 0)
					{
						this._over.texture = BrutalDG._partsoverlaySprite.FirstOrDefault(x => x.Key == Persona.Number(this.ragdoll._persona)).Value;
						this._sprite.texture = BrutalDG._parts1Sprite.FirstOrDefault(x => x.Key == Persona.Number(this.ragdoll._persona)).Value;
					}
					else
					{
						this._sprite.texture = BrutalDG._parts2Sprite.FirstOrDefault(x => x.Key == Persona.Number(this.ragdoll._persona)).Value;
					}
					this._didrecreate = true;
				}
				this.timer++;
				if (this.ragdoll._doll != null && !this.ragdoll._doll.inSleepingBag && this.timer >= Rando.Int(140, 201))
				{
					if (this.ragdoll.part == 0)
					{
						this._over.texture = BrutalDG._partsoverlaySprite.FirstOrDefault(x => x.Key == Persona.Number(this.ragdoll._persona)).Value;
						this._sprite.texture = BrutalDG._parts1Sprite.FirstOrDefault(x => x.Key == Persona.Number(this.ragdoll._persona)).Value;
					}
					else
					{
						this._sprite.texture = BrutalDG._parts2Sprite.FirstOrDefault(x => x.Key == Persona.Number(this.ragdoll._persona)).Value;
					}
					//string text = "ragdollparts1";
					//if (this.ragdoll.part != 0)
					//	text = "ragdollparts2";
					//SpriteMap sprite = new SpriteMap(GetPath(text), 32, 32);
					//if (this._sprite.texture != Graphics.Recolor(sprite.texture, this.ragdoll._persona.color))
					//{
					//	this._sprite.texture = Graphics.Recolor(sprite.texture, this.ragdoll._persona.color);
					//	this._didrecreate = true;
					//	if (this._over != null)
					//		this._over.texture = Graphics.Recolor(new SpriteMap(GetPath("ragdollparts1overlay"), 32, 32).texture, this.ragdoll._persona.color);
					//}
					this.timer = 0;
				}
			}
			if (this.ragdoll == null || this.ragdoll.removeFromLevel || this.ragdoll.part == 2)
			{
				Level.Remove(this);
			}
			else
			{
				RagdollPart r = this.ragdoll;
				if (r != null && this._sprite != null)
				{
					if (r.part == 0)
					{
						this._sprite.center = new Vec2(16f, 13f);
					}
					else if (r.part == 1)
					{
						this._sprite.center = new Vec2(16f, 13f);
					}
					else if (r.part == 3)
					{
						this._sprite.center = new Vec2(6f, 8f);
					}
					else
					{
						this._sprite.center = new Vec2(8f, 8f);
					}
				}
				if (this._blood != null)
				{
					this._blood.center = this._sprite.center;
				}
				if (BrutalOptionsData.bloodcolor < BrutalDG.blood.Count && this._blood != null)
				{
					this._blood.color = BrutalDG.blood[BrutalOptionsData.bloodcolor];
				}
				if (this._swordtimer <= 0f)
				{
					foreach (Sword sword in Level.current.things[typeof(Sword)])
					{
						if (Level.CheckLineAll<RagdollPart>(sword.barrelStartPos, sword.barrelPosition).Contains(this.ragdoll) && sword._swing > 0f && !this._swords.Contains(sword))
						{
							this._swords.Add(sword);
							this.RemovePart(new Bullet(this.ragdoll.x, this.ragdoll.y, new ATTracer(), Maths.PointDirection(sword.barrelStartPos, sword.barrelPosition) + Rando.Float(-2f, 3f), null, false, 0f, true, true), this.ragdoll.position, false);
							this._swordtimer = 2f;
						}
					}
					foreach (Chainsaw chainsaw in Level.current.things[typeof(Chainsaw)])
					{
						if (Level.CheckLineAll<RagdollPart>(chainsaw.barrelStartPos, chainsaw.barrelPosition).Contains(this.ragdoll) && chainsaw.throttle)
						{
							this.RemovePart(new Bullet(this.ragdoll.x, this.ragdoll.y, new ATTracer(), Maths.PointDirection(chainsaw.barrelStartPos, chainsaw.barrelPosition) + Rando.Float(-2f, 3f), null, false, 0f, true, true), this.ragdoll.position, false);
							this._swordtimer = 1f;
						}
					}
					foreach (ForceWave wave in Level.current.things[typeof(ForceWave)])
					{
						if (Level.CheckRectAll<RagdollPart>(wave.topLeft, wave.bottomRight).Contains(this.ragdoll))
						{
							var h = typeof(ForceWave).GetField("_speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(wave);
							var v = typeof(ForceWave).GetField("_speedv", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(wave);
							if (h != null && v != null)
							{
								float speedh = (float)h;
								float speedv = (float)v;
								this.RemovePart(new Bullet(this.ragdoll.x, this.ragdoll.y, new ATTracer(), Maths.PointDirection(Vec2.Zero, new Vec2(((speedh - 3f) * (float)wave.offDir * 1.5f + (float)wave.offDir * 4f) * wave.alpha, (speedv + -4.5f) * wave.alpha)) + Rando.Float(-2f, 3f), null, false, 0f, true, true), this.ragdoll.position, false);
							}
						}
					}
				}
				else
				{
					this._swordtimer -= 0.1f;
				}
			}
			base.Update();
		}
		
		public void RemovePart(Bullet bullet, Vec2 hitPos, bool special = false)
		{
			if (this._sprite != null && this.ragdoll != null && this.ragdoll._persona != null)
			{
				if (this._blood == null)
				{
					this._blood = new SpriteMap(GetPath("ragdollparts1blood"), 32, 32);
					if (this.ragdoll.part != 0)
					{
						this._blood = new SpriteMap(GetPath("ragdollparts2blood"), 32, 32);
					}
					if (this.ragdoll.graphic != null)
					{
						this._sprite.center = this.ragdoll.graphic.center;
						this._blood.center = this._sprite.center;
					}
				}
				bool flag = BrutalOptionsData.enablegibs;
				if (!special && bullet != null)
				{
					int part = this._sprite.frame;
					if (part >= 9 && part < 18)
						part -= 9;
					else if (part >= 18 && part < 27)
						part -= 18;
					else if (part >= 27)
						part -= 27;
					if (part == 0)
					{
						part = Rando.Int(1, 3);
						if (flag)
						{
							if (this.ragdoll.part == 0)
							{
								if (part == 1)
								{
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 2, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								}
								else
								{
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 5, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 1, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								}
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 4, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							}
							else
							{
								if (part == 1)
								{
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 11, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								}
								else
								{
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 12, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								}
							}
						}
					}
					else if (part == 1 || part == 2)
					{
						if (flag)
						{
							if (this.ragdoll.part == 0)
							{
								if (part == 1)
								{
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 2, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								}
								else
								{
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 5, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 1, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								}
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 4, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							}
							else
							{
								if (part == 1)
								{
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 12, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								}
								else
								{
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 11, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								}
							}
						}
						part = 3;
					}
					else if (part == 3)
					{
						part = Rando.Int(4, 7);
						if (flag)
						{
							if (this.ragdoll.part == 0)
							{
								if (part == 4)
								{
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 7, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								}
								else
								{
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 6, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 7, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 9, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
									Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 10, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
									if (part == 6)
									{
										Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 8, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
									}
								}
							}
							else
							{
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 13, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 14, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							}
						}
					}
					else if (part == 4)
					{
						if (flag)
						{
							if (this.ragdoll.part == 0)
							{
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 8, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 9, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 10, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							}
							else
							{
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 14, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							}
						}
						part = 7;
					}
					else if (part == 6)
					{
						if (flag)
						{
							if (this.ragdoll.part == 0)
							{
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 4, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 6, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 3, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							}
							else
							{
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 13, bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							}
						}
						part = 7;
					}
					else if (part == this._sprite.frame && part < 8)
					{
						part++;
					}
					this._sprite.frame = part;
					if (BrutalOptionsData.bloodcolor == BrutalDG.blood.Count)
					{
						this._blood.color = BrutalDG.blood[Rando.Int(BrutalDG.blood.Count - 1)];
					}
					if (flag)
					{
						for (int i = 0; i < 2; i++)
						{
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, Rando.Int(13, 25), bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
						}
					}
					if (this.ragdoll._doll.inSleepingBag && Rando.Int(4) > 2)
					{
						if (part < 8)
						{
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, Rando.Int(14, 24), bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f)), true));
						}
						else
                        {
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, Rando.Int(14, 40), bullet.travelDirNormalized * 3f + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f)), true));
						}
					}
				}
				else
				{
					if (flag)
					{
						if (this.ragdoll.part == 0)
						{
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, Maths.AngleToVec(-20) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 1, Maths.AngleToVec(2) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 2, Maths.AngleToVec(30) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 3, Maths.AngleToVec(-90) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 5, Maths.AngleToVec(-10) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 6, Maths.AngleToVec(21) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 8, Maths.AngleToVec(81) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 9, Maths.AngleToVec(90) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 10, Maths.AngleToVec(109) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							for (int i = 0; i < 360; i += 40)
							{
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, Rando.Int(13, 25), Maths.AngleToVec(Maths.DegToRad(i)) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							}
							if (this.ragdoll._doll.inSleepingBag)
							{
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, Maths.AngleToVec(-40) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f)), true));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, Maths.AngleToVec(50) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f)), true));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 17, Maths.AngleToVec(23) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f)), true));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 17, Maths.AngleToVec(-90) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f)), true));
							}
						}
						else
						{
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 11, Maths.AngleToVec(102) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 12, Maths.AngleToVec(283) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 14, Maths.AngleToVec(3) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 13, Maths.AngleToVec(78) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, Maths.AngleToVec(-49) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							for (int i = 0; i < 360; i += 40)
							{
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, Rando.Int(13, 25), Maths.AngleToVec(Maths.DegToRad(i)) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f))));
							}
							if (this.ragdoll._doll.inSleepingBag)
							{
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, Maths.AngleToVec(122) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f)), true));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 0, Maths.AngleToVec(233) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f)), true));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 17, Maths.AngleToVec(23) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f)), true));
								Level.Add(new DuckGib(hitPos.x, hitPos.y, this.ragdoll._persona, 17, Maths.AngleToVec(-90) + new Vec2(Rando.Float(-1f, 1f), -Rando.Float(3f, -0.5f)), true));
							}
						}
					}
					this._sprite.frame = 7;
					if (BrutalOptionsData.bloodcolor == BrutalDG.blood.Count)
					{
						this._blood.color = BrutalDG.blood[Rando.Int(BrutalDG.blood.Count - 1)];
					}
				}
			}
		}
		
		public override void Draw()
		{
			if (this._sprite == null || this._sprite.texture == null)
            {
				Level.Remove(this);
				return;
            }
			if (this.ragdoll != null && this.ragdoll.isInitialized && this._sprite != null)
			{
				if (this.ragdoll._doll != null)
				{
					if (this.ragdoll._doll.inSleepingBag && this._didrecreate)
					{
						string text = "ragdollparts1camp";
						if (this.ragdoll.part != 0)
							text = "ragdollparts2camp";
						if (this._sprite.texture != Content.Load<Tex2D>(GetPath(text)))
						{
							this._sprite.texture = Content.Load<Tex2D>(GetPath(text));
						}
						if (this._over != null)
						{
							this._over.texture = Content.Load<Tex2D>(GetPath("ragdollparts1campoverlay"));
						}
						//this._didrecreate = false;
					}
					if (this._blood != null)
					{
						if (this.ragdoll._doll.inSleepingBag && this.ragdoll.part == 0)
						{
							if (this._sprite.frame < 9)
							{
								this._blood.texture = Content.Load<Tex2D>(GetPath("ragdollparts1campblood"));
							}
							else
                            {
								this._blood.texture = BrutalDG._blank;
							}
						}
						else if (this.ragdoll._doll.inSleepingBag && this.ragdoll.part != 0)
						{
							this._blood.texture = BrutalDG._blank;
						}
						else
                        {
							this._blood.texture = Content.Load<Tex2D>(GetPath("ragdollparts1blood"));
							if (this.ragdoll.part != 0)
							{
								this._blood.texture = Content.Load<Tex2D>(GetPath("ragdollparts2blood"));
							}
						}
					}
				}
				this.ragdoll.layer = RagdollParts._ragdollLayer;
				if (this._blood == null)
				{
					this._blood = new SpriteMap(GetPath("ragdollparts1blood"), 32, 32);
					if (this.ragdoll.part != 0)
					{
						this._blood = new SpriteMap(GetPath("ragdollparts2blood"), 32, 32);
					}
					this._sprite.center = this.ragdoll.graphic.center;
					this._blood.center = this._sprite.center;
				}
				if (this.ragdoll.part == 0)
				{
					if (this.ragdoll.doll.captureDuck != null)
					{
						if (this.ragdoll.doll.captureDuck.IsQuacking())
						{
							try
							{
								Vec2 vec = (Vec2)typeof(RagdollPart).GetField("_stickLerp", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(this.ragdoll);
								if (vec.length > 0f)
								{
									this._doOverlay = true;
								}
								else
								{
									this._doOverlay = false;
								}
							}
							catch { }

							if (this.ragdoll.doll.captureDuck.eyesClosed)
							{
								if (this._sprite.frame < 9)
								{
									this._sprite.frame += 27;
								}
								else if (this._sprite.frame >= 9 && this._sprite.frame < 18)
								{
									this._sprite.frame += 18;
								}
								else if (this._sprite.frame >= 18 && this._sprite.frame < 27)
								{
									this._sprite.frame += 9;
								}
								else if (this._sprite.frame >= 36 && this._sprite.frame < 45)
								{
									this._sprite.frame -= 9;
								}
								else if (this._sprite.frame >= 45 && this._sprite.frame < 54)
								{
									this._sprite.frame -= 18;
								}
							}
							else
							{
								if (this._sprite.frame < 9)
								{
									this._sprite.frame += 18;
								}
								else if (this._sprite.frame >= 9 && this._sprite.frame < 18)
								{
									this._sprite.frame += 9;
								}
								else if (this._sprite.frame >= 27 && this._sprite.frame < 36)
								{
									this._sprite.frame -= 9;
								}
								else if (this._sprite.frame >= 36 && this._sprite.frame < 45)
								{
									this._sprite.frame -= 18;
								}
								else if (this._sprite.frame >= 45 && this._sprite.frame < 54)
								{
									this._sprite.frame -= 27;
								}
							}
						}
						else
						{
							if (this.ragdoll.doll.captureDuck.eyesClosed)
							{
								if (this._sprite.frame < 9)
								{
									this._sprite.frame += 9;
								}
								else if (this._sprite.frame >= 18 && this._sprite.frame < 27)
								{
									this._sprite.frame -= 9;
								}
								else if (this._sprite.frame >= 27 && this._sprite.frame < 36)
								{
									this._sprite.frame -= 18;
								}
								else if (this._sprite.frame >= 36 && this._sprite.frame < 45)
								{
									this._sprite.frame -= 27;
								}
								else if (this._sprite.frame >= 45 && this._sprite.frame < 54)
								{
									this._sprite.frame -= 36;
								}
							}
							else
							{
								if (this._sprite.frame >= 9 && this._sprite.frame < 18)
								{
									this._sprite.frame -= 9;
								}
								else if (this._sprite.frame >= 18 && this._sprite.frame < 27)
								{
									this._sprite.frame -= 18;
								}
								else if (this._sprite.frame >= 27 && this._sprite.frame < 36)
								{
									this._sprite.frame -= 27;
								}
								else if (this._sprite.frame >= 36 && this._sprite.frame < 45)
								{
									this._sprite.frame -= 36;
								}
								else if (this._sprite.frame >= 45 && this._sprite.frame < 54)
								{
									this._sprite.frame -= 45;
								}
							}
						}
					}
					int num = this._sprite.frame;
					if (num > 0 && num < 9)
					{
						this._blood.frame = num - 1;
					}
					else if (num > 9 && num < 18)
					{
						this._blood.frame = num - 10;
					}
					if (num > 18 && num < 27)
					{
						this._blood.frame = num - 11;
					}
					else if (num > 27 && num < 36)
					{
						this._blood.frame = num - 20;
					}
				}
				else if (this._sprite.frame > 0)
				{
					this._blood.frame = this._sprite.frame - 1;
				}
				bool flag = true;
				if (this._sprite.frame == 0 || this._sprite.frame == 9 || this._sprite.frame == 18 || this._sprite.frame == 27)
				{
					flag = false;
				}
				this._blood.angleDegrees = this.ragdoll.angleDegrees;
				this._sprite.angleDegrees = this.ragdoll.angleDegrees;
				this._sprite.flipH = (this.ragdoll.offDir < 0);//this.ragdoll.graphic.flipH;
				this._blood.flipH = (this.ragdoll.offDir < 0);//this.ragdoll.graphic.flipH;
				if (this._sprite != null && this._sprite.texture != null && this.ragdoll.visible)
				{
					if (this._over != null)
                    {
						if (!this.ragdoll._doll.inSleepingBag)
						{
							if (this._sprite.frame >= 18 && this._sprite.frame < 27)
							{
								this._over.frame = this._sprite.frame - 18;
								this._doOverlay = true;
							}
							else if (this._sprite.frame >= 27 && this._sprite.frame < 36)
							{
								this._over.frame = this._sprite.frame - 18;
								this._doOverlay = true;
							}
							else if (this._sprite.frame >= 36 && this._sprite.frame < 45)
							{
								this._over.frame = this._sprite.frame - 18;
								this._doOverlay = true;
							}
							else if (this._sprite.frame >= 45 && this._sprite.frame < 54)
							{
								this._over.frame = this._sprite.frame - 18;
								this._doOverlay = true;
							}
							else
                            {
								this._doOverlay = false;
							}
						}
						else
						{
							if (this._sprite.frame >= 18 && this._sprite.frame < 27)
                            {
                                this._over.frame = this._sprite.frame - 18;
								this._doOverlay = true;
							}
							else
                            {
								this._doOverlay = false;
                            }
						}
					}
					if (this._over != null && this._doOverlay)
					{
						this._over.flipH = this._sprite.flipH;
						this._over.center = this._sprite.center;
						this._over.angleDegrees = this.ragdoll.angleDegrees;
						int num = 2;
						if (this.ragdoll._doll.inSleepingBag)
							num = 1;
						Graphics.Draw(this._over, this.ragdoll.x, this.ragdoll.y, this.ragdoll.depth.Add(num));
						//this.ragdoll.depth = this.ragdoll.depth.Add(-1);
						this.ragdoll.Draw();
						//this.ragdoll.depth = this.ragdoll.depth.Add(1);
					}
					Graphics.Draw(this._sprite, this.ragdoll.x, this.ragdoll.y, this.ragdoll.depth);
				}
				if (flag && this._blood != null && this._blood.texture != null && this.ragdoll.visible)
				{
					Graphics.Draw(this._blood, this.ragdoll.x, this.ragdoll.y, (Depth)(this.ragdoll.depth.value + 0.05f));
				}
				
			}
			//Graphics.DrawRect(this.ragdoll.topLeft, this.ragdoll.bottomRight, Color.Red * 0.1f, 2f, false, 1f);
			base.Draw();
		}
		
		public void Recreate()
		{
			//if (Graphics.inFocus && this.ragdoll != null && this.ragdoll._persona != null && !Graphics.screen.IsDisposed)
			//{
			//	for (int i = 0; i < 4; i++)
			//	{
			//		string text = "ragdollparts1";
			//		if (this.ragdoll.part != 0)
			//			text = "ragdollparts2";
			//		SpriteMap sprite = new SpriteMap(GetPath(text), 32, 32);
			//		this._sprite.texture.Dispose();
			//		this._sprite.texture = Graphics.Recolor(sprite.texture, this.ragdoll._persona.color);
			//		if (this._over != null)
			//		{
			//			this._over.texture.Dispose();
			//			this._over.texture = Graphics.Recolor(new SpriteMap(GetPath("ragdollparts1overlay"), 32, 32).texture, this.ragdoll._persona.color);
			//		}
			//		this._didrecreate = true;
			//	}
			//}
		}
		
		public static RagdollParts GetRagdollParts(RagdollPart part)
		{
			if (part != null)
			{
				foreach (RagdollParts parts in Level.current.things[typeof(RagdollParts)])
				{
					if (parts.ragdoll == part)
					{
						return parts;
					}
				}
			}
			return null;
		}
		
		private List<Sword> _swords = new List<Sword>();
		
		public SpriteMap _sprite;
		
		public SpriteMap _blood;
		
		public RagdollPart ragdoll;
		
		public int timer;
		
		public float _swordtimer;

		public static Layer _ragdollLayer = new Layer("ragdolls", 0, null, false, Vec2.Zero);

		private bool _didrecreate;

		public SpriteMap _over;

		private bool _doOverlay;

		private Tex2D _prevTex;
	}
}
