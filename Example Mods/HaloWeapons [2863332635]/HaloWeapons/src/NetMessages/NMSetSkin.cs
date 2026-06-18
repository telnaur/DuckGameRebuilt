namespace DuckGame.HaloWeapons
{
    public sealed class NMSetSkin : NMEvent
    {
        public HaloWeapon Weapon;
        public int SkinIndex;

        public NMSetSkin(HaloWeapon weapon, int skinIndex)
        {
            Weapon = weapon;
            SkinIndex = skinIndex;
        }

        public NMSetSkin()
        {

        }

        public override void Activate()
        {
            if (Options.Data.IgnoreOnlineSkins || Weapon is null)
                return;

            Weapon.SetGraphics(SkinIndex);
        }
    }
}
