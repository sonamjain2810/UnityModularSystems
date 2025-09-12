using UnityEngine;
using Obvious.Soap;

namespace Rikhil.VibrationSystem
{
    public class VibrationManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoolVariable vibrationEnabled;
        [SerializeField] private ScriptableEventNoParam onVibrateEvent;

        private void OnEnable()
        {
            if (vibrationEnabled != null)
                vibrationEnabled.OnValueChanged += HandleVibrationChanged;

            if (onVibrateEvent != null)
                onVibrateEvent.OnRaised += HandleVibrateEvent;
        }

        private void OnDisable()
        {
            if (vibrationEnabled != null)
                vibrationEnabled.OnValueChanged -= HandleVibrationChanged;

            if (onVibrateEvent != null)
                onVibrateEvent.OnRaised -= HandleVibrateEvent;
        }

        private void HandleVibrationChanged(bool enabled)
        {
            if (enabled) Vibrate();
        }

        private void HandleVibrateEvent()
        {
            Vibrate();
        }

        public void Vibrate()
        {
            if (!vibrationEnabled || !vibrationEnabled.Value) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#elif UNITY_IOS && !UNITY_EDITOR
            Handheld.Vibrate();
#else
            Debug.Log("Vibration triggered (Editor)");
#endif
        }
    }
}
