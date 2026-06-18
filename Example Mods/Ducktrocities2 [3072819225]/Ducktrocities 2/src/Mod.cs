using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;

[assembly: AssemblyTitle("Ducktrocities 2")]
[assembly: AssemblyCompany("Epsilous")]
[assembly: AssemblyDescription("10 more cursed hats for your enjoyment.")]
[assembly: AssemblyVersion("1.0.0.0")]

namespace DuckGame.Ducktrocities2Modpack
{
    public class Ducktrocities2 : Mod
    {
        protected override void OnPostInitialize()
        {
            CopyHats();
            base.OnPostInitialize();
        }
        private byte[] GetHash(byte[] sourceBytes)
        {
            return new MD5CryptoServiceProvider().ComputeHash(sourceBytes);
        }
        private void CopyHats()
        {
			var oldLocation = new DirectoryInfo(GetPath<Ducktrocities2>("Hats"));
            string newLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Steam\steamapps\common\Duck Game\Hats");
            foreach (var f in oldLocation.GetFiles("*", SearchOption.AllDirectories))
            {
				string hat = newLocation + f.FullName.Substring(f.FullName.LastIndexOf("Hats") + "Hats".Length);
                Directory.CreateDirectory(hat.Replace(f.Name, ""));
                if (!File.Exists(hat) || !GetHash(File.ReadAllBytes(f.FullName)).SequenceEqual(GetHash(File.ReadAllBytes(hat))))
                    f.CopyTo(hat, true);
            }
		}
    }
}