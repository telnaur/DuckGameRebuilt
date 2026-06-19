using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    internal class ATEMProjectile : AmmoType
    {
        public ATEMProjectile()
        {
            accuracy = 1f;
            range = 1000f;
            penetration = 1f;
            bulletLength = 800f;
            bulletSpeed = 40f;
            bulletThickness = 0.3f;
            bulletType = typeof(EMProjectile);
        }
    }
}
