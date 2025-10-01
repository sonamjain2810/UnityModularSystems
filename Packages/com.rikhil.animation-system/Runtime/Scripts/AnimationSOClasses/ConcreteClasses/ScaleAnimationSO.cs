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
    /// - Has callback support for chaining animations.
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
        public bool setActiveOnPlay = true;
        public bool deactivateOnEnd = false;

        [Header("Loop Settings")]
        public bool loop = false;
        public int loopCount = -1;
        public DGTweenLoopType loopType = DGTweenLoopType.Yoyo;

        /// <summary>
        /// Play the scaling animation with an optional onComplete callback.
        /// </summary>
        public void Play(GameObject target, System.Action onComplete = null)
        {
            if (target == null)
            {
                onComplete?.Invoke();
                return;
            }

            KillTweens(target);
            var t = target.transform;

            if (setActiveOnPlay) target.SetActive(true);

            t.localScale = from;

            var tween = t.DOScale(to, Duration).SetEase(ease);

            if (loop)
            {
                tween.SetLoops(loopCount, loopType)
                     .OnComplete(() =>
                     {
                         // Even in looping case, invoke completion chain
                         onComplete?.Invoke();
                     });
            }
            else
            {
                tween.OnComplete(() =>
                {
                    if (deactivateOnEnd) target.SetActive(false);

                    RaiseOnComplete();
                    onComplete?.Invoke(); // 🔗 Trigger next animation
                });
            }
        }

        /// <summary>
        /// Backward compatibility with base Play().
        /// </summary>
        public override void Play(GameObject target)
        {
            Play(target, null);
        }
    }
}
