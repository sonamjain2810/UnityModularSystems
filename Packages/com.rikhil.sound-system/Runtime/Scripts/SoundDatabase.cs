using UnityEngine;
using Obvious.Soap;

namespace Rikhil.SoundSystem
{
    [CreateAssetMenu(fileName = "SoundDatabase", menuName = "SoundSystem/Sound Database")]
    public class SoundDatabase : ScriptableObject
    {
        [System.Serializable]
        public class SoundEntry
        {
            // NOTE: abhi category field hata diya; pehle compile theek karte hain
            public ScriptableEnumSoundTypeRegistry soundType; // 🔁 registry type
            public AudioClip clip;
            public bool loop;
        }

        public SoundEntry[] sounds;

        public AudioClip GetClip(ScriptableEnumSoundTypeRegistry type, out bool loop)
        {
            foreach (var entry in sounds)
            {
                if (entry.soundType == type)
                {
                    loop = entry.loop;
                    return entry.clip;
                }
            }

            loop = false;
            return null;
        }
    }
}
