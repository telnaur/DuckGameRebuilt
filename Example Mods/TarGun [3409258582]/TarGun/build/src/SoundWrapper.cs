using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.TarGunMod
{
    public class SoundWrapper
    {
        private Sound _sound;
        public bool IsPlaying {get { return _sound != null; }}
        public void Play(string path, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool looped = false)
        {
            if (_sound == null)
            {
                _sound = SFX.Play(path, volume, pitch, pan, looped);
            }
            else
            {
                _sound.Volume = volume;
                _sound.Pitch = pitch;
            }
        }
        public void Stop()
        {
            if (_sound != null)
            {
                _sound.Stop();
                _sound = null;
            }
        }
        public void SetProperties(float volume, float pitch)
        {
            if (_sound != null)
            {
                _sound.Volume = volume;
                _sound.Pitch = pitch;
            }
        }
    }
}
