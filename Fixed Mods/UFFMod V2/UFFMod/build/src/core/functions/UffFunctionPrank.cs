namespace DuckGame.UFFMod
{
    internal class UffFunctionPrank : UffFunction
    {
        private string prankType;

        public UffFunctionPrank(string _prankType)
        {
            prankType = _prankType;
        }

        public override void Call(Duck duck)
        {
            switch (prankType)
            {
                case UffMain.PrankGodmode:
                    if (duck.isServerForObject)
                    {
                        foreach (Godhead gh in Level.current.things[typeof(Godhead)])
                            if (gh._theDuck.Equals(duck))
                            {
                                Level.Remove(gh);
                                return;
                            }
                        Level.Add(new Godhead(duck));
                    }
                    break;

                case UffMain.PrankWaltz:
                    if (duck.isServerForObject)
                        foreach (Gun g in Level.current.things[typeof(Gun)])
                        {
                            if (g.owner == null)
                                Thing.Fondle(g, DuckNetwork.localConnection);
                            g.PressAction();
                        }
                    break;

                case UffMain.PrankTrollbombs:
                    if (duck.isServerForObject)
                        foreach (Duck d in Level.current.things[typeof(Duck)])
                        {
                            TrollBomb trollBomb = new TrollBomb(d.x, d.y);
                            Level.Add(trollBomb);
                            trollBomb.vSpeed = -1.5f;
                            if (d.ragdoll != null)
                                trollBomb.position = d.ragdoll.position;
                        }
                    break;

                case UffMain.PrankRoast:
                    foreach (Duck d in Level.current.things[typeof(Duck)])
                        if (!d.Equals(duck))
                            d.Kill(new DTIncinerate(duck));
                    break;

                case UffMain.PrankIce:
                    if (duck.isServerForObject)
                    {
                        IceCubeUFFEdition iceCube = new IceCubeUFFEdition(duck.x, duck.y);
                        Level.Add(iceCube);
                        iceCube.vSpeed = -1.5f;
                        if (duck.ragdoll != null)
                            iceCube.position = duck.ragdoll.position;
                    }
                    duck.Kill(new DTImpale(duck));
                    Level.Remove(duck);
                    if (duck.ragdoll != null)
                        Level.Remove(duck.ragdoll);
                    break;

                case UffMain.PrankGolden:
                    if (duck.gun != null)
                        duck.gun.infinite = true;
                    break;
            }
        }
    }
}
