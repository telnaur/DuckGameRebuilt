namespace DuckGame.HaloWeapons
{
    public static class EditorGroups
    {
        public const string Guns = Section + "Guns";
        public const string Props = Section + "Props";
        public const string Equipment = Section + "Equipment";
        public const string Blocks = Section + "Blocks";
        public const string Background = Section + "Background";
        public const string JumpThroung = $"{Blocks}|JumpThrough";
        public const string Parallax = $"{Background}|Parallax";

        private const string Section = "Halo Weapons|";
    }
}
