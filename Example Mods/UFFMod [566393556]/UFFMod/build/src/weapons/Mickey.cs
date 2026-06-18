using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    // based off decompiled Old Pistol

    [EditorGroup("uff|weapons|misc")]
    [BaggedProperty("isFatal", false)]
    public class Mickey : Gun
    {
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");
        public StateBinding netSFX_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAStateBinding = (StateBinding)new NetSoundBinding("netSFX_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");

        public NetSoundEffect netSFX_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\mickeyDuck")
        })
        {
            volume = 1f
        };

        private int _cooldown;

        public Mickey(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Mic";

            // collision & sprite settings
            graphic = new SpriteMap(Mod.GetPath<UffMod>("weapons\\mickey"), 11, 16);
            _center = new Vec2(6f, 8f);
            _collisionSize = new Vec2(8f, 14f);
            _collisionOffset = new Vec2(-4f, -7f);
            _holdOffset = new Vec2(3f, 2f);

            // weapon settings
            ammo = 3;
            _ammoType = new AT9mm();
            _ammoType.combustable = false;
            _weight = 2.5f;

            // defaults
            _cooldown = 0;
            _hasTrigger = false;
        }

        public override void Update()
        {
            if (ammo == 0)
            {
                Level.Remove(this);
                return;
            }

            if (_cooldown > 0)
                _cooldown--;

            _holdOffset = duck != null && duck.sliding ? new Vec2(-1f, 1f) : new Vec2(3f, 2f);
            handOffset.x = duck != null && duck.sliding ? 0f : Math.Abs(4f * (float)Math.Cos(angle));
            handOffset.y = duck != null && duck.sliding ? -2f : offDir * 4f * (float)Math.Sin(angle);

            base.Update();
        }

        public override void OnPressAction()
        {
            if (duck == null || _cooldown != 0)
                return;

            ammo--;
            _cooldown = 60;

            if (isServerForObject)
            {
                duck.quack = 10;
                Level.Add(new MicWave(x, y));
                Level.Add(new StunHandler(duck, fixH: true));
                netSFX_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA.Play();

                foreach (Duck d in Level.CheckCircleAll<Duck>(position, 80f))
                    if (d != duck && !d.HasEquipment(typeof(Earmuffs)))
                        Level.Add(new StunHandler(d, 120, showDaze: true));

                /*
                IList<Duck> duckList = new List<Duck>();
                foreach (RagdollPart ragdollPart in Level.CheckCircleAll<RagdollPart>(position, 80f))
                    if (ragdollPart._doll != null && ragdollPart._doll._duck != null && !duckList.Contains(ragdollPart._doll._duck))
                    {
                        Level.Add(new StunHandler(ragdollPart._doll._duck, 120, showDaze: true));
                        duckList.Add(ragdollPart._doll._duck);
                    }
                */
            }
        }

        public override void CheckIfHoldObstructed()
        {
            if(duck != null)
                duck.holdObstructed = false;
        }

        public override void Fire()
        {
            /* do nothing */
        }
    }
}
