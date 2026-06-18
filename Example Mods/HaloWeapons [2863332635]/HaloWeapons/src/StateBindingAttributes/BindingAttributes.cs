using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static System.Reflection.Emit.OpCodes;

namespace DuckGame.HaloWeapons
{
    internal static class BindingAttributes
    {
        private static readonly Dictionary<Type, IEnumerable<BindingAttribute>> s_bindingAttributes = new Dictionary<Type, IEnumerable<BindingAttribute>>();

        public static void InitializeStateBindings(GhostObject ghostObject)
        {
            Thing thing = AccessTools.Field(typeof(GhostObject), "_thing").GetValue<Thing>(ghostObject);
            List<StateBinding> bindings = AccessTools.Field(typeof(GhostObject), "_fields").GetValue<List<StateBinding>>(ghostObject);

            foreach (BindingAttribute bindingAttribute in GetBindingAttributes(thing.GetType()))
            {
                StateBinding binding = bindingAttribute.CreateStateBinding();

                binding.Connect(thing);
                bindings.Add(binding);
            }
        }

        public static void Initialize()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (Type type in Editor.ThingTypes.Where(type => type.Assembly == assembly))
                s_bindingAttributes.Add(type, FindBindingAttributes(type));
        }

        public static void CorrectBindingsCount(Type type, ref int count)
        {
            if (s_bindingAttributes.TryGetValue(type, out IEnumerable<BindingAttribute> bindingAttributes))
                count += bindingAttributes.Count();
        }

        public static IEnumerable<CodeInstruction> InsertReadMaskPatchInstructions(IEnumerable<CodeInstruction> insertTo)
        {
            return insertTo.InsertBetween(Stloc_0, Ldloc_0, new CodeInstruction[]
            {
                new CodeInstruction(Ldarg_0),
                new CodeInstruction(Ldloca_S, 0),
                new CodeInstruction(Call, AccessTools.Method(typeof(BindingAttributes), nameof(BindingAttributes.CorrectBindingsCount)))
            });
        }

        private static IEnumerable<BindingAttribute> GetBindingAttributes(Type type)
        {
            if (s_bindingAttributes.TryGetValue(type, out IEnumerable<BindingAttribute> bindingAttributes))
                return bindingAttributes;

            return Enumerable.Empty<BindingAttribute>();
        }

        private static IEnumerable<BindingAttribute> FindBindingAttributes(Type type)
        {
            foreach (MemberInfo member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (member is not FieldInfo && member is not PropertyInfo)
                    continue;

                BindingAttribute bindingAttribute = member.GetCustomAttribute<BindingAttribute>();

                if (bindingAttribute is null)
                    continue;

                bindingAttribute.MemberName = member.Name;

                yield return bindingAttribute;
            }
        }
    }
}
