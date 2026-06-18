using System;
using DuckGame;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace DuckGame.BrutalDG
{
	internal class KetchupParticle : PhysicsParticle
	{
		public KetchupParticle child
		{
			get
			{
				return this._child;
			}
			set
			{
				this._child = value;
			}
		}

		public KetchupParticle(float xpos, float ypos, Vec2 hitAngle, FluidData dat, KetchupParticle stream = null, float thickMult = 1f) : base(xpos, ypos)
		{
			this.hSpeed = -hitAngle.x * 2f * (Rando.Float(1f) + 0.3f);
			this.vSpeed = -hitAngle.y * 2f * (Rando.Float(1f) + 0.3f) - Rando.Float(2f);
			this.hSpeed = hitAngle.x;
			this.vSpeed = hitAngle.y;
			this._bounceEfficiency = 0.6f;
			this._stream = stream;
			if (stream != null)
			{
				stream.child = this;
			}
			this._canStick = Rando.Int(2) == 1;
			base.alpha = 1f;

			this._gravMult = 2f;
			base.depth = -0.5f;
			this.data = dat;
			this._thickMult = thickMult;
			this._thickness = Maths.Clamp(this.data.amount * 600f, 0.2f, 8f) * this._thickMult;
			this.startThick = this._thickness;
			this._glob = new SpriteMap("bigGlob", 8, 8, false);
		}

		public override void Update()
		{
			if (Level.current.things[typeof(KetchupParticle)].Count() > 200)
			{
				this._canStick = Rando.Int(30) > 15;
			}
			if (BrutalDG.bloodguns && this._canStick)
			{
				FluidPuddle puddle = Level.CheckPoint<FluidPuddle>(this.position, null, null);
				Holdable thing1 = Level.CheckPoint<Holdable>(this.position, null, null);
				bool flag = false;
				if (puddle != null && puddle.data.color == Fluid.Water.color && puddle.data.sprite == Fluid.Water.sprite)
				{
					flag = true;
				}
				if (thing1 != null && BloodThings.GetThing(thing1) == null && thing1.visible && !(thing1 is RagdollPart))
				{
					Level.Add(new BloodThings(thing1));
				}
				else if (thing1 != null && thing1.visible && !(thing1 is RagdollPart) && BloodThings.GetThing(thing1) != null && !flag)
				{
					BloodThings.GetThing(thing1)._amount++;
					BrutalDG.MakeDecals(thing1);
					Level.Remove(this);
				}
			}
			float num1 = BrutalOptionsData.bloodamount;
			if (num1 == 0f)
			{
				num1 = 0.08f;
			}
			if (!Network.isActive)
			{
				num1 *= 1.6f;
			}
			if (Level.current.things[typeof(KetchupParticle)].Count() > (1200 * num1) && this._stream == null)
			{
				if (Rando.Int(20) > 16)
				{
					Level.Remove(this);
					return;
				}
			}
			if (!BrutalOptionsData.enableblood)
			{
				Level.Remove(this);
			}
			this._life = 1f;
			if (this._thickness < 4f || Math.Abs(this.vSpeed) < 1.5f)
			{
				this.live -= 0.01f;
			}
			this._thickness = Lerp.FloatSmooth(this.startThick, 0.1f, 1f - this.live, 1f);
			if ((this.live < 0f || (this._grounded && Math.Abs(this.vSpeed) < 0.1f)))
			{
				Level.Remove(this);
				this.active = false;
				KetchupPuddle fluidPuddle = null;
				foreach (Thing thing in Level.current.things[typeof(KetchupPuddle)])
				{
					KetchupPuddle fluidPuddle2 = (KetchupPuddle)thing;
					if (base.x > fluidPuddle2.left && base.x < fluidPuddle2.right && Math.Abs(fluidPuddle2.y - base.y) < 10f)
					{
						fluidPuddle = fluidPuddle2;
						break;
					}
				}
				if (fluidPuddle == null)
				{
					Vec2 vec;
					Block block = Level.CheckLine<AutoBlock>(this.position + new Vec2(0f, -8f), this.position + new Vec2(0f, 16f), out vec, null);
					if (block != null && vec.y == block.top)
					{
						fluidPuddle = new KetchupPuddle(vec.x, vec.y, block);
						fluidPuddle.depth = this.depth + 3;
						Level.Add(fluidPuddle);
					}
				}
				if (fluidPuddle != null)
				{
					fluidPuddle.Feed(this.data);
				}
				return;
			}
			BloodBlocks block1 = Level.CheckCircle<BloodBlocks>(this.position, 4f, null);
			if (block1 != null && this._stream == null && this._canStick && Rando.Float(100) > 92f)
			{
				this.MakeDecals(block1);
			}
			//
			if (!this.isLocal)
			{
				Vec2 position = this.position;
				Vec2 vec = this.netLerpPosition;
				if ((position - vec).lengthSq > 2048f || (position - vec).lengthSq < 1f)
				{
					this.position = vec;
					return;
				}
				this.position = Lerp.Vec2Smooth(position, vec, 0.2f);
				return;
			}
			else
			{
				if (Network.isActive && (base.y < -200f || base.y > Level.current.lowestPoint + 200f))
				{
					Level.Remove(this);
					return;
				}
				this._hit = false;
				this._touchedFloor = false;
				this._framesAlive += 1f;
				if (!this.onlyDieWhenGrounded || this._grounded || this._framesAlive > 400f)
				{
					this._life -= 0.005f;
					if (this._life < 0f)
					{
						base.alpha -= 0.1f;
						if (base.alpha < 0f)
						{
							Level.Remove(this);
						}
					}
				}
				if (this._foreverGrounded)
				{
					this._grounded = true;
					if (Rando.Float(250f) < 1f - this._sticky)
					{
						this._foreverGrounded = false;
						this._grounded = false;
						this.hSpeed = -this._stickDir * Rando.Float(0.8f);
					}
				}
				if (!this._grounded)
				{
					if (this.hSpeed > 0f)
					{
						this.hSpeed -= this._airFriction;
					}
					if (this.hSpeed < 0f)
					{
						this.hSpeed += this._airFriction;
					}
					if (this.hSpeed < this._airFriction && this.hSpeed > -this._airFriction)
					{
						this.hSpeed = 0f;
					}
					if (this.vSpeed < 4f)
					{
						this.vSpeed += 0.1f * this._gravMult;
					}
					if (float.IsNaN(this.hSpeed))
					{
						this.hSpeed = 0f;
					}
					this._spinAngle -= (float)(10 * Math.Sign(this.hSpeed));
					Thing thing = Level.CheckPoint<Block>(base.x + this.hSpeed, base.y + this.vSpeed, null, null);
					if (thing != null && this._framesAlive < 2f)
					{
						this._waitForNoCollide = true;
					}
					if (thing != null && this._waitForNoCollide)
					{
						thing = null;
					}
					else if (thing == null && this._waitForNoCollide)
					{
						this._waitForNoCollide = false;
					}
					if (thing != null)
					{
						this._touchedFloor = true;
						if (this._bounceSound != "" && (Math.Abs(this.vSpeed) > 1f || Math.Abs(this.hSpeed) > 1f))
						{
							SFX.Play(this._bounceSound, 0.5f, -0.1f + Rando.Float(0.2f), 0f, false);
						}
						if (this.vSpeed > 0f && thing.top > base.y)
						{
							this.vSpeed = -(this.vSpeed * this._bounceEfficiency);
							this._hit = true;
							if (Math.Abs(this.vSpeed) < 0.5f)
							{
								this.vSpeed = 0f;
								this._grounded = true;
							}
						}
						else if (this.vSpeed < 0f && thing.bottom < base.y)
						{
							this.vSpeed = -(this.vSpeed * this._bounceEfficiency);
							this._hit = true;
						}
						if (this.hSpeed > 0f && thing.left > base.x)
						{
							this.hSpeed = -(this.hSpeed * this._bounceEfficiency);
							this._hit = true;
							if (this._sticky > 0f && Rando.Float(1f) < this._sticky)
							{
								this.hSpeed = 0f;
								this.vSpeed = 0f;
								this._foreverGrounded = true;
								this._stickDir = 1f;
							}
						}
						else if (this.hSpeed < 0f && thing.right < base.x)
						{
							this.hSpeed = -(this.hSpeed * this._bounceEfficiency);
							this._hit = true;
							if (this._sticky > 0f && Rando.Float(1f) < this._sticky)
							{
								this.hSpeed = 0f;
								this.vSpeed = 0f;
								this._foreverGrounded = true;
								this._stickDir = -1f;
							}
						}
						if (!this._hit)
						{
							this._grounded = true;
						}
					}
					else
					{
						base.x += this.hSpeed;
						base.y += this.vSpeed;
					}
				}
				if (this._spinAngle > 360f)
				{
					this._spinAngle -= 360f;
				}
				if (this._spinAngle < 0f)
				{
					this._spinAngle += 360f;
				}
			}
			//
			if (this._touchedFloor && !this._firstHit)
			{
				this._firstHit = true;
				this.hSpeed += Rando.Float(-1f, 1f);
				this.hSpeed *= Rando.Float(-1f, 1.5f);
				this.vSpeed *= Rando.Float(0.3f, 1f);
			}
			if (this._stream != null)
			{
				float num = Math.Abs(this.hSpeed - this._stream.hSpeed);
				if (Math.Abs(base.x - this._stream.x) * num > 40f || Math.Abs(this.vSpeed - this._stream.vSpeed) > 1.9f || num > 1.9f)
				{
					this.BreakStream();
				}
			}
		}

		public void BreakStream()
		{
			if (this._child != null)
			{
				this._child._stream = null;
			}
			this._child = null;
			if (this._stream != null)
			{
				this._stream._child = null;
			}
			this._stream = null;
		}

		public override void Draw()
		{
			if (this._stream != null)
			{
				Graphics.currentDrawIndex++;
				Graphics.DrawLine(this.position, this._stream.position, new Color(this.data.color) * base.alpha, this._thickness, base.depth);
				return;
			}
			if (this._child == null)
			{
				if (this._thickness > 4f)
				{
					this._glob.depth = base.depth;
					this._glob.frame = 2;
					this._glob.color = new Color(this.data.color) * base.alpha;
					this._glob.CenterOrigin();
					this._glob.angle = Maths.DegToRad(-Maths.PointDirection(this.position, this.position + base.velocity) + 90f);
					Graphics.Draw(this._glob, base.x, base.y);
					return;
				}
				Graphics.DrawRect(this.position - new Vec2(this._thickness / 2f, this._thickness / 2f), this.position + new Vec2(this._thickness / 2f, this._thickness / 2f), new Color(this.data.color) * base.alpha, base.depth, true, 1f);
			}
		}

		private void MakeDecals(BloodBlocks block1)
		{
			if (block1 != null && block1._block != null && !(block1._block is PurpleBlock) && (block1._block is BackgroundTile || !(block1._block is BackgroundTile)))
			{
				Vec2 pos = new Vec2((float)Math.Round((double)(this.position.x / 1f)) * 1f, (float)Math.Round((double)(this.position.y / 1f)) * 1f);
				if (block1._block.graphic != null && block1._block.graphic.texture != null && !block1._pos.Contains(block1.position - block1.collisionOffset - pos) && (block1._allowedPos.Count > 0 || block1._topPos.Count > 0 || block1._leftPos.Count > 0 || block1._rightPos.Count > 0 || block1._bottomPos.Count > 0))
				{
					float topp = Vec2.Distance(this.position, block1.topLeft + new Vec2(8f, 0f));
					float leftp = Vec2.Distance(this.position, block1.topLeft + new Vec2(0f, 8f));
					float rightp = Vec2.Distance(this.position, block1.topRight + new Vec2(0f, 8f));
					float bottomp = Vec2.Distance(this.position, block1.bottomLeft + new Vec2(8f, 0f));
					bool doTop = false;
					bool doLeft = false;
					bool doRight = false;
					bool doBottom = false;
					List<Vec2> _allowed = new List<Vec2>();
					if (topp < leftp && topp < rightp && topp < bottomp && block1._topPos.Count > 0)
                    {
						_allowed = block1._topPos;
						doTop = true;
                    }
					else if (leftp < topp && leftp < rightp && leftp < bottomp && block1._leftPos.Count > 0)
					{
						_allowed = block1._leftPos;
						doLeft = true;
					}
					else if (rightp < topp && rightp < leftp && rightp < bottomp && block1._rightPos.Count > 0)
					{
						_allowed = block1._rightPos;
						doRight = true;
					}
					else if (bottomp < topp && bottomp < leftp && bottomp < rightp && block1._bottomPos.Count > 0)
					{
						_allowed = block1._bottomPos;
						doBottom = true;
					}
					else
                    {
						if (block1._allowedPos.Count == 0)
							return;
						_allowed = block1._allowedPos;
					}
					Thing t = block1._block;
					System.IO.MemoryStream stream = new System.IO.MemoryStream();
					Microsoft.Xna.Framework.Graphics.Texture2D tex = t.graphic.texture;
					//if (block1.graphic.texture != null && block1.visible)
					//{
					//	tex = block1.graphic.texture;
					//	if (block1.graphic.texture.width != t.graphic.texture.width || block1.graphic.texture.height != t.graphic.texture.height || block1.graphic.width != t.graphic.width || block1.graphic.height != t.graphic.height)
					//	{
					//		return;
					//	}
					//}
					tex.SaveAsPng(stream, tex.Width, tex.Height);
					Bitmap bmp = new Bitmap(stream);
					stream.Close();
					stream.Dispose();
					if (bmp != null)
					{
						using (var bitmap = new Bitmap(bmp))
						{
							int num1 = 0;
							Vec2 _from = Vec2.Zero;
							for (int y = 0; y < t.graphic.texture.height / t.graphic.height; y++)
							{
								for (int x = 0; x < t.graphic.texture.width / t.graphic.width; x++)
								{
									if ((t.graphic as SpriteMap).frame == x + num1)
									{
										_from = new Vec2(x, y);
									}
								}
								num1 += 8;
							}
							for (int i = 0; i < Rando.Int(10, 16); i++)
							{
								if (_allowed.Count > 0)
								{
									int num = Rando.Int(_allowed.Count - 1);
									for (int x = 0; x < t.graphic.texture.width / t.graphic.width; x++)
									{
										for (int y = 0; y < t.graphic.texture.height / t.graphic.height; y++)
										{
											bool flag = new Vec2(x, y) == _from;
											if (t is ItemBox)
											{
												flag = true;
											}
											if (flag)
											{
												try
												{
													System.Drawing.Color c = bitmap.GetPixel((int)_allowed[num].x + x * t.graphic.width, (int)_allowed[num].y + y * t.graphic.height);
													if (c != null && c.A > 100 && c.GetBrightness() > 0.1f && !block1._pos.Contains(new Vec2((int)_allowed[num].x + x * t.graphic.width, (int)_allowed[num].y + y * t.graphic.height)))
													{
														block1._pos.Add(new Vec2((int)_allowed[num].x + x * t.graphic.width, (int)_allowed[num].y + y * t.graphic.height));
														Color color = new Color(240, 48, 48);
														if (BrutalOptionsData.bloodcolor < BrutalDG.blood.Count)
														{
															color = BrutalDG.blood[BrutalOptionsData.bloodcolor];
														}
														if (BrutalOptionsData.bloodcolor == BrutalDG.blood.Count)
														{
															color = BrutalDG.blood[Rando.Int(BrutalDG.blood.Count - 1)];
														}
														float r = color.r;
														float g = color.g;
														float b = color.b;
														if (BrutalOptionsData.bloodcolor != BrutalDG.blood.Count)
														{
															r *= c.GetBrightness();
															g *= c.GetBrightness();
															b *= c.GetBrightness();
														}
														color.r = (byte)r;
														color.g = (byte)g;
														color.b = (byte)b;

														KetchupPixelBlock pixel = new KetchupPixelBlock(t, new Vec2((int)_allowed[num].x, (int)_allowed[num].y), color, block1);
														if (!block1._ketchup.Contains(pixel))
														{
															block1._ketchup.Add(pixel);
														}
														//block1._blood.Add(new Vec2(x, y));
														//System.Drawing.Color color1 = System.Drawing.Color.FromArgb(color.a, color.r, color.g, color.b);
														//bitmap.SetPixel((int)_allowed[num].x + x * t.graphic.width, (int)_allowed[num].y + y * t.graphic.height, color1);
													}
												}
												catch {	}
											}
										}
									}
									_allowed.Remove(_allowed[num]);
								}
							}
							if (doLeft)
                            {
								block1._leftPos = _allowed;
							}
							else if (doRight)
							{
								block1._rightPos = _allowed;
							}
							else if (doTop)
							{
								block1._topPos = _allowed;
							}
							else if (doBottom)
							{
								block1._bottomPos = _allowed;
							}
							else
                            {
								block1._allowedPos = _allowed;
							}
							//BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
							//bool process = Rando.Int(3) > 1;
							//int w = 0;
							//int num3 = bitmapData.Width * bitmapData.Height;
							//int* ptr = (int*)((void*)bitmapData.Scan0);
							//while (w < num3)
							//{
							//	if (process && *ptr == -65281)
							//	{
							//		*ptr = 0;
							//	}
							//	else
							//	{
							//		byte* ptr2 = (byte*)ptr;
							//		byte b = *ptr2;
							//		*ptr2 = ptr2[2];
							//		ptr2[2] = b;
							//		float num2 = (float)ptr2[3] / 255f;
							//		for (int j = 0; j < 3; j++)
							//		{
							//			ptr2[j] = (byte)((float)ptr2[j] * num2);
							//		}
							//	}
							//	w++;
							//	ptr++;
							//}
							//int[] array = new int[bitmapData.Width * bitmapData.Height];
							//System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, array, 0, array.Length);
							//Microsoft.Xna.Framework.Graphics.Texture2D texture2D = new Microsoft.Xna.Framework.Graphics.Texture2D(Graphics.device, bitmapData.Width, bitmapData.Height);
							//texture2D.SetData<int>(array);
							//block1._sprite = new SpriteMap("null", 16, 16);
							//block1._sprite.texture = texture2D;
							//block1.graphic.texture = texture2D;
							//block1.visible = true;
							//bitmap.UnlockBits(bitmapData);
							Level.Remove(this);
							return;
						}
					}
				}
			}
		}

		private KetchupParticle _stream;

		private KetchupParticle _child;

		private bool _firstHit;

		private float _thickness;

		public FluidData data;

		private float _thickMult;

		public SpriteMap _glob;

		private float startThick;

		private float live = 1f;
		
		private bool _canStick;
		
		private float _framesAlive;
		
		private bool _waitForNoCollide;
	}
}
