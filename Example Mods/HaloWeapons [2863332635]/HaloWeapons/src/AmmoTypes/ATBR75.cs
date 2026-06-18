namespace DuckGame.HaloWeapons
{
    public class ATBR75 : AmmoType
    {
        public ATBR75()
        {
            accuracy = 0.85f;
            range = 200f;
            rangeVariation = 30f;
            penetration = 1f;
            bulletThickness = 0.7f;
            bulletLength = 100f;
            combustable = true;
        }
    }
}
