using UnityEngine;
using Obvious.Soap;   // For ScriptableEventNoParam
using DG.Tweening;

namespace Rikhil.AnimationSystem
{
    /// <summary>
    /// Abstract base class for all Animation Actions in the Animation System.
    /// Each subclass defines a specific animation type (e.g., Scale, Fade, Move)
    /// and knows how to play it on a target GameObject using DOTween.
    /// 
    /// Usage:
    /// - Create a concrete subclass (e.g. ScaleAnimationSO).
    /// - Define its animation settings in the asset inspector.
    /// - Assign it to the AnimationDatabase with a matching enum type.
    /// - AnimationManager will call Play() when triggered.
    /// </summary>
    public abstract class AnimationActionSO : ScriptableObject
    {
        [Header("Common Animation Settings")]

        [Tooltip("Duration (in seconds) for the animation to complete.")]
        [Min(0f)]
[SerializeField] private float duration = 0.45f;

/// <summary>
/// Exposes the animation duration so sequences can time correctly.
/// </summary>
public virtual float Duration => duration;

        [Tooltip("DOTween easing function to control the animation curve.")]
        public Ease ease = Ease.OutBack;

        [Tooltip("If true, kills any existing tweens on the target before starting a new one.")]
        public bool killExistingTweens = true;

        [Header("Events")]

        [Tooltip("Optional SOAP event raised when the animation finishes.\n" +
                 "⚠️ Only called for non-looping animations.")]
        public ScriptableEventNoParam onCompleteEvent;

        /// <summary>
        /// Plays the animation on the specified GameObject.
        /// Each subclass must implement this.
        /// </summary>
        /// <param name="target">The GameObject to animate.</param>
        public abstract void Play(GameObject target);

        /// <summary>
        /// Raises the optional onComplete SOAP event (if assigned).
        /// Should be called at the end of non-looping animations.
        /// </summary>
        protected void RaiseOnComplete()
        {
            onCompleteEvent?.Raise();
        }

        /// <summary>
        /// Kills all DOTweens associated with the target’s Transform (and CanvasGroup if present).
        /// Ensures clean restart of the animation without stacking tweens.
        /// </summary>
        protected void KillTweens(GameObject target)
        {
            if (target == null) return;
            if (!killExistingTweens) return;

            // Kill tweens on Transform
            target.transform.DOKill(true);

            // Kill tweens on CanvasGroup (optional UI alpha tweens)
            if (target.TryGetComponent<CanvasGroup>(out var cg))
                cg.DOKill();
        }
    }
}
