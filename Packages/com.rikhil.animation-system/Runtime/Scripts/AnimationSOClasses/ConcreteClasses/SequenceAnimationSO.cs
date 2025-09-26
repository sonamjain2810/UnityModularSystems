using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using DGTweenLoopType = DG.Tweening.LoopType;

namespace Rikhil.AnimationSystem
{
    /// <summary>
    /// A composite animation that plays multiple AnimationActionSO steps in sequence.
    /// - Supports multiple targets or a single passed-in target.
    /// - Respects startDelay, looping, and loopType.
    /// - Waits for each step's Duration before moving to the next.
    /// </summary>
    [CreateAssetMenu(menuName = "AnimationSystem/Sequence Animation", fileName = "SequenceAnimation")]
    public class SequenceAnimationSO : AnimationActionSO
    {
        [Header("Steps in the sequence")]
        [Tooltip("Each step is another AnimationActionSO (Scale, Fade, Punch, etc.)")]
        public List<AnimationActionSO> steps = new();

        [Header("Targets")]
        [Tooltip("If empty → will use the one passed to Play(target). Otherwise plays on all listed targets in order.")]
        public List<GameObject> predefinedTargets = new();

        [Header("Sequence Settings")]
        [Tooltip("Delay before the entire sequence starts.")]
        public float startDelay = 0f;

        [Tooltip("How many times the entire sequence should repeat. (-1 = infinite)")]
        public int loopCount = 0;

        [Tooltip("How the sequence loops (Restart, Yoyo, Incremental).")]
        public DG.Tweening.LoopType loopType = DG.Tweening.LoopType.Restart;



        public override void Play(GameObject target)
        {
            // Determine targets
            List<GameObject> targets = predefinedTargets.Count > 0
                ? predefinedTargets
                : new List<GameObject> { target };

            if (targets == null || targets.Count == 0 || steps.Count == 0)
                return;

            // Kill any tweens running on targets
            foreach (var t in targets)
                KillTweens(t);

            // Create a sequence
            Sequence seq = DOTween.Sequence();

            // Global start delay
            if (startDelay > 0)
                seq.AppendInterval(startDelay);

            // Build sequence for each target
            foreach (var t in targets)
            {
                foreach (var step in steps)
                {
                    if (step == null) continue;

                    // Append step play
                    seq.AppendCallback(() => step.Play(t));

                    // Wait for its duration before moving to next
                    seq.AppendInterval(step.Duration);
                }
            }

            // Apply looping if requested
            if (loopCount != 0)
                seq.SetLoops(loopCount, loopType);

            // Completion callback
            seq.OnComplete(() =>
            {
                RaiseOnComplete();
            });
        }
    }
}
