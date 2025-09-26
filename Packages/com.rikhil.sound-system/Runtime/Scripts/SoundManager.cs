using UnityEngine;
using Obvious.Soap;

namespace Rikhil.SoundSystem
{
    public class SoundManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScriptableEventSoundType onPlaySound;
        [SerializeField] private BoolVariable musicEnabled;
        [SerializeField] private BoolVariable sfxEnabled;
        [SerializeField] private SoundDatabase soundDatabase;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Background Music")]
        [SerializeField] private ScriptableEnumSoundTypeRegistry backgroundMusicType;

        private AudioClip currentMusicClip;

        private void OnEnable()
        {
            // Listen for sound play events
            onPlaySound.OnRaised += HandlePlaySound;

            // Listen for runtime BoolVariable changes
            musicEnabled.OnValueChanged += OnMusicSettingChanged;
            sfxEnabled.OnValueChanged += OnSfxSettingChanged;
        }

        private void OnDisable()
        {
            onPlaySound.OnRaised -= HandlePlaySound;

            musicEnabled.OnValueChanged -= OnMusicSettingChanged;
            sfxEnabled.OnValueChanged -= OnSfxSettingChanged;
        }

        private void Start()
        {
            // Play background music on start if assigned and enabled
            if (backgroundMusicType != null)
            {
                //HandlePlaySound(backgroundMusicType);
            }
        }

        /// <summary>
        /// Handles when a sound event is raised
        /// </summary>
        private void HandlePlaySound(ScriptableEnumSoundTypeRegistry soundType)
        {
            if (!soundDatabase) return;

            var clip = soundDatabase.GetClip(soundType, out bool loop);
            if (!clip) return;

            if (loop)
            {
                currentMusicClip = clip; // store for restart if toggle re-enabled
                PlayMusic(clip);
            }
            else
            {
                PlaySfx(clip);
            }
        }

        /// <summary>
        /// Play looping background music
        /// </summary>
        private void PlayMusic(AudioClip clip)
        {
            if (!musicEnabled.Value || !musicSource) return;
#if UNITY_IOS || UNITY_ANDROID
            if (AudioSettings.Mobile.muteState) return;
#endif
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        /// <summary>
        /// Play one-shot sound effect
        /// </summary>
        private void PlaySfx(AudioClip clip)
        {
            if (!sfxEnabled.Value || !sfxSource) return;
#if UNITY_IOS || UNITY_ANDROID
            if (AudioSettings.Mobile.muteState) return;
#endif
            sfxSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Called whenever the musicEnabled BoolVariable changes
        /// </summary>
        private void OnMusicSettingChanged(bool isEnabled)
        {
            if (!musicSource) return;

            if (isEnabled)
            {
                // Restart music if a clip was assigned
                if (currentMusicClip != null)
                    PlayMusic(currentMusicClip);
            }
            else
            {
                // Stop immediately
                musicSource.Stop();
            }
        }

        /// <summary>
        /// Called whenever the sfxEnabled BoolVariable changes
        /// </summary>
        private void OnSfxSettingChanged(bool isEnabled)
        {
            // Optional: Could fade-out or clear sfx queue
            // For now nothing needed since PlaySfx() already checks sfxEnabled
        }
    }
}
