namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Props)]
    [BaggedProperty("previewPriority", true)]
    public class ActiveCamo : Holdable
    {
        private readonly SpriteMap _sprite = Resources.LoadSpriteMap("activeCamo", 8, 8);

        [Binding] private bool _used;

        private float _activateTimer = 0.15f;
        private bool _activating;

        public ActiveCamo(float x, float y) : base(x, y)    
        {
            graphic = _sprite;

            collisionSize = new Vec2(8f, 8f);
            center = collisionSize / 2f;
            collisionOffset = -center;
            weight = 3f;

            _holdOffset = new Vec2(-1f, 0f);
        }

        public override void Update()
        {
            base.Update();

            if (isServerForObject)
            {
                if (_used && grounded)
                {
                    Level.Remove(this);
                    return;
                }

                if (_activating)
                {
                    _activateTimer -= 0.01f;

                    if (_activateTimer <= 0f)
                    {
                        Activate();
                        DuckNetwork.SendToEveryone(new NMActivateCamo(this));

                        _activating = false;
                    }
                }
            }
        }

        public override void OnPressAction()
        {
            if (_used)
                return;

            if (isServerForObject)
            {
                _activating = true;
                _used = true;
            }
        }

        public void Activate()
        {
            material = new MaterialCamo(graphic, 0.02f);

            _sprite.frame++;

            if (duck is not null)
                Level.Add(new Camo(duck));
        }
    }
}
