namespace DuckGame.UFFMod
{
    internal class GaroHat : TeamHat
    {
        public GaroHat(float x, float y, Team t)
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
            d._netQuack = new NetSoundEffect("roboQuack");
        }

        public override void UnEquip()
        {
            if (equippedDuck != null)
                equippedDuck._netQuack = new NetSoundEffect("quack");
            base.UnEquip();
        }

        public override void Quack(float volume, float pitch)
        {
            SFX.Play("roboQuack", volume, pitch);
        }
    }
}
