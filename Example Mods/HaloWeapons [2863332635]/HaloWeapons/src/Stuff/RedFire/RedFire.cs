using System.Collections.Generic;
using System.Linq;

namespace DuckGame.HaloWeapons
{
    public static class RedFire
    {
        private static List<FireData> s_fireData = new List<FireData>(); 
        private static List<SmallFire> s_redFireParticles = new List<SmallFire>();

        public static void Add(SmallFire fire, bool local)
        {
            Add(new FireData(fire));

            if (local)
                DuckNetwork.SendToEveryone(new NMRedFire(fire));
        }

        public static void Add(ushort netIndex)
        {
            Add(new FireData(netIndex));
        }

        public static void Update()
        {
            foreach (SmallFire fire in Level.current.things.OfType<SmallFire>())
            {
                if (!s_redFireParticles.Contains(fire))
                {
                    foreach (FireData data in s_fireData)
                    {
                        if (data.Matches(fire))
                        {
                            ChangeTexture(fire);

                            return;
                        }
                    }

                    MaterialThing stick = fire.stick;

                    if (stick is not null && stick.onFire && s_redFireParticles.Contains(stick.lastBurnedBy))
                    {
                        Add(fire, true);
                        ChangeTexture(fire);
                    }
                }
            }
        }

        public static void OnChildSpawned(SmallFire child, SmallFire parent)
        {
            if (s_redFireParticles.Contains(parent))
                Add(child, true);
        }

        public static void Clear()
        {
            DevConsole.Log("Clear");

            s_fireData.Clear();
            s_redFireParticles.Clear();
        }

        private static void Add(FireData data)
        {
            s_fireData.Add(data);
        }

        private static void ChangeTexture(SmallFire fire)
        {
            fire.graphic.texture = Resources.LoadTexture("redFire.png");
            s_redFireParticles.Add(fire);
        }

        private sealed class FireData
        {
            private readonly SmallFire _fire;
            private readonly ushort? _netIndex;

            public FireData(SmallFire fire)
            {
                _fire = fire;
            }

            public FireData(ushort netIndex)
            {
                _netIndex = netIndex;
            }

            public bool Matches(SmallFire fire)
            {
                return _fire is not null && _fire == fire || _netIndex is not null && _netIndex.Value == fire.netIndex;
            }
        }
    }
}
