using UnityEngine;

namespace Rikhil.AnimationSystem
{
    /// <summary>
    /// Singleton AnimationManager that executes animations
    /// based on AnimationDatabase lookups.
    /// </summary>
    [DisallowMultipleComponent]
    public class AnimationManager : MonoBehaviour
    {
        [SerializeField] private AnimationDatabase database;

        public static AnimationManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Plays an animation of the given type on the given target.
        /// </summary>
        public void Play(ScriptableEnumAnimationTypeRegistry animType, GameObject target)
        {
            if (animType == null || database == null || target == null)
            {
                Debug.LogWarning("[AnimationManager] Missing data for Play()");
                return;
            }

            var action = database.GetAction(animType);
            if (action == null)
            {
                Debug.LogWarning($"[AnimationManager] No action mapped for {animType.name}.");
                return;
            }

#if UNITY_EDITOR
            Debug.Log($"[AnimationManager] Playing {animType.name} on {target.name}");
#endif
            action.Play(target);
        }
    }
}
