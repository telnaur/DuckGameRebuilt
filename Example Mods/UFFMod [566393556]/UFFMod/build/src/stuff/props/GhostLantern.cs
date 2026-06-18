using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|props")]
    public class GhostLantern : Holdable, IPlatform
    {
        public StateBinding ghostSpawnedStateBinding = new StateBinding("GhostSpawned");
        public StateBinding unlitStateBinding = new StateBinding("Unlit");

        public bool GhostSpawned { get; set; }
        public bool Unlit { get; set; }

        private SpriteMap sprite;

        public GhostLantern(float xpos, float ypos, Duck theDuck = null)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Ghost Lantern";

            // general settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\ghostLantern"), 16, 20);
            sprite.AddAnimation("normal", 0.25f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            sprite.AddAnimation("unlit", 1f, false, 8);
            sprite.SetAnimation("normal");
            sprite.frame = Rando.Int(7);
            graphic = sprite;
            center = new Vec2(8f, 10f);
            collisionOffset = new Vec2(-8f, -4f);
            collisionSize = new Vec2(16f, 14f);
            _holdOffset = new Vec2(-2f, 7f);
            depth = -0.5f;
            thickness = 0.3f;
            weight = 2f;
            flammable = 0f;
            physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Update()
        {
            if (Unlit)
            {
                if (sprite.currentAnimation != "unlit")
                    sprite.SetAnimation("unlit");
            }
            else if (sprite.currentAnimation != "normal")
                sprite.SetAnimation("normal");

            if (isServerForObject && !GhostSpawned && Level.CheckCircle<Duck>(position, 160f) != null)
                {
                    GhostSpawned = true;
                    Level.Add(new Ghost(x, y - 12f, true, false, this));
                }

            base.Update();
        }
    }
}
