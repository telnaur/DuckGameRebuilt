namespace DuckGame.HaloWeapons
{
    public abstract class ATReboundBullet : AmmoType
    {
        public ATReboundBullet()
        {
            rebound = true;

            speedVariation = 0f;
            rangeVariation = 0f;
            accuracy = 1f;
        }
    }
}
