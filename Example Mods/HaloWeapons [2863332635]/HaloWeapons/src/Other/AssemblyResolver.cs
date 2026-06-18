using System;
using System.IO;
using System.Reflection;

namespace DuckGame.HaloWeapons
{
    public static class AssemblyResolver
    {
        public static Assembly Resolve(object sender, ResolveEventArgs eventArgs)
        {
            if (Assembly.GetCallingAssembly() != Assembly.GetExecutingAssembly())
                return null;

            string assemblyFullName = eventArgs.Name;

            try
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    if (assembly.FullName == assemblyFullName)
                        return assembly;
            }
            catch
            {

            }

            string assemblyShortName = new AssemblyName(eventArgs.Name).Name;
            string path = Paths.GetDllPath(assemblyShortName);

            if (!File.Exists(path))
                return null;

            Assembly loadedAssembly = null;

            try
            {
                loadedAssembly = Assembly.LoadFrom(path);
            }
            catch
            {
                try
                {
                    loadedAssembly = Assembly.Load(path);
                }
                catch
                {

                }
            }

            return loadedAssembly;
        }
    }
}
