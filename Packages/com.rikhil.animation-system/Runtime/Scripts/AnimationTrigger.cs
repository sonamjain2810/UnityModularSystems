using UnityEngine;
using Obvious.Soap;

namespace Rikhil.AnimationSystem
{
    [DisallowMultipleComponent]
    public class AnimationTrigger : MonoBehaviour
    {
        [Header("Animation Type")]
        [SerializeField] private ScriptableEnumAnimationTypeRegistry animationType;

        [Header("Target (Optional)")]
        [SerializeField] private GameObject target;

        [Header("Events (SOAP)")]
        [SerializeField] private ScriptableEventAnimationType onPlayAnimation;

        [Header("Auto Play")]
        [SerializeField] private bool playOnEnable = true;

        public ScriptableEnumAnimationTypeRegistry AnimationType => animationType;
        public GameObject Target => target != null ? target : gameObject;

        private void Reset() => target = gameObject;

        private void OnEnable()
        {
            // Subscribe to SOAP event
            if (onPlayAnimation != null)
                onPlayAnimation.OnRaised += HandleAnimationEvent;

            // Auto-play when enabled
            if (playOnEnable && animationType != null && AnimationManager.Instance != null)
                AnimationManager.Instance.Play(animationType, Target);
        }

        private void OnDisable()
        {
            // Unsubscribe from SOAP event
            if (onPlayAnimation != null)
                onPlayAnimation.OnRaised -= HandleAnimationEvent;
        }

        private void HandleAnimationEvent(ScriptableEnumAnimationTypeRegistry raisedType)
        {
            if (raisedType == animationType && AnimationManager.Instance != null)
            {
                AnimationManager.Instance.Play(animationType, Target);
            }
        }
    }
}
