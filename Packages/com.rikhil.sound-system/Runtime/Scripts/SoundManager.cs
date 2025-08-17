
using UnityEngine;
// using Obvious.Soap; // Uncomment when your PlaySound event type is defined

namespace Rikhil.SoundSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.playOnAwake = false;
            }
        }

        public void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null || audioSource == null) return;
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
