using System.IO;

namespace DuckGame.HaloWeapons
{
    internal static class Paths
    {
        public static string SpritesDirectory => ContentDirectory + "Sprites/";
        public static string SoundsDirectory => ContentDirectory + "SFX/";
        public static string ShadersDirectory => ContentDirectory + "Shaders/";
        public static string ShadersSourceDirectory => ShadersDirectory + "Source";
        public static string DllsDirectory => ContentDirectory + "DLLs/";

        private static string ContentDirectory => $"{ModLoader.GetMod<HaloWeapons>().configuration.contentDirectory}/";

        public static string GetSpritePath(string fileName)
        {
            return SpritesDirectory + CheckExtension(fileName, "png");
        }

        public static string GetSoundPath(string fileName)
        {
            return SoundsDirectory + CheckExtension(fileName, "wav");
        }

        public static string GetShaderPath(string fileName)
        {
            return ShadersDirectory + CheckExtension(fileName, "xnb");
        }

        public static string GetDllPath(string fileName)
        {
            return DllsDirectory + CheckExtension(fileName, "dll");
        }

        public static string Get(string fileName)
        {
            return Mod.GetPath<HaloWeapons>(fileName);
        }

        private static string CheckExtension(string fileName, string extension)
        {
            extension = extension.Insert(0, ".");

            if (Path.GetExtension(fileName) == extension)
                return fileName;

            return fileName + extension;
        }
    }
}
