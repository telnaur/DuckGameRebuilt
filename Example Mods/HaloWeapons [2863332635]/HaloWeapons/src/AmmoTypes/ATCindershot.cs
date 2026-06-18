namespace DuckGame.HaloWeapons
{
    public class ATCindershot : ATReboundBullet
    {
        private readonly Color _impulseCircleColor = new Color(172, 34, 144);
        private readonly float _impulseRadius = 20f;

        public ATCindershot()
        {
            sprite = Resources.LoadSprite("cindershotBullet.png", true);

            affectedByGravity = true;
            bulletThickness = 3f;
            range = 500f;
            bulletSpeed = 6f;
            penetration = 1f;
            bulletColor = new Color(241, 92, 211); 
            bulletType = typeof(CindershotBullet);
            combustable = true;
            flawlessPipeTravel = true;
        }

        public override void OnHit(bool destroyed, Bullet bullet)
        {
            if (destroyed)
            {
                Vec2 position = bullet.position;

                for (int i = 0; i < Rando.Int(2, 3); i++)
                {
                    Color color = Lerp.Color(_impulseCircleColor, Color.White, Rando.Float(0.8f));

                    float radius = Rando.Float(_impulseRadius / 3f);
                    float radiusIncrease = Rando.Float(0.5f, 2f);
                    float width = Rando.Float(2f, 3.5f);
                    int lifeDuration = Rando.Int(10, 15);

                    Level.Add(new FadingCircle(position, color, radius, radiusIncrease, width, lifeDuration));
                }

                if (bullet.isLocal)
                    foreach (MaterialThing thing in Level.CheckCircleAll<MaterialThing>(position, _impulseRadius))
                        thing.Destroy(new DTImpact(null));
            }
        }
    }
}
