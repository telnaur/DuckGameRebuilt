using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("More Instruments")]

// The author of the mod
[assembly: AssemblyCompany("grappigegovert")]

// The description of the mod
[assembly: AssemblyDescription("Adds more music instruments to play with")]

// The mod's version
[assembly: AssemblyVersion("1.2.0.23")]

namespace DuckGame.MoreInstruments
{
	public partial class MoreInstruments : Mod
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
			Mod.Debug.Log("MoreInstruments initialized.");
			base.OnPostInitialize();
		}
	}
}
