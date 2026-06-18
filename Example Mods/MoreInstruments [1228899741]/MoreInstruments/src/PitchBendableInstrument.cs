using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.MoreInstruments
{
	public partial class MoreInstruments : Mod
	{
		[BaggedProperty("canSpawn", false)]
		public abstract class PitchBendableInstrument : Gun
		{
			public StateBinding _handPitchBinding;
			public StateBinding _notePitchBinding;
			public float handPitch;
			private float hitPitch;
			public float notePitch;
			private Sound noteSound;
			private float prevNotePitch;
			private string instrumentname;

			public PitchBendableInstrument(float xval, float yval, string instrumentname)
				: base(xval, yval)
			{
				this.instrumentname = instrumentname;
				this._notePitchBinding = new StateBinding("notePitch", -1, false);
				this._handPitchBinding = new StateBinding("handPitch", -1, false);
				base.ammo = 4;
				base.graphic = new Sprite(Mod.GetPath<MoreInstruments>(instrumentname + ".png"), 0f, 0f);
				this.center = new Vec2(16f, 16f);
				this.collisionOffset = new Vec2(-16f, -6f);
				this.collisionSize = new Vec2(32f, 12f);
				base.depth = 0.6f;
				base._barrelOffsetTL = new Vec2(16f, 16f);
				base._holdOffset = new Vec2(6f, 2f);
				this._notePitchBinding.skipLerp = true;
			}

			public override void Fire()
			{ }

			public override void OnPressAction()
			{ }

			public override void OnReleaseAction()
			{ }

			public override void Update()
			{
				Duck owner = this.owner as Duck;
				if (owner != null)
				{
					if (base.isServerForObject)
					{
						this.handPitch = owner.inputProfile.leftTrigger;
						if (owner.inputProfile.Down("SHOOT"))
						{
							this.notePitch = this.handPitch + 0.01f;
						}
						else
						{
							this.notePitch = 0f;
						}
					}
					if (this.notePitch != this.prevNotePitch)
					{
						if (this.notePitch != 0f)
						{
							int num = (int)Math.Round(this.notePitch * 12f);
							if (num < 0)
								num = 0;
							if (num > 12)
								num = 12;
							if (this.noteSound == null)
							{
								this.hitPitch = this.notePitch;
								Sound sound = SFX.Play(Mod.GetPath<MoreInstruments>(instrumentname + num.ToString()), 1f, 0f, 0f, false);
								this.noteSound = sound;
								Level.Add(new MusicNote(base.barrelPosition.x, base.barrelPosition.y, base.barrelVector));
							}
							else
							{
								this.noteSound.Pitch = Maths.Clamp((float)((this.notePitch - this.hitPitch) * 0.1f), (float)-1f, (float)1f);
							}
						}
						else if (this.noteSound != null)
						{
							this.noteSound.Stop();
							this.noteSound = null;
						}
					}
				}
				this.prevNotePitch = this.notePitch;
				base.Update();
			}
		}
	}
}
