namespace DuckGame.HaloWeapons
{
    internal static class Core
    {
        public static Profile LocalProfile => Network.isActive ? DuckNetwork.localProfile : Profiles.DefaultPlayer1;

        public static Duck LocalDuck => LocalProfile?.duck;

        public static void Log(string text)
        {
            DevConsole.Log($"||ORANGE|HALO WEAPONS|GREEN|||WHITE|{text}", Color.LimeGreen);
        }
    }
}
