using System;
using System.Collections.Generic;
using System.Collections;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|hats")]
    public class CowboyHat : Hat
    {
        private Gun currentGun;

        public CowboyHat(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Cowboy Hat";

            // collision & sprite settings
            _pickupSprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\cowboyHatPickup"), 32, 32);
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\cowboyHat"), 32, 32);
            graphic = sprite;
            center = new Vec2(18f, 15f);
            _collisionSize = new Vec2(19f, 9f);
            _collisionOffset = new Vec2(-11f, -6f);

            // equipment settings
            _equippedThickness = 0.1f;
        }

        public override void Terminate()
        {
            if (equippedDuck != null && equippedDuck.gun != null)
            {
                Gun g = Activator.CreateInstance(equippedDuck.gun.GetType(), Editor.GetConstructorParameters(equippedDuck.gun.GetType())) as Gun;
                if (equippedDuck.gun.ammoType != null)
                    equippedDuck.gun.ammoType.accuracy = g.ammoType.accuracy;
                equippedDuck.gun.loseAccuracy = g.loseAccuracy;
            }
            if (currentGun != null)
            {
                Gun g = Activator.CreateInstance(currentGun.GetType(), Editor.GetConstructorParameters(currentGun.GetType())) as Gun;
                if (currentGun.ammoType != null)
                    currentGun.ammoType.accuracy = g.ammoType.accuracy;
                currentGun.loseAccuracy = g.loseAccuracy;
                currentGun = null;
            }
            base.Terminate();
        }

        public override void UnEquip()
        {
            if (equippedDuck != null && equippedDuck.gun != null)
            {
                Gun g = Activator.CreateInstance(equippedDuck.gun.GetType(), Editor.GetConstructorParameters(equippedDuck.gun.GetType())) as Gun;
                if (equippedDuck.gun.ammoType != null)
                    equippedDuck.gun.ammoType.accuracy = g.ammoType.accuracy;
                equippedDuck.gun.loseAccuracy = g.loseAccuracy;
            }
            if (currentGun != null)
            {
                Gun g = Activator.CreateInstance(currentGun.GetType(), Editor.GetConstructorParameters(currentGun.GetType())) as Gun;
                if (currentGun.ammoType != null)
                    currentGun.ammoType.accuracy = g.ammoType.accuracy;
                currentGun.loseAccuracy = g.loseAccuracy;
                currentGun = null;
            }
            base.UnEquip();
        }

        public override void Update()
        {
            if (equippedDuck != null)
            {
                if (currentGun != null && (equippedDuck.gun == null || currentGun != equippedDuck.gun))
                {
                    Gun g = Activator.CreateInstance(currentGun.GetType(), Editor.GetConstructorParameters(currentGun.GetType())) as Gun;
                    if (currentGun.ammoType != null)
                        currentGun.ammoType.accuracy = g.ammoType.accuracy;
                    currentGun.loseAccuracy = g.loseAccuracy;
                    currentGun = equippedDuck.gun;
                }
                if (equippedDuck.gun != null)
                {
                    currentGun = equippedDuck.gun;
                    if (equippedDuck.gun.ammoType != null)
                        equippedDuck.gun.ammoType.accuracy = 1f;
                    equippedDuck.gun.loseAccuracy = 0f;
                    if (equippedDuck.gun is Magnum)
                        ((Magnum)equippedDuck.gun).rise = 0f;
                }
            }
            else if (currentGun != null)
            {
                Gun g = Activator.CreateInstance(currentGun.GetType(), Editor.GetConstructorParameters(currentGun.GetType())) as Gun;
                if (currentGun.ammoType != null)
                    currentGun.ammoType.accuracy = g.ammoType.accuracy;
                currentGun.loseAccuracy = g.loseAccuracy;
                currentGun = null;
            }
            base.Update();
        }
    }
}
