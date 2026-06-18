using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [BaggedProperty("isFatal", false)]
    [EditorGroup("Zyrafa|Guns|Misc")]
    public class airhorn : Gun
    {
        public float notePitch;
        public float handPitch;
        private float prevNotePitch;
        private float hitPitch;
        private Sound noteSound;

        public SpriteMap _sprite;

        public airhorn(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 20;
            this._type = "gun";

            this._sprite = new SpriteMap(GetPath("airhorn"), 9, 13, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(5f, 6f);
            this.collisionOffset = new Vec2(-5f, -6f);
            this.collisionSize = new Vec2(9f, 13f);
            this._holdOffset = new Vec2(0f, 3f);
            this.weight = 2f;
            this._barrelOffsetTL = new Vec2(7f, 5f);
            this.physicsMaterial = PhysicsMaterial.Plastic;
            this._sprite.frame = 0;
            this._editorName = "Airhorn";
            this.editorTooltip = "WHAT?! I CAN'T HEAR YOU OVER THIS JERK WITH AN AIRHORN!";

        }

        public override void Update()
        {
            Duck owner = this.owner as Duck;
            if (owner != null)
            {
                if (this.isServerForObject && owner.inputProfile != null)
                {
                    this.handPitch = owner.inputProfile.leftTrigger;
                    if (owner.inputProfile.hasMotionAxis)
                        this.handPitch += owner.inputProfile.motionAxis;
                    int num = Keyboard.CurrentNote(owner.inputProfile, (Thing)this);
                    if (num >= 0)
                    {
                        this.notePitch = (float)((double)num / 12.0 + 0.00999999977648258);
                        this.handPitch = this.notePitch;
                        if ((double)this.notePitch != (double)this.prevNotePitch)
                        {
                            this.prevNotePitch = 0.0f;
                            if (this.noteSound != null)
                            {
                                this.noteSound.Stop();
                                this._sprite.frame = 0;
                                this.noteSound = (Sound)null;
                            }
                        }
                    }
                    else
                        this.notePitch = !owner.inputProfile.Down("SHOOT") ? 0.0f : this.handPitch + 0.01f;
                }
                if ((double)this.notePitch != (double)this.prevNotePitch)
                {
                    if ((double)this.notePitch != 0.0)
                    {
                        if (this.noteSound == null)
                        {
                            this.hitPitch = this.notePitch;
                            this.noteSound = SFX.Play(GetPath("horn"), 1f, Maths.Clamp(((this.notePitch + 1) * 0.5f), -1f, 1f), 0.0f, false);
                            this._sprite.frame = 1;
                            EmitParticles();
                            //Level.Add((Thing)new MusicNote(this.barrelPosition.x, this.barrelPosition.y, this.barrelVector));
                        }
                        else
                        {
                            this.noteSound.Pitch = Maths.Clamp((float)(((double)this.notePitch - (double)this.hitPitch) * 0.100000001490116), -1f, 1f);
                        }
                    }
                    else if (this.noteSound != null)
                    {
                        this.noteSound.Stop();
                        this._sprite.frame = 0;
                        this.noteSound = (Sound)null;
                    }
                }
            }
            this.prevNotePitch = this.notePitch;
            base.Update();
        }

        public void EmitParticles()
        {
            for (int index = 0; index < 4; ++index)
            {
                int i = Rando.Int(0, 3);
                switch (i)
                {
                    case 0:
                        doritoSmoke doritoSmoke = new doritoSmoke((float)this.barrelPosition.x, this.barrelPosition.y);
                        doritoSmoke.depth = (Depth)((float)(0.899999976158142 + (double)1 * (1.0 / 1000.0)));
                        doritoSmoke.fly = new Vec2(Rando.Float(-2f, 2f) + this.barrelVector.x * Rando.Float(2f, 6f), Rando.Float(-2f, 2f) + this.barrelVector.y * Rando.Float(2f, 6f));
                        Level.Add((Thing)doritoSmoke);
                        break;
                    case 1:
                        glassesSmoke glassesSmoke = new glassesSmoke((float)this.barrelPosition.x, this.barrelPosition.y);
                        glassesSmoke.depth = (Depth)((float)(0.899999976158142 + (double)1 * (1.0 / 1000.0)));
                        glassesSmoke.fly = new Vec2(Rando.Float(-2f, 2f) + this.barrelVector.x * Rando.Float(2f, 6f), Rando.Float(-2f, 2f) + this.barrelVector.y * Rando.Float(2f, 6f));
                        Level.Add((Thing)glassesSmoke);
                        break;
                    case 2:
                        mlgSmoke mlgSmoke = new mlgSmoke((float)this.barrelPosition.x, this.barrelPosition.y);
                        mlgSmoke.depth = (Depth)((float)(0.899999976158142 + (double)1 * (1.0 / 1000.0)));
                        mlgSmoke.fly = new Vec2(Rando.Float(-2f, 2f) + this.barrelVector.x * Rando.Float(2f, 6f), Rando.Float(-2f, 2f) + this.barrelVector.y * Rando.Float(2f, 6f));
                        Level.Add((Thing)mlgSmoke);
                        break;
                    case 3:
                        illuminatiSmoke illuminatiSmoke = new illuminatiSmoke((float)this.barrelPosition.x, this.barrelPosition.y);
                        illuminatiSmoke.depth = (Depth)((float)(0.899999976158142 + (double)1 * (1.0 / 1000.0)));
                        illuminatiSmoke.fly = new Vec2(Rando.Float(-2f, 2f) + this.barrelVector.x * Rando.Float(2f, 6f), Rando.Float(-2f, 2f) + this.barrelVector.y * Rando.Float(2f, 6f));
                        Level.Add((Thing)illuminatiSmoke);
                        break;
                    default:
                        doritoSmoke doritoSmoke2 = new doritoSmoke((float)this.barrelPosition.x, this.barrelPosition.y);
                        doritoSmoke2.depth = (Depth)((float)(0.899999976158142 + (double)1 * (1.0 / 1000.0)));
                        doritoSmoke2.fly = new Vec2(Rando.Float(-2f, 2f) + this.barrelVector.x * Rando.Float(2f, 6f), Rando.Float(-2f, 2f) + this.barrelVector.y * Rando.Float(2f, 6f));
                        Level.Add((Thing)doritoSmoke2);
                        break;
                }
            }
        }

        public override void OnPressAction()
        {
        }

        public override void Thrown()
        {
            base.Thrown();
            this._sprite.frame = 0;
        }

        public override void OnReleaseAction()
        {
            if (this.owner == null)
                return;
            this._sprite.frame = 0;
        }

    }
}