namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Props)]
    public class DropWall : Holdable
    {
        private bool _canUnfold;

        public DropWall(float x, float y) : base(x, y)
        {
            graphic = Resources.LoadSprite("dropWall.png");
            weight = 4f;

            Utilities.SetCollisionBox(this, 10f, 6f);
        }

        public override void Update()
        {
            base.Update();

            if (grounded && _canUnfold && isServerForObject)
            {
                Level.Remove(this);

                Vec2 basePosition = position + collisionOffset;

                Level.Add(new DropWallBase(basePosition.x, basePosition.y)
                {
                    Direction = offDir
                });
            }
        }

        public override void Thrown()
        {
            base.Thrown();

            _canUnfold = true;
        }
    }
}
