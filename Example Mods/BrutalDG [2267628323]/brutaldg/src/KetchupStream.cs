using System;
using DuckGame;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.BrutalDG
{
	internal class KetchupStream : Thing
	{
		public Vec2 sprayAngle
		{
			get
			{
				return this._sprayAngle;
			}
			set
			{
				this._sprayAngle = value;
			}
		}

		public Vec2 startSprayAngle
		{
			get
			{
				return this._startSprayAngle;
			}
		}

		public float holeThickness
		{
			get
			{
				return this._holeThickness;
			}
			set
			{
				this._holeThickness = value;
			}
		}

		public Vec2 offset
		{
			get
			{
				return this._offset;
			}
			set
			{
				this._offset = value;
			}
		}

		public bool onFire
		{
			get
			{
				return this._onFire;
			}
			set
			{
				this._onFire = value;
			}
		}

		public KetchupStream(float xpos, float ypos, Vec2 sprayAngleVal, float sprayVelocity, Vec2 off = default(Vec2)) : base(xpos, ypos, null)
		{
			this._endPoint = new Vec2(xpos, ypos);
			this._sprayAngle = sprayAngleVal;
			this._startSprayAngle = sprayAngleVal;
			this._sprayVelocity = sprayVelocity;
			this._offset = off;
			this.ketchup = 3f;
		}

		public void Feed(FluidData dat)
		{
			float num = Maths.Clamp(dat.amount * 200f, 0.1f, 2f);
			if (num > this._maxSpeedMul)
			{
				this._maxSpeedMul = Lerp.Float(this._maxSpeedMul, num, 0.1f);
			}
			this._lastFluid = new KetchupParticle(base.x, base.y, (this._sprayAngle * ((2f + (float)Math.Sin((double)this._fluctuate) * 0.5f) * this._speedMul) + new Vec2(this.hSpeed * 0f, this.vSpeed * 0f)) * this.streamSpeedMultiplier, dat, this._lastFluid, 1f);
			Level.Add(this._lastFluid);
			this._fluid.Add(this._lastFluid);
			this._framesSinceFluid = 0;
			this._fluctuate += 0.2f;
		}

		public override void Update()
		{
			List<KetchupParticle> list = new List<KetchupParticle>();
			foreach (KetchupParticle particle in this._fluid)
			{
				if (particle != null && particle.removeFromLevel)
				{
					list.Add(particle);
				}
			}
			foreach (KetchupParticle particle in list)
			{
				this._fluid.Remove(particle);
			}
			if (this.thing != null)
			{
				this._sprayAngle = new Vec2(this.thing._hSpeed + Rando.Float(-0.01f, 0.02f), this.thing._vSpeed - Rando.Float(0.1f, 0.5f));
			}
			this.ketchup -= 0.01f;
			if (this.ketchup <= 0f || !BrutalOptionsData.enableblood)
			{
				if (this.thing != null && CoolUpdate.streams.Contains(this.thing))
				{
					CoolUpdate.streams.Remove(this.thing);
				}
				Level.Remove(this);
			}
			this.color = new Color(240, 48, 48);
			if (BrutalOptionsData.bloodcolor < BrutalDG.blood.Count)
			{
				this.color = BrutalDG.blood[BrutalOptionsData.bloodcolor];
			}
			else
			{
				this.color = BrutalDG.blood[Rando.Int(BrutalDG.blood.Count - 1)];
			}
			FluidData fluid = new FluidData(0f, color.ToVector4() * 0.8f, 0.4f, null, 0f, 0.7f);
			fluid.amount = Rando.Float(this.ketchup / 8 * 0.005f, this.ketchup / 8 * 0.01f);
			this._framesSinceFire++;
			this._maxSpeedMul = Lerp.Float(this._maxSpeedMul, 0.1f, 0.001f);
			this._speedMul = Lerp.Float(this._speedMul, this._maxSpeedMul, 0.04f);
			//if (this._lastFluid != null)
			{
				this._framesSinceFluid++;
			}
			if (this._framesSinceFluid > 1)
			{
				this.Feed(fluid);
			}
			//this._framesSinceFluid = 0;
			//this._lastFluid = null;
		}

		private Vec2 _sprayAngle;

		private Vec2 _startSprayAngle;

		private float _holeThickness = 1f;

		private float _sprayVelocity;

		private Vec2 _endPoint;
		
		private Vec2 _offset;

		private bool _onFire;

		private int _framesSinceFire;

		private KetchupParticle _lastFluid;
		
		private List<KetchupParticle> _fluid = new List<KetchupParticle>();

		private int _framesSinceFluid;

		private float _fluctuate;

		private float _speedMul = 0.1f;

		private float _maxSpeedMul = 0.1f;

		public float streamSpeedMultiplier = 1f;
		
		public float ketchup;
		
		public MaterialThing thing;
		
		private Color color;
	}
}
