using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class wizardSound : Thing
    {
        private float timer = 3f;

        public wizardSound()
          : base(0.0f, 0.0f, (Sprite)null)
        {
        }
        public override void Initialize()
        {
            SFX.Play(GetPath("wizardShort"), 0.95f, 0.0f, 0.0f, false);
            base.Initialize();
        }

        public override void Update()
        {
            timer -= 0.1f;
            if (timer < 0)
            {
                level.RemoveThing(this);
            }
            base.Update();
        }
    }
 }
