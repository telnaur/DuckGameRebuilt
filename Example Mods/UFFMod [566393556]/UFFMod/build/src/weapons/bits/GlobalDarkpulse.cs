namespace DuckGame.UFFMod
{
    public class GlobalDarkpulse : GlobalPulse
    {
        public GlobalDarkpulse(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\darkpulseShockwave"), 160, 160);
            _sprite.AddAnimation("darkpulse", 0.75f, false, 0, 1, 2, 3, 4, 5);
            _sprite.SetAnimation("darkpulse");
            graphic = _sprite;
        }
    }
}
