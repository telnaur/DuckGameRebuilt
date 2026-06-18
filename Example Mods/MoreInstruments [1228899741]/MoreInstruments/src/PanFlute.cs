using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.MoreInstruments
{
	public partial class MoreInstruments : Mod
	{
		[BaggedProperty("isFatal", false), EditorGroup("guns|misc")]
		public class PanFlute : PitchBendableInstrument
		{
			public PanFlute(float xval, float yval)
				: base(xval, yval, "panflute")
			{
				base.graphic = new Sprite(Mod.GetPath<MoreInstruments>("panflute.png"), 0f, 0f);
				this.center = new Vec2(3f, 5f);
				this.collisionOffset = new Vec2(-3f, -5f);
				this.collisionSize = new Vec2(6, 11f);
				base._barrelOffsetTL = new Vec2(0f, 6f);
				base._holdOffset = new Vec2(3f, 2f);
			}

			public override void Update()
			{
				base.Update();
				if (owner != null)
				{
					if (base._raised)
					{
						base.handAngle = 0f;
						base.handOffset = new Vec2(0f, -2f);
						base._holdOffset = new Vec2(0f, 2f);
						this.OnReleaseAction();
					}
					else
					{
						base.handOffset = new Vec2(2f, -0.8f); // second
						base.handAngle = 0.1f * this.offDir;
						base._holdOffset = new Vec2(-1f, 1.3f);// first
					}
				}
				else
				{
					this.collisionOffset = new Vec2(-3f, -5f);
					this.collisionSize = new Vec2(6, 11f);
				}
			}
		}
	}
}
