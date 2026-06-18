using System.Collections.Generic;
using System.Linq;

namespace DuckGame.HaloWeapons
{
    public class Camo : Thing
    {
        private readonly Duck _duck;
        private readonly List<MaterialCamo> _materials = new List<MaterialCamo>();
        private readonly HashSet<Equipment> _invisibleEquipment = new HashSet<Equipment>();
        private readonly Vec2 _camoCenterOffset = new Vec2(-1f, 3f);

        private readonly float _bodySpriteDeltaTime = 0.006f;
        private readonly float _wingSpriteDeltaTime = 0.01f;

        private float _timer = 4f;

        public Camo(Duck duck)
        {
            _duck = duck;
        }

        public Duck Duck => _duck;

        private bool LocalDuck => _duck.isServerForObject;

        public override void Update()
        {
            _timer -= 0.01f;

            if (LocalDuck)
            {
                foreach (Equipment equipment in _duck._equipment)
                {
                    if (equipment is TeamHat hat)
                        continue;

                    equipment.visible = false;
                    _invisibleEquipment.Add(equipment);
                }
            }

            if (_timer <= 0f)
            {
                foreach (MaterialCamo material in _materials)
                    material.StartDeactivating();

                PlaySound("activeCamoDeactivate.wav");

                if (LocalDuck)
                {
                    foreach (Equipment equipment in _invisibleEquipment)
                    {
                        Duck equippedDuck = equipment.equippedDuck;

                        if (equippedDuck is null || equippedDuck == _duck)
                            equipment.visible = true;
                    }
                }


                Level.Remove(this);
            }
        }

        public override void Initialize()
        {
            foreach (Camo camo in Level.current.things.OfType<Camo>())
                if (camo.Duck == _duck)
                    Level.Remove(camo);

            if (LocalDuck)
                foreach (Equipment equipment in new List<Equipment>(_duck._equipment.Where(equipment => equipment is Helmet || equipment is TeamHat)))
                    _duck.Unequip(equipment);

            Sprite bodySprite = _duck._sprite;
            Sprite quackSprite = _duck._spriteQuack;
            Sprite wingSprite = _duck._spriteArms;

            AddMaterial(bodySprite, new MaterialCamo(bodySprite, _bodySpriteDeltaTime)
            {
                CenterOffset = _camoCenterOffset
            });

            AddMaterial(quackSprite, new MaterialCamo(quackSprite, _bodySpriteDeltaTime)
            {
                CenterOffset = _camoCenterOffset
            });

            AddMaterial(wingSprite, new MaterialCamo(wingSprite, _wingSpriteDeltaTime));

            PlaySound("activeCamoActivate.wav");
        }

        private void AddMaterial(Sprite sprite, MaterialCamo material)
        {
            SpriteMaterials.Add(sprite, material);
            _materials.Add(material);
        }

        private void PlaySound(string fileName)
        {
            SFX.Play(Paths.GetSoundPath(fileName), 0.35f);
        }
    }
}
