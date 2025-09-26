using UnityEngine;
using DG.Tweening;
using DGTweenLoopType = DG.Tweening.LoopType;


namespace Rikhil.AnimationSystem
{
    /// <summary>
    /// A data-driven scaling animation using DOTween.
    /// - Scales a target GameObject from one value to another.
    /// - Can optionally activate/deactivate the GameObject.
    /// - Supports looping (Restart, Yoyo, Incremental).
    /// </summary>
    [CreateAssetMenu(menuName = "AnimationSystem/Scale Animation", fileName = "ScaleAnimation")]
    public class ScaleAnimationSO : AnimationActionSO
    {
        [Header("Scale Settings")]
        [Tooltip("The scale to start from when the animation begins.")]
        public Vector3 from = Vector3.zero;

        [Tooltip("The scale to animate towards during the animation.")]
        public Vector3 to = Vector3.one;

        [Header("Activation Options")]
        [Tooltip("If true, ensures the GameObject is active before playing the animation.")]
        public bool setActiveOnPlay = true;

        [Tooltip("If true, deactivates the GameObject once the animation finishes. Ignored if looping.")]
        public bool deactivateOnEnd = false;

        [Header("Loop Settings")]
        [Tooltip("If true, the animation will loop based on Loop Count and Loop Type.")]
        public bool loop = false;

        [Tooltip("How many times the animation should loop.\n-1 = infinite.")]
        public int loopCount = -1;

        [Tooltip("The type of looping to use (Restart, Yoyo, Incremental).")]
        public DG.Tweening.LoopType loopType = DG.Tweening.LoopType.Yoyo;


        /// <summary>
        /// Play the scaling animation on a target GameObject.
        /// </summary>
        /// <param name="target">The GameObject to animate.</param>
        public override void Play(GameObject target)
        {
            if (target == null) return;

            // Kill any running tweens on this target before starting a new one
            KillTweens(target);

            var t = target.transform;

            // Optionally activate the object before animating
            if (setActiveOnPlay) target.SetActive(true);

            // Force starting scale
            t.localScale = from;

            // Create scale tween
            var tween = t.DOScale(to, Duration).SetEase(ease);

            // If looping enabled → repeat animation
            if (loop)
            {
                tween.SetLoops(loopCount, loopType);
            }
            else
            {
                // When not looping → complete normally
                tween.OnComplete(() =>
                {
                    // Optionally deactivate object after animation ends
                    if (deactivateOnEnd) target.SetActive(false);

                    // Raise global onComplete event (if any listeners are connected)
                    RaiseOnComplete();
                });
            }
        }
    }
}
