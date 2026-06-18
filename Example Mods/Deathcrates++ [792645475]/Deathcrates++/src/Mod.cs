using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("Deathcrates++")]

// The author of the mod
[assembly: AssemblyCompany("ImDaBanana")]

// The description of the mod
[assembly: AssemblyDescription("This mod adds new Deathcrate possibilities for all your death and chaos!")]

// The mod's version
[assembly: AssemblyVersion("1.2.3.0")] //MAJOR.MINOR.PATCH.DEV (DEV should ALWAYS be 0 on a public version)

namespace DuckGame.DeathcratesPlusPlus
{
    public class DeathcratesPlusPlus : Mod
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
