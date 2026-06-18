using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|boots")]
    public class TornadoBoots : Boots
    {
        public StateBinding _jumpedStateBinding = new StateBinding("_jumped");
        public StateBinding _stoppedJumpingStateBinding = new StateBinding("_stoppedJumping");
        public StateBinding _jumpCountStateBinding = new StateBinding("_jumpCount");

        public bool _jumped;
        public bool _stoppedJumping;
        public int _jumpCount;

        public TornadoBoots(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Tornado Boots";

            _pickupSprite = new Sprite(Mod.GetPath<UffMod>("equipment\\tornadoBootsPickup"), 0f, 0f);
            _sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\tornadoBoots"), 32, 32, false);
            graphic = _pickupSprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -6f);
            collisionSize = new Vec2(12f, 13f);
            _equippedDepth = 1;

            // defaults
            _jumped = false;
            _stoppedJumping = false;
            _jumpCount = 0;
        }

        public override void Update()
        {
            if (equippedDuck != null && !equippedDuck.grounded)
            {
                bool check = false;
                if (equippedDuck._hovering && _jumpCount < 12)
                {
                    check = true;
                    equippedDuck._hovering = false;
                }
                if (check && equippedDuck.inputProfile.Down(Triggers.Jump) && !_jumped)
                {
                    // jump
                    SFX.Play("jump", 0.5f, 0f, 0f, false);
                    if(isServerForObject)
                        Level.Add(new TornadoWisp(equippedDuck.x, equippedDuck.y + 12f));
                    equippedDuck._vSpeed = -5.9f;
                    _jumped = true;
                }
                else if (equippedDuck.inputProfile.Down(Triggers.Jump) && _jumped && _jumpCount < 12 && !_stoppedJumping)
                {
                    equippedDuck._vSpeed += 0.32f - (0.12f * (float)Math.Sqrt(_jumpCount)) < 0.02f ? 0.02f : 0.32f - (0.12f * (float)Math.Sqrt(_jumpCount));
                    _jumpCount++;
                }
                else if (!equippedDuck.inputProfile.Down(Triggers.Jump) && _jumped && _jumpCount < 12)
                {
                    _stoppedJumping = true;
                    equippedDuck._vSpeed += 0.32f - (0.03f * (float)Math.Sqrt(_jumpCount));
                    _jumpCount++;
                }
            }
            else
            {
                _jumped = false;
                _stoppedJumping = false;
                _jumpCount = 0;
            }
            base.Update();
        }
    }
}
