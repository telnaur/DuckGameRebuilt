namespace DuckGame.HaloWeapons
{
    public class ATMangler : AmmoType
    {
        public ATMangler()
        {
            affectedByGravity = true;
            accuracy = 0.95f;
            bulletSpeed = 15f;
            bulletLength = 300f;
            range = 700f;
            weight = 25f;
            flawlessPipeTravel = true;
            penetration = 1f;
            combustable = true;
        }
    }
}
