using HarmonyLib;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using DuckGame.RUDE;
using System.Linq;

[assembly: AssemblyTitle("Halo Weapons")]
[assembly: AssemblyDescription("Weapons from Halo series")]
[assembly: AssemblyCompany("|GREEN|TheKingOfCringe|RED|11")]
[assembly: AssemblyVersion("1.0.0.0")]


namespace DuckGame.HaloWeapons
{
    public class HaloWeapons : Mod
    {
        protected override void OnPreInitialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.Resolve;
        }

        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            BindingAttributes.Initialize();

            Resources.LoadShaders();
            Skins.Load();
            Options.Load();
            ModInteractions.LoadMods();

            var harmony = new Harmony("null or empty");

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                try
                {
                    harmony.CreateClassProcessor(type).Patch();
                }
                catch
                {
                    Core.Log($"|RED|Failed to apply patch: {type.Name}");
                }
            }

            if (!ModInteractions.ModEnabled("RUDE")) 
                harmony.Patch(AccessTools.Method(typeof(Duck), nameof(Duck.Kill)), postfix: new HarmonyMethod(typeof(DuckKillPatched), nameof(DuckKillPatched.Kill)));

            AccessTools.Field(typeof(Game), "updateableComponents").GetValue<List<IUpdateable>>(MonoMain.instance).Add(new Updater());

            //ModInteractions.Add("RUDE", AddGunsToRUDEGunGame);
        }

        private void AddGunsToRUDEGunGame(Assembly modAssembly)
        {
            List<GunGame.GunLevel> gunLevels = GunGame.allGunLevels;
            int length = gunLevels.Count;

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsAbstract || !type.IsSubclassOf(typeof(HaloWeapon)))
                    continue;

                GunGameLevelAttribute gunGameLevel = type.GetCustomAttribute<GunGameLevelAttribute>();

                if (gunGameLevel is null)
                    continue;

                int value = gunGameLevel.Value - 1;

                if (value < length)
                    gunLevels[value].Add(type);
            }
        }
    }
}

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit
    {

    }
}
