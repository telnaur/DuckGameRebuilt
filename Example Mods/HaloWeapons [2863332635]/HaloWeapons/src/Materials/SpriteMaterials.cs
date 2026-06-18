using System.Collections.Generic;

namespace DuckGame.HaloWeapons
{
    public static class SpriteMaterials
    {
        private static readonly Dictionary<Sprite, Material> s_materials = new Dictionary<Sprite, Material>();  

        public static void Update()
        {
            foreach (Material material in s_materials.Values)
                material.Update();
        }

        public static void OnSpriteDrawStart(Sprite sprite)
        {
            if (s_materials.TryGetValue(sprite, out var material))  
                Graphics.material = material;   
        }

        public static void OnSpriteDrawFinished()
        {
            Graphics.material = null;
        }

        public static void Add(Sprite sprite, Material material)
        {
            if (s_materials.ContainsKey(sprite))
            {
                s_materials[sprite] = material;
                return;
            }

            s_materials.Add(sprite, material);
        }

        public static bool Remove(Sprite sprite)
        {
            return s_materials.Remove(sprite);
        }

        public static void Clear()
        {
            s_materials.Clear();
        }
    }
}
