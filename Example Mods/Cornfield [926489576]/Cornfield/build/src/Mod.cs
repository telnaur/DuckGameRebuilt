using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("Cornfield")]

// The author of the mod
[assembly: AssemblyCompany("Zyrafa")]

// The description of the mod
[assembly: AssemblyDescription("It's kinda like a reskin of a treetop, only more corny")]

// The mod's version
[assembly: AssemblyVersion("131.0.0.0")]

namespace DuckGame.MyMod
{
    public class MyMod : Mod
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
