namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Guns)]
    [GunGameLevel(5)]
    public class PulseCarbine : BurstingWeapon
    {
        public PulseCarbine(float x, float y) : base(x, y)
        {
            FireDelay = 3;
            BurstDelay = 3;

            ammo = 90;

            _ammoType = new ATPulseCarbine();
            _holdOffset = new Vec2(4f, 2f);
            _kickForce = 1f;
            _barrelOffsetTL = new Vec2(34f, 4f);
            _fireSound = Paths.GetSoundPath("pulseCarbineFire.wav");
        }

        public override void OnPressAction()
        {
            if (isServerForObject && CanStartBursting && ammo > 0)
                heat += 0.2f;

            base.OnPressAction();
        }
    }
}
