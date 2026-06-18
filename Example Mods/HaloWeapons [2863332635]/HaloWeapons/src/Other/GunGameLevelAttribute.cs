using System;

namespace DuckGame.HaloWeapons
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GunGameLevelAttribute : Attribute
    {
        private readonly int _value;

        public GunGameLevelAttribute(int value)
        {
            _value = value;
        }

        public int Value => _value;
    }
}
