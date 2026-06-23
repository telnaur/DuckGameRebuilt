using System.Reflection;
using DuckGame;

// Mod display metadata is read off these assembly attributes (NOT mod.conf).
// See docs/modding-guide.md §2.4.
[assembly: AssemblyTitle("SuperDuck")]
[assembly: AssemblyDescription("Magical guns and equipment for ducks. Gandalf's Staff, Balrog's Whip, and more.")]
[assembly: AssemblyCompany("David Wood")]
[assembly: AssemblyVersion("1.0.0.0")]

namespace DuckGame.SuperDuck
{
    // Every code mod needs exactly one non-abstract Mod subclass; the loader finds it by
    // reflection (ModLoader.cs:631). Content (the staff) is discovered automatically via
    // its [EditorGroup] attribute, so this class only needs to exist.
    public class SuperDuckMod : Mod
    {
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();
        }
    }
}
