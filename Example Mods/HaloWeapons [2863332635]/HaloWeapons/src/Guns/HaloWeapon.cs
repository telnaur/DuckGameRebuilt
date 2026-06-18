namespace DuckGame.HaloWeapons
{
    public abstract class HaloWeapon : Gun
    {
        public readonly StateBinding PickedUpBinding = new StateBinding("HasBeenPickedUp");

        public HaloWeapon(float x, float y) : base(x, y)
        {
            _type = "gun";

            SetGraphics(Skins.GetDefaultSkinPath(GetType()));
            SetSpriteCollisionBox();
        }

        public bool HasBeenPickedUp { get; protected set; }

        protected SpriteMap SpriteMap => graphic as SpriteMap;

        public override void Update()
        {
            base.Update();

            if (duck is not null && duck.inputProfile.Pressed("QUACK"))
                OnQuack();

            if (isServerForObject && !HasBeenPickedUp && owner is Duck)
            {
                HasBeenPickedUp = true;
                OnPickUp();
            }
        }

        public void SetGraphics(string spritePath)
        {
            graphic = CreateGraphics(spritePath);
            DevConsole.Log(spritePath);
        }

        public void SetGraphics(int skinIndex)
        {
            SetGraphics(Skins.GetSkinPath(GetType(), skinIndex));
        }

        protected virtual Sprite CreateGraphics(string spritePath)
        {
            return new Sprite(spritePath);  
        }

        protected virtual void OnQuack()
        {

        }

        protected void SetCollisionBox(float width, float height)
        {
            Utilities.SetCollisionBox(this, width, height);
        }

        protected void SetSpriteCollisionBox()
        {
            if (graphic is null)
                return;

            SetCollisionBox(graphic.w, graphic.h);
        }

        protected void DoFireSynchronized()
        {
            Fire();
            Send.Message(new NMFireGun(this, firedBullets, bulletFireIndex, false, duck is null ? (byte)4 : duck.netProfileIndex, true), NetMessagePriority.Urgent);
            firedBullets.Clear();
        }

        private void OnPickUp()
        {
            if (Skins.TryGetEquippedIndex(GetType(), out int index))
            {
                string animation = string.Empty;
                int frame = 0;

                if (SpriteMap is not null)
                {
                    string currentAnimation = SpriteMap.currentAnimation;

                    if (currentAnimation is not null)
                        animation = string.Copy(currentAnimation);

                    frame = SpriteMap.frame;
                }

                SetGraphics(index);

                if (Options.Data.ShareSkins)
                    DuckNetwork.SendToEveryone(new NMSetSkin(this, index));

                if (SpriteMap is not null)
                {
                    SpriteMap.SetAnimation(animation);
                    SpriteMap.frame = frame;
                }
            }
        }
    }
}
