using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

[assembly: AssemblyTitle("Tar Gun")]
[assembly: AssemblyCompany("Lia")]
[assembly: AssemblyDescription("Adds a state-of-the-art tool for disposing of the heaviest of oil fractions")]
[assembly: AssemblyVersion("1.2.1.0")]

namespace DuckGame.TarGunMod
{
    public class TarGunMod : Mod
    {
        // The mod's priority; this property controls the load order of the mod.
        public override Priority priority
		{
			get { return base.priority; }
		}

		// This function is run before all mods are finished loading.
		protected override void OnPreInitialize()
		{
			base.OnPreInitialize();
		}

		// This function is run after all mods are loaded.
		protected override void OnPostInitialize()
		{
			base.OnPostInitialize();
		}
	}
}
