using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|hats")]
    public class WhizzardHat : Hat
    {
        public StateBinding _potencyStateBinding = new StateBinding("_potency");
        public StateBinding _timesDoneStateBinding = new StateBinding("_timesDone");

        public float _potency;
        public int _timesDone;

        private int sparkleWait;
        private int levWait;

        public WhizzardHat(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Wizard Hat";
            
            // collision & sprite settings
            _pickupSprite = (Sprite)new SpriteMap(Mod.GetPath<UffMod>("equipment\\whizzard"), 23, 14);
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\whizzard"), 23, 14);
            graphic = sprite;
            center = new Vec2(12f, 7f);
            _collisionOffset = new Vec2(-11f, -6f);
            _collisionSize = new Vec2(21f, 12f);
            _wearOffset = new Vec2(0f, -4f);
            sprite.CenterOrigin();

            // equipment settings
            _equippedThickness = 0.1f;

            // defaults
            _potency = 0f;
            _timesDone = 0;
            sparkleWait = 0;
        }

        public override void Update()
        {
            if (equippedDuck != null)
            {
                if (equippedDuck._hovering && _potency > -1)
                {
                    if (sparkleWait == 0)
                    {
                        for (int i = 0; i < Rando.Int(1, 3); i++)
                            Level.Add(new WhizzardPixieDust(equippedDuck.x + Rando.Float(-18f, 18f), equippedDuck.y + Rando.Float(-22f, 22f)));
                        sparkleWait = 6;
                    }
                    else
                        sparkleWait--;

                    if (levWait == 0)
                    {
                        SFX.Play(Mod.GetPath<UffMod>("SFX\\lev"), 0.3f, 0f, 0f, false);
                        levWait = 18;
                    }
                    else
                        levWait--;

                    // levitate
                    if (equippedDuck._vSpeed > 0.5f)
                    {
                        equippedDuck._vSpeed = -1.18f - _potency;
                        _potency -= ((float)Math.Sqrt(0.002 * _timesDone));
                        _timesDone++;
                    }
                }
                if (equippedDuck.grounded)
                {
                    _potency = 0f;
                    _timesDone = 0;
                }
            }
            base.Update();
        }
    }
}
