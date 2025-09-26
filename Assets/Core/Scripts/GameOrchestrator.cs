using UnityEngine;
using Obvious.Soap;
using Rikhil.GameStateSystem;   // for ScriptableEnumGameState
using Rikhil.SoundSystem;      // for ScriptableEventSoundType + ScriptableEnumSoundTypeRegistry
using Rikhil.AnimationSystem;  // for ScriptableEventAnimationType + ScriptableEnumAnimationTypeRegistry

namespace Rikhil.Core
{
    /// <summary>
    /// Acts as the backbone of the game.
    /// Listens to GameState changes and orchestrates:
    /// - Background music (via SoundSystem)
    /// - Animations (via AnimationSystem)
    /// </summary>
    public class GameOrchestrator : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private ScriptableEventScriptableEnumGameState onGameStateChanged;
        [SerializeField] private ScriptableEventSoundType onPlaySound;
        [SerializeField] private ScriptableEventAnimationType onPlayAnimation;

        [Header("States")]
        [SerializeField] private ScriptableEnumGameState mainMenuState;

        [SerializeField] private ScriptableEnumGameState gamePlayState;

        [Header("Sound Types")]
        [SerializeField] private ScriptableEnumSoundTypeRegistry mainMenuBackGroundMusic;

        [Header("Animation Types")]
        [SerializeField] private ScriptableEnumAnimationTypeRegistry titleAnimationType;
        [SerializeField] private ScriptableEnumAnimationTypeRegistry playButtonAnimationType;

        [Header("GamePlay Animation Types")]
        [SerializeField] private ScriptableEnumAnimationTypeRegistry imageAnimationType;
        [SerializeField] private ScriptableEnumAnimationTypeRegistry alphabetAnimationType;
        [SerializeField] private ScriptableEnumAnimationTypeRegistry scaleFastAnimationType;


        private void OnEnable()
        {
            if (onGameStateChanged != null)
                onGameStateChanged.OnRaised += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            if (onGameStateChanged != null)
                onGameStateChanged.OnRaised -= HandleGameStateChanged;
        }

        private void HandleGameStateChanged(ScriptableEnumGameState state)
        {
            if (state == mainMenuState)
            {
                // 🔊 Play Background Music
                if (onPlaySound != null && mainMenuBackGroundMusic != null)
                    onPlaySound.Raise(mainMenuBackGroundMusic);

                // 🎬 Animate Title
                if (onPlayAnimation != null && titleAnimationType != null)
                    onPlayAnimation.Raise(titleAnimationType);

                // 🎬 Animate Title
                if (onPlayAnimation != null && playButtonAnimationType != null)
                    onPlayAnimation.Raise(playButtonAnimationType);
            }

            else if (state == gamePlayState)
            {
                // 🎬 Animate Text
                PlayAnimation(alphabetAnimationType);
                PlayAnimation(titleAnimationType);
                PlayAnimation(scaleFastAnimationType);
            }
            else
            {

            }
        }
        
        private void PlayAnimation(ScriptableEnumAnimationTypeRegistry type)
        {
            if (onPlayAnimation != null && type != null)
                onPlayAnimation.Raise(type);
        }
    }
}
