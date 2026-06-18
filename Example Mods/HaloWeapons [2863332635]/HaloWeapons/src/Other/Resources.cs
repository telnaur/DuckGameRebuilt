using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using XnaContentProvider;

namespace DuckGame.HaloWeapons
{
    internal static class Resources
    {
        private static Dictionary<string, Texture2D> s_texturesCached = new Dictionary<string, Texture2D>();
        private static Dictionary<string, byte[]> s_shadersCode = new Dictionary<string, byte[]>();

        public static void LoadShaders()
        {
            string shadersDirectory = Paths.ShadersDirectory;

#if DEBUG
            var context = new CpC();
            var processor = new EffectProcessor();

            foreach (string path in Directory.GetFiles(Paths.ShadersSourceDirectory))
            {
                if (Path.GetExtension(path) == ".fx")
                {
                    var content = new EffectContent()
                    {
                        EffectCode = File.ReadAllText(path),
                    };

                    CompiledEffectContent effectContent = processor.Process(content, context);

                    byte[] code = effectContent.GetEffectCode();
                    string fileName = Path.GetFileName(path);
                    string compiledFileName = Path.ChangeExtension(fileName, ".xnb");
                    string compiledPath = Path.Combine(shadersDirectory, compiledFileName);

                    File.WriteAllBytes(compiledPath, code);

                    s_shadersCode.Add(compiledPath, code);
                }
            }
#else
            foreach (string path in Directory.GetFiles(shadersDirectory))
            {
                if (Path.GetExtension(path) == ".xnb")
                {
                    byte[] code = File.ReadAllBytes(path);

                    s_shadersCode.Add(path, code);
                }
            }
#endif
        }

        public static Sprite LoadSprite(string fileName, bool centerOrigin = false)
        {
            var sprite = new Sprite(Paths.GetSpritePath(fileName));

            if (centerOrigin)
                sprite.CenterOrigin();

            return sprite;
        }

        public static SpriteMap LoadSpriteMap(string fileName, int frameWidth, int frameHeight)
        {
            return new SpriteMap(Paths.GetSpritePath(fileName), frameWidth, frameHeight);
        }

        public static Texture2D LoadTexture(string fileName)
        {
            string path = Paths.GetSpritePath(fileName);    

            if (s_texturesCached.TryGetValue(path, out Texture2D texture))
                return texture;

            using (FileStream stream = File.OpenRead(path))
            {
                texture = Texture2D.FromStream(Graphics.device, stream);
                s_texturesCached.Add(path, texture);

                return texture;
            }
        }

        public static Sound LoadSound(string fileName, float volume = 1f, float pitch = 0f, float pan = 0f, bool looped = false)
        {
            return SFX.Get(Paths.GetSoundPath(fileName), volume, pitch, pan, looped);
        }

        public static MTEffect LoadMTEffect(string fileName)
        {
            string path = Paths.GetShaderPath(fileName);

            if (!s_shadersCode.TryGetValue(path, out byte[] shaderCode))
                return null;

            var effect = new Effect(Graphics.device, shaderCode);

            return new MTEffect(effect, Path.GetFileNameWithoutExtension(fileName));
        }
    }
}
