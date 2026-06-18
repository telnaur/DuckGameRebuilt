using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Stuff")]

    public class invisibleBlock : Block, IDontMove
    {
        protected SpriteMap _sprite;
        public bool _pin = true;
        private Sprite _bottom;
        private Sprite _top;
        private float _open;
        private float _desiredOpen;
        private bool _opened;
        private Vec2 _topLeft;
        private Vec2 _topRight;
        private Vec2 _bottomLeft;
        private Vec2 _bottomRight;
        private bool _cornerInit;


        public invisibleBlock(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap(GetPath("invisibleBlock"), 16, 16, false);
            this.graphic = (Sprite)this._sprite;
            this._sprite.AddAnimation("idle", 1f, false, 4);
            this._sprite.AddAnimation("touch", 0.15f, 1 != 0, 1, 2, 3);
            this._sprite.AddAnimation("idle2", 1f, false, 0);
            this._sprite.SetAnimation("idle");
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.thickness = 4f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._editorName = "Invisible Block";
            this.editorTooltip = "Get close, it appears. Get away, it disappears. Simple.";
        }
        public override void Update()
        {
            if (!this._cornerInit)
            {
                this._topLeft = this.topLeft;
                this._topRight = this.topRight;
                this._bottomLeft = this.bottomLeft;
                this._bottomRight = this.bottomRight;
                this._cornerInit = true;
            }
            if (Level.CheckRect<PhysicsObject>(this._topLeft - new Vec2(15f, 5f), this._bottomRight + new Vec2(15f, 5f), (Thing)null) != null)
                this._desiredOpen = 1f;
            else if (Level.CheckRect<PhysicsObject>(new Vec2(this.x - 4f, this.y - 24f), new Vec2(this.x + 4f, this.y + 8f), (Thing)null) == null)
                this._desiredOpen = 0.0f;
            if ((double)this._desiredOpen > 0.5 && !this._opened)
            {
                this._opened = true;
                this._sprite.SetAnimation("touch");
            }
            if ((double)this._desiredOpen < 0.5 && this._opened)
            {
                this._opened = false;
                this._sprite.SetAnimation("idle");
            }
        }

        public override void EditorRender()
        {
            this._sprite.SetAnimation("idle2");
            base.EditorRender();
        }
    }
}

