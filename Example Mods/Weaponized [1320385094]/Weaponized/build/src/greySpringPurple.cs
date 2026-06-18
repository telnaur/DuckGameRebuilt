using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Stuff|Props")]
    public class greySpringPurple : Holdable, IPlatform
    {
        private SpriteMap _sprite;
        protected float _mult;
        protected float _soundWait;

        public greySpringPurple(float xpos, float ypos, float mult = 0.3f)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap(GetPath("greySpringPurple"), 16, 16, false);
            this.graphic = (Sprite)this._sprite;
            this._sprite.AddAnimation("idle", 1f, false, new int[1]);
            this._sprite.AddAnimation("spring", 0.5f, 0 != 0, 1, 1, 0);
            this._sprite.SetAnimation("idle");
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = -0.5f;
            this.thickness = 2f;
            this.weight = 6f;
            this.flammable = 0f;
            this.collideSounds.Add("metalRebound");
            this._mult = mult;
            this._editorName = "Spring Box Purple";
            this.editorTooltip = "Genetically weaker than his red counterpart. Such a shame.";
            this.physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Update()
        {
            if ((double)this._soundWait > 0.0)
                this._soundWait -= 0.1f;
            base.Update();
        }

        public void SpringUp()
        {
            this._sprite.currentAnimation = "spring";
            this._sprite.frame = 0;
            if ((double)this._soundWait > 0.0)
                return;
            SFX.Play("spring", 0.3f, Rando.Float(0.2f) - 0.1f, 0.0f, false);
            this._soundWait = 1f;
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {

            if (from != ImpactedFrom.Top)
                return;
            else
            {
                if (with.isServerForObject)
                {
                    if ((double)with.vSpeed > -22.0 * (double)this._mult)
                        with.vSpeed = -22f * this._mult;
                    if (with is RagdollPart)
                    {
                        if ((double)Math.Abs(with.hSpeed) < 0.100000001490116)
                            with.hSpeed = (double)Rando.Float(1f) >= 0.5 ? 1.3f : -1.3f;
                        else
                            with.hSpeed *= (float)(double)Rando.Float(1.1f, 1.4f);
                    }
                    if (with is Mine)
                    {
                        if ((double)Math.Abs(with.hSpeed) < 0.100000001490116)
                            with.hSpeed = (double)Rando.Float(1f) >= 0.5 ? 1.2f : -1.2f;
                        else
                            with.hSpeed *= (float)(double)Rando.Float(1.1f, 1.2f);
                    }

                    with.lastHSpeed = with._hSpeed;
                    with.lastVSpeed = with._vSpeed;
                    if (with is Duck)
                        (with as Duck).jumping = false;
                    if (with is Gun)
                    {
                        if (with is sausage)
                            return;
                        (with as Gun).PressAction();
                    }
                }
                this.SpringUp();
            }
        }
    }
}
