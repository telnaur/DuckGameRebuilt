using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.MoreInstruments
{
	public partial class MoreInstruments : Mod
	{
		[BaggedProperty("isFatal", false), EditorGroup("guns|misc")]
		public class Guitar : PitchBendableInstrument
		{
			public Guitar(float xval, float yval)
				: base(xval, yval, "guitar")
			{
				base.graphic = new Sprite(Mod.GetPath<MoreInstruments>("guitar.png"), 0f, 0f);
				this.center = new Vec2(16f, 16f);
				this.collisionOffset = new Vec2(-16f, -6f);
				this.collisionSize = new Vec2(32f, 12f);
				base._barrelOffsetTL = new Vec2(16f, 16f);
				base._holdOffset = new Vec2(6f, 2f);
			}

			public override void Update()
			{
				base.Update();
				if (owner != null)
				{
					if (base._raised)
					{
						base.handAngle = 0f;
						base.handOffset = new Vec2(0f, 0f);
						base._holdOffset = new Vec2(0f, 2f);
						this.collisionOffset = new Vec2(-4f, -7f);
						this.collisionSize = new Vec2(8f, 16f);
						this.OnReleaseAction();
					}
					else
					{
						base.handOffset = new Vec2(4f, 3f);
						base.handAngle = ((-1f - this.handPitch) * 0.4f) * this.offDir;
						base._holdOffset = new Vec2(2f, 5f);
						this.collisionOffset = new Vec2(-1f, -7f);
						this.collisionSize = new Vec2(2f, 16f);
					}
				}
				else
				{
					this.collisionOffset = new Vec2(-16f, -6f);
					this.collisionSize = new Vec2(32f, 12f);
				}
			}
		}
	}
}
