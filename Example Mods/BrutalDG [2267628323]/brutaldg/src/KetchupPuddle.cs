using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.BrutalDG
{
	internal class KetchupPuddle : MaterialThing
	{
		public KetchupPuddle(float xpos, float ypos, Block b) : base(xpos, ypos)
		{
			this._collisionOffset.y = -4f;
			this._collisionSize.y = 1f;
			this._block = b;
			base.depth = 3.9f;
			this.flammable = 0f;
			base.alpha = 0f;
			List<BlockCorner> groupCorners = b.GetGroupCorners();
			this._leftCorner = null;
			this._rightCorner = null;
			foreach (BlockCorner blockCorner in groupCorners)
			{
				if (Math.Abs(ypos - blockCorner.corner.y) < 4f)
				{
					if (blockCorner.corner.x > xpos)
					{
						if (this._rightCorner == null)
						{
							this._rightCorner = blockCorner;
						}
						else if (blockCorner.corner.x < this._rightCorner.corner.x)
						{
							this._rightCorner = blockCorner;
						}
					}
					else if (blockCorner.corner.x < xpos)
					{
						if (this._leftCorner == null)
						{
							this._leftCorner = blockCorner;
						}
						else if (blockCorner.corner.x > this._leftCorner.corner.x)
						{
							this._leftCorner = blockCorner;
						}
					}
				}
			}
		}

		public override void Initialize()
		{
			if (this._leftCorner == null || this._rightCorner == null)
			{
				Level.Remove(this);
				return;
			}
			base.y = this._leftCorner.corner.y;
		}

		public void Feed(FluidData dat)
		{
			if (this._lava == null && dat.sprite != "" && dat.sprite != null)
			{
				if (this.data.sprite == null)
				{
					this.data.sprite = dat.sprite;
				}
				this._lava = new SpriteMap(dat.sprite, 16, 16, false);
				this._lava.AddAnimation("idle", 0.1f, true, new int[]
				{
					0,
					1,
					2,
					3
				});
				this._lava.SetAnimation("idle");
				this._lava.center = new Vec2(8f, 10f);
				this._lavaAlternate = new SpriteMap(dat.sprite, 16, 16, false);
				this._lavaAlternate.AddAnimation("idle", 0.1f, true, new int[]
				{
					2,
					3,
					0,
					1
				});
				this._lavaAlternate.SetAnimation("idle");
				this._lavaAlternate.center = new Vec2(8f, 10f);
			}
			if (this._lightRect == null && Layer.lighting)
			{
				this._lightRect = new WhiteRectangle(base.x, base.y, this.width, this.height, dat.heat <= 0f);
				Level.Add(this._lightRect);
			}
			if (dat.amount > 0f)
			{
				this._framesSinceFeed = 0;
			}
			this.data.Mix(dat);
			this.data.amount = Maths.Clamp(this.data.amount, 0f, this.MaxFluidFill());
			this._wide = this.FeedAmountToDistance(this.data.amount);
			float num = this._wide + 4f;
			this._collisionOffset.x = -(num / 2f);
			this._collisionSize.x = num;
			this.FeedEdges();
			if (this._leftCorner != null && this._rightCorner != null && this._wide > this._rightCorner.corner.x - this._leftCorner.corner.x)
			{
				this._wide = this._rightCorner.corner.x - this._leftCorner.corner.x;
				base.x = this._leftCorner.corner.x + (this._rightCorner.corner.x - this._leftCorner.corner.x) / 2f;
			}
			num = this._wide + 4f;
			this._collisionOffset.x = -(num / 2f);
			this._collisionSize.x = num;
		}

		public float DistanceToFeedAmount(float distance)
		{
			return distance / 600f;
		}
		
		public float FeedAmountToDistance(float feed)
		{
			return feed * 600f;
		}

		public float MaxFluidFill()
		{
			if (this._topLeftCorner != null && this._topRightCorner != null)
			{
				float num = this._topLeftCorner.corner.y + 8f;
				if (this._topRightCorner.corner.y > num)
				{
					num = this._topRightCorner.corner.y + 8f;
				}
				return this.DistanceToFeedAmount((this._leftCorner.corner.y - num) * this._collisionSize.x);
			}
			return 999999f;
		}

		public void FeedEdges()
		{
			if (this._rightCorner != null && this.right > this._rightCorner.corner.x && this._rightCorner.wallCorner)
			{
				base.x -= this.right - this._rightCorner.corner.x;
			}
			if (this._leftCorner != null && this.left < this._leftCorner.corner.x && this._leftCorner.wallCorner)
			{
				base.x += this._leftCorner.corner.x - this.left;
			}
			if (this._rightCorner != null && this.right > this._rightCorner.corner.x && !this._rightCorner.wallCorner && Level.current.things[typeof(KetchupStream)].Count() < 5)
			{
				float val = this.DistanceToFeedAmount(this.right - this._rightCorner.corner.x);
				base.x -= (this.right - this._rightCorner.corner.x) / 2f;
				if (this._rightStream == null)
				{
					this._rightStream = new KetchupStream(this._rightCorner.corner.x - 2f, base.y, new Vec2(1f, 0f), 1f, default(Vec2));
				}
				this._rightStream.position.y = base.y - this._collisionOffset.y;
				this._rightStream.position.x = this._rightCorner.corner.x + 2f;
				this._rightStream.Feed(this.data.Take(val));
			}
			this._wide = this.FeedAmountToDistance(this.data.amount);
			float num = this._wide + 4f;
			this._collisionOffset.x = -(num / 2f);
			this._collisionSize.x = num;
			if (this._leftCorner != null && this.left < this._leftCorner.corner.x && !this._leftCorner.wallCorner && Level.current.things[typeof(KetchupStream)].Count() < 5)
			{
				float val2 = this.DistanceToFeedAmount(this._leftCorner.corner.x - this.left);
				base.x += (this._leftCorner.corner.x - this.left) / 2f;
				if (this._leftStream == null)
				{
					this._leftStream = new KetchupStream(this._leftCorner.corner.x - 2f, base.y, new Vec2(-1f, 0f), 1f, default(Vec2));
				}
				this._leftStream.position.y = base.y - this._collisionOffset.y;
				this._leftStream.position.x = this._leftCorner.corner.x - 2f;
				this._leftStream.Feed(this.data.Take(val2));
			}
			this._wide = this.FeedAmountToDistance(this.data.amount);
			num = this._wide + 4f;
			this._collisionOffset.x = -(num / 2f);
			this._collisionSize.x = num;
		}

		public float CalculateDepth()
		{
			float num = this.FeedAmountToDistance(this.data.amount);
			if (this._wide == 0f)
			{
				this._wide = 0.001f;
			}
			return Maths.Clamp(num / this._wide, 1f, 99999f);
		}

		public override void Update()
		{
			if (this.collisionSize.y > 2f)
			{
				this.data.amount -= 0.01f;
			}
			if (this._block.removeFromLevel || !BrutalOptionsData.enableblood)
			{
				Level.Remove(this);
			}
			this._framesSinceFeed++;
			this.fluidWave += 0.1f;
			if (this.data.amount < 0.0001f)
			{
				Level.Remove(this);
			}
			if (this.collisionSize.y > 10f)
			{
				this.bubbleWait++;
				if (this.bubbleWait > Rando.Int(15, 25))
				{
					for (int i = 0; i < (int)Math.Floor((double)(this.collisionSize.x / 16f)); i++)
					{
						if (Rando.Float(1f) > 0.85f)
						{
							Level.Add(new TinyBubble(this.left + (float)(i * 16) + Rando.Float(-4f, 4f), this.bottom + Rando.Float(-4f), 0f, this.top + 10f, false));
						}
					}
					this.bubbleWait = 0;
				}
				IEnumerable<PhysicsObject> enumerable = Level.CheckRectAll<PhysicsObject>(base.topLeft, base.bottomRight);
				foreach (PhysicsObject physicsObject in enumerable)
				{
					physicsObject.sleeping = false;
				}
			}
			KetchupPuddle fluidPuddle = Level.CheckLine<KetchupPuddle>(new Vec2(this.left, base.y), new Vec2(this.right, base.y), this);
			if (fluidPuddle != null && fluidPuddle.data.amount < this.data.amount)
			{
				fluidPuddle.active = false;
				float num = Math.Min(fluidPuddle.left, this.left);
				float num2 = Math.Max(fluidPuddle.right, this.right);
				base.x = num + (num2 - num) / 2f;
				this.Feed(fluidPuddle.data);
				Level.Remove(fluidPuddle);
			}
			if (this._leftStream != null)
			{
				this._leftStream.Update();
				this._leftStream.onFire = base.onFire;
			}
			if (this._rightStream != null)
			{
				this._rightStream.Update();
				this._rightStream.onFire = base.onFire;
			}
			float num3 = this.FeedAmountToDistance(this.data.amount);
			if (this._wide == 0f)
			{
				this._wide = 0.001f;
			}
			float num4 = Maths.Clamp(num3 / this._wide, 1f, 99999f);
			base.alpha = Lerp.Float(base.alpha, 1f, 0.04f);
			if (num4 < 3f)
			{
				FluidData dat2 = this.data;
				dat2.amount = -0.0001f;
				this.Feed(dat2);
			}
			num4 = this.CalculateDepth();
			if (num4 > 4f && !this._initializedUpperCorners)
			{
				this._initializedUpperCorners = true;
				List<BlockCorner> groupCorners = this._block.GetGroupCorners();
				foreach (BlockCorner blockCorner in groupCorners)
				{
					if (this._leftCorner != null && blockCorner.corner.x == this._leftCorner.corner.x && blockCorner.corner.y < this._leftCorner.corner.y)
					{
						if (this._topLeftCorner == null)
						{
							this._topLeftCorner = blockCorner;
						}
						else if (blockCorner.corner.y > this._topLeftCorner.corner.y)
						{
							this._topLeftCorner = blockCorner;
						}
					}
					else if (this._rightCorner != null && blockCorner.corner.x == this._rightCorner.corner.x && blockCorner.corner.y < this._rightCorner.corner.y)
					{
						if (this._topRightCorner == null)
						{
							this._topRightCorner = blockCorner;
						}
						else if (blockCorner.corner.y > this._topRightCorner.corner.y)
						{
							this._topRightCorner = blockCorner;
						}
					}
				}
			}
			if (this._leftStream != null)
			{
				this._leftStream.position.y = base.y - this._collisionOffset.y;
			}
			if (this._rightStream != null)
			{
				this._rightStream.position.y = base.y - this._collisionOffset.y;
			}
			this._collisionOffset.y = -num4;
			this._collisionSize.y = num4;
		}

		public override void Draw()
		{
			Graphics.DrawLine(this.position + new Vec2(-this._collisionOffset.x, this.collisionOffset.y / 2f + 0.5f), this.position + new Vec2(this._collisionOffset.x, this.collisionOffset.y / 2f + 0.5f), new Color(this.data.color) * this.data.transparent, this._collisionSize.y, 0.38f);
			Graphics.DrawLine(this.position + new Vec2(-this._collisionOffset.x, this.collisionOffset.y / 2f + 0.5f), this.position + new Vec2(this._collisionOffset.x, this.collisionOffset.y / 2f + 0.5f), new Color(this.data.color), this._collisionSize.y, -0.99f);
			if (this._lightRect != null)
			{
				this._lightRect.position = base.topLeft;
				this._lightRect.size = new Vec2(this.width, this.height);
			}
			int num = (int)Math.Ceiling((double)(this._collisionSize.x / 16f));
			float num2 = this._collisionSize.x / (float)num;
			if (this._lava != null && this.collisionSize.y > 2f)
			{
				bool flag = false;
				for (int j = 0; j < num; j++)
				{
					SpriteMap spriteMap = this._lava;
					if (flag)
					{
						spriteMap = this._lavaAlternate;
					}
					spriteMap.depth = 0.38f;
					spriteMap.depth += j;
					spriteMap.alpha = 0.7f;
					Graphics.DrawWithoutUpdate(spriteMap, (float)Math.Round((double)(this.left + 8f + (float)j * num2)), base.y + this._collisionOffset.y - 4f, 1f, 1f, false);
					spriteMap.depth = -0.5f;
					spriteMap.depth += j;
					spriteMap.alpha = 1f;
					Graphics.DrawWithoutUpdate(spriteMap, (float)Math.Round((double)(this.left + 8f + (float)j * num2)), base.y + this._collisionOffset.y - 4f, 1f, 1f, false);
					flag = !flag;
				}
				this._lava.UpdateFrame(false);
				this._lavaAlternate.UpdateFrame(false);
			}
			base.Draw();
		}

		public override void Terminate()
		{
			if (this._lightRect != null)
			{
				Level.Remove(this._lightRect);
			}
			base.Terminate();
		}

		private WhiteRectangle _lightRect;

		public FluidData data;

		public float _wide;

		public float fluidWave;

		private KetchupStream _leftStream;

		private KetchupStream _rightStream;

		private BlockCorner _leftCorner;

		private BlockCorner _rightCorner;

		private BlockCorner _topLeftCorner;

		private BlockCorner _topRightCorner;

		private bool _initializedUpperCorners;

		private Block _block;

		private SpriteMap _lava;

		private SpriteMap _lavaAlternate;

		private int _framesSinceFeed;

		private int bubbleWait;
	}
}
