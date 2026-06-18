using System;
using DuckGame;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.BrutalDG
{
	internal class BloodThings : Thing
	{
		public BloodThings(Thing t) : base()
		{
			this._thing = t;
			if (t.graphic != null && t.graphic.texture != null)
			{
				this._prevGraphic = this._thing.graphic.texture;
			}
		}
		
		public override void Update()
		{
			if (!BrutalDG.bloodguns || this._prevGraphic == null)
			{
				Level.Remove(this);
				return;
			}
			this.drawing = Rando.Int(40) > 29;
			this.timer += 0.1f;
			FluidPuddle puddle = Level.CheckPoint<FluidPuddle>(this._thing.position, null, null);
			//if (this.timer >= 350f)
			if (puddle != null)
			{
				//if (this._blood.Count > 0)//if (this._blood.Count > 20)
				//{
				//	this._draw = this._thing.graphic.Clone();
				//	this._draw.depth = this._draw.depth;
				//	this._thing.graphic.texture = this._prevGraphic;
				//	this._amount = 6;
				//	this._amountdid = 0;
				//	this._blood.Clear();
				//}
				//this.timer = 0f;
				foreach (KetchupPixelThing pixel in this._ketchup.Keys)
                {
					pixel.Wash();
                }
				if (this.remove)
                {
					this._ketchup.Clear();
					this.remove = false;
				}
			}
			if (BrutalOptionsData.bloodcolor == BrutalDG.blood.Count)
			{
				this._blood.Clear();
			}
			if (this._thing == null || this._thing.removeFromLevel)
			{
				Level.Remove(this);
				return;
			}
			foreach (KetchupPixelThing pixel in this._ketchup.Keys)
			{
				try
				{
					if (this._thing.graphic is SpriteMap)
					{
						if (this._ketchup.FirstOrDefault(x => x.Key == pixel).Value == (this._thing.graphic as SpriteMap).frame)
						{
							if (!Level.current.things.Contains(pixel))
							{
								Level.Add(pixel);
							}
						}
						else
						{
							if (Level.current.things.Contains(pixel))
							{
								Level.Remove(pixel);
							}
						}
					}
					else
                    {
						if (!Level.current.things.Contains(pixel))
						{
							Level.Add(pixel);
						}
					}
				}
				catch { }
			}
			base.Update();
		}
		
		public override void Draw()
		{
			if (this._draw != null && this._thing.graphic != null)
			{
				Sprite spr = this._thing.graphic;
				this._draw.depth = spr.depth.Add(1);
				this._draw.center = spr.center;
				this._draw.position = spr.position;
				this._draw.scale = spr.scale;
				this._draw._angle = spr.angle;
				this._draw.flipH = spr.flipH;
				this._draw.flipV = spr.flipV;
				this._draw.alpha -= 0.01f;
				this._draw.Draw();
			}
			base.Draw();
		}
		
		public static BloodThings GetThing(Thing t)
		{
			foreach (BloodThings thing in Level.current.things[typeof(BloodThings)])
			{
				if (thing._thing == t)
				{
					return thing;
				}
			}
			return null;
		}
		
		public Thing _thing;
		
		public int _amount;
		
		public int _amountdid;
		
		public List<Vec2> _blood = new List<Vec2>();
		
		public Tex2D _prevGraphic;
		
		public Sprite _draw;
		
		private float timer;
		
		public bool drawing;

		public Dictionary<KetchupPixelThing, int> _ketchup = new Dictionary<KetchupPixelThing, int>();

		public bool remove;
	}
}
