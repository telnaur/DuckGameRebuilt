using System;

namespace DuckGame.UFFMod
{
    internal class BuzzsawBlade : PhysicsObject
    {
        public StateBinding _forceStateBinding = new StateBinding("force");

        private SpriteMap sprite;
        public float force;

        public BuzzsawBlade(float xpos, float ypos) :
            base(xpos, ypos)
        {
            //sprite
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\saw"), 20, 20);
            sprite.AddAnimation("roll", 1f, true, 0, 1, 2, 3);
            sprite.SetAnimation("roll");
            sprite.CenterOrigin();
            graphic = sprite;

            //collision and setup
            center = new Vec2(10f, 10f);
            collisionSize = new Vec2(4f, 4f);
            collisionOffset = new Vec2(-2f, -2f);
            weight = 3f;
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is Door || with is VerticalDoor || with is Window ||
                (with is PhysicsObject && !(with is BuzzsawBlade) && !(with is Duck) && !(with is RagdollPart)))
            {
                Fondle(with);
                with.Destroy(new DTRocketExplosion(this));
                if (with is Door || with is VerticalDoor || with is Window)
                    Level.Remove(with);
            }
            else if (with is Duck)
                with.Destroy(new DTImpale(this));
            else if(with is RagdollPart)
                ((RagdollPart)with)._doll._duck.Destroy(new DTImpale(this));
            hSpeed /= 1.3f;
            force /= 1.05f;
            base.OnImpact(with, from);
        }

        public override void Update()
        {
            base.Update();

            if (grounded)
            {
                if (Math.Abs(hSpeed) <= 0.1f)
                    Level.Remove(this);
            }

            foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(position, 8f))
            {
                if (materialThing is Door || materialThing is VerticalDoor || materialThing is Window ||
                    materialThing is PhysicsObject && !(materialThing is BuzzsawBlade) && !(materialThing is Duck) && !(materialThing is RagdollPart))
                {
                    Fondle(materialThing);
                    materialThing.Destroy(new DTRocketExplosion(this));
                    if (materialThing is Door || materialThing is VerticalDoor || materialThing is Window)
                        Level.Remove(materialThing);
                }
                else if (materialThing is Duck)
                    materialThing.Destroy(new DTImpale(this));
                else if (materialThing is RagdollPart)
                    ((RagdollPart)materialThing)._doll._duck.Destroy(new DTImpale(this));
            }
        }

        public override void Terminate()
        {
            if (!(Level.current is Editor))
            {
                Level.Add(SmallSmoke.New(x, y));
                Level.Add(SmallSmoke.New(x + 4f, y));
                Level.Add(SmallSmoke.New(x - 4f, y));
                Level.Add(SmallSmoke.New(x, y + 4f));
                Level.Add(SmallSmoke.New(x, y - 4f));
            }
            base.Terminate();
        }
    }
}