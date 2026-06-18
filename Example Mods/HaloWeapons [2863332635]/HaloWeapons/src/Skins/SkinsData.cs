using System;
using System.Collections.Generic;

namespace DuckGame.HaloWeapons
{
    public sealed class SkinsData
    {
        public int Credits { get; set; }

        public Dictionary<Type, int[]> OwnedSkins { get; set; } = new Dictionary<Type, int[]>();

        public Dictionary<Type, int> EquippedSkins { get; set; } = new Dictionary<Type, int>();
    }
}
