using HarmonyLib;
using Microsoft.Xna.Framework;
using System;

namespace DuckGame.HaloWeapons
{
    internal sealed class Updater : IUpdateable
    {
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public bool Enabled => true;
        public int UpdateOrder => 1;

        public void Update(GameTime gameTime)
        {
            if (Level.current is null)
                return;

            foreach (Thing thing in Level.current.things)
            {
                if (thing is not ITimerThing timerThing)
                    continue;

                if (timerThing.Timer <= 0f && thing.isServerForObject)
                {
                    Level.Remove(thing);
                    continue;
                }

                if (thing is IFadingThing fadingThing)
                    thing.alpha = fadingThing.Timer / fadingThing.MaxTime;
            }
            
            UI.Update();
            SpriteMaterials.Update();
            RedFire.Update();
        }
    }
}
