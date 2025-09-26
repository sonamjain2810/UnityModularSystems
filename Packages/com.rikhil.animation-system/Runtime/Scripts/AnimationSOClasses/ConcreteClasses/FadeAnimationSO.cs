using UnityEngine;
using UnityEngine.UI;       // For Image, Text, RawImage, etc.
using DG.Tweening;
using DGTweenLoopType = DG.Tweening.LoopType;

namespace Rikhil.AnimationSystem
{
    /// <summary>
    /// Fade animation using DOTween.
    /// Supports:
    /// - CanvasGroup (recommended for fading an entire UI container).
    /// - UI Graphic (Image, Text, RawImage, etc.).
    ///
    /// Features:
    /// - Fades alpha from "from" → "to".
    /// - Optional activation/deactivation of GameObject.
    /// - Supports looping and blink mode.
    /// - Works without requiring DOTween.UI module in the package (uses DOTween.To / ToAlpha).
    /// </summary>
    [CreateAssetMenu(menuName = "AnimationSystem/Fade Animation", fileName = "FadeAnimation")]
    public class FadeAnimationSO : AnimationActionSO
    {
        [Header("Fade Settings")]
        [Tooltip("Starting alpha value (0 = invisible, 1 = fully visible).")]
        [Range(0f, 1f)] public float from = 0f;

        [Tooltip("Target alpha value (0 = invisible, 1 = fully visible).")]
        [Range(0f, 1f)] public float to = 1f;

        [Header("Blink Settings")]
        [Tooltip("If true → fades repeatedly from 'from' ↔ 'to'.")]
        public bool blink = false;

        [Tooltip("Blink interval in seconds (time for one fade direction).")]
        [Min(0.01f)] public float blinkInterval = 0.5f;

        [Header("Activation Options")]
        [Tooltip("If true, ensures GameObject is active before playing animation.")]
        public bool setActiveOnPlay = true;

        [Tooltip("If true, deactivates GameObject when animation finishes (ignored if looping/blink).")]
        public bool deactivateOnEnd = false;

        [Header("Loop Settings")]
        [Tooltip("If true, animation loops with given count and type (ignored if Blink is true).")]
        public bool loop = false;

        [Tooltip("Number of loops (-1 = infinite).")]
        public int loopCount = -1;

        [Tooltip("Looping style (Restart, Yoyo, Incremental).")]
        public DGTweenLoopType loopType = DGTweenLoopType.Yoyo;

        /// <inheritdoc />
        public override void Play(GameObject target)
        {
            Play(target, 1f);
        }

        /// <summary>
        /// Play with optional speed multiplier (1 = normal speed).
        /// Provided for future use if manager/trigger wants to pass a multiplier.
        /// </summary>
        public void Play(GameObject target, float speedMultiplier = 1f)
        {
            if (target == null) return;

            // safety
            KillTweens(target);

            if (setActiveOnPlay)
                target.SetActive(true);

            Tween tween = null;

            // decide effective duration (guard against zero)
            float effectiveDuration = Mathf.Max(0.0001f, Duration) / Mathf.Max(0.0001f, speedMultiplier);
            float usedDuration = blink ? Mathf.Max(0.0001f, blinkInterval) : effectiveDuration;

            // Case 1: CanvasGroup (fade entire UI group)
            if (target.TryGetComponent<CanvasGroup>(out var cg))
            {
                cg.alpha = from;
                tween = DOTween.To(() => cg.alpha, x => cg.alpha = x, to, usedDuration).SetEase(ease);
            }
            // Case 2: UI Graphic (Image, Text, RawImage, etc.)
            else if (target.TryGetComponent<Graphic>(out var graphic))
            {
                var c = graphic.color;
                graphic.color = new Color(c.r, c.g, c.b, from);
                tween = DOTween.ToAlpha(() => graphic.color, x => graphic.color = x, to, usedDuration).SetEase(ease);
            }

            if (tween == null)
            {
                Debug.LogWarning($"[FadeAnimationSO] No CanvasGroup or UI Graphic found on {target.name}.");
                return;
            }

            // Loop / blink handling:
            if (blink)
            {
                // Blink should ping-pong between from <-> to. Use Yoyo loop type explicitly.
                tween.SetLoops(-1, DGTweenLoopType.Yoyo);
            }
            else if (loop)
            {
                tween.SetLoops(loopCount, loopType);
            }
            else
            {
                tween.OnComplete(() =>
                {
                    if (deactivateOnEnd)
                        target.SetActive(false);

                    RaiseOnComplete();
                });
            }
        }
    }
}
