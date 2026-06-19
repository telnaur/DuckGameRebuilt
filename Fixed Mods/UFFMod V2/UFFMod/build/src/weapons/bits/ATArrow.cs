namespace DuckGame.UFFMod
{
    internal class ATArrow : AmmoType
    {
        public ATArrow()
        {
            accuracy = 1f;
            range = 1000f;
            penetration = 0.2f;
            bulletSpeed = 6f;
            bulletThickness = 1.2f;
            affectedByGravity = true;
            sprite = new Sprite(Mod.GetPath<UffMod>("weapons\\arrow"), 0f, 0f);
            sprite.CenterOrigin();
        }

        public ATArrow(bool ballista)
        {
            accuracy = 1f;
            range = 1000f;
            bulletThickness = 1.2f;
            affectedByGravity = true;
            if (ballista)
            {
                penetration = 2f;
                bulletSpeed = 38f;
                sprite = new Sprite(Mod.GetPath<UffMod>("weapons\\ballistaArrow"), 0f, 0f);
            }
            else
            {
                penetration = 0.2f;
                bulletSpeed = 6f;
                sprite = new Sprite(Mod.GetPath<UffMod>("weapons\\arrow"), 0f, 0f);
            }
            sprite.CenterOrigin();
        }

        public override void OnHit(bool destroyed, Bullet b)
        {
            if (penetration >= 2f)
                foreach (Door door in Level.CheckCircleAll<Door>(b.position, 2f))
                {
                    if (b.isLocal)
                        Thing.Fondle(door, DuckNetwork.localConnection);
                    if (Level.CheckLine<Block>(b.position, door.position, door) == null)
                        door.Destroy(new DTImpact(b));
                }
            base.OnHit(destroyed, b);
        }
    }
}
