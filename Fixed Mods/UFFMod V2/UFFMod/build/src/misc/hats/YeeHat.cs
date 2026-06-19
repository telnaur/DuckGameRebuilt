namespace DuckGame.UFFMod
{
    internal class YeeHat : TeamHat
    {
        public YeeHat(float x, float y, Team t)
            : base(x, y, t)
        {
        }

        public override void Terminate()
        {
            if (equippedDuck != null)
                equippedDuck._netQuack = new NetSoundEffect("quack");
            base.Terminate();
        }

        public override void Equip(Duck d)
        {
            base.Equip(d);
            d._netQuack = new NetSoundEffect(Mod.GetPath<UffMod>("SFX\\yee"));
        }

        public override void UnEquip()
        {
            if (equippedDuck != null)
                equippedDuck._netQuack = new NetSoundEffect("quack");
            base.UnEquip();
        }

        public override void Quack(float volume, float pitch)
        {
            SFX.Play(Mod.GetPath<UffMod>("SFX\\yee"), volume, pitch);
        }
    }
}
