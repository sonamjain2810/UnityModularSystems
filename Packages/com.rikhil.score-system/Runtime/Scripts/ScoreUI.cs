using UnityEngine;
using UnityEngine.UI; // Fallback if TMP not installed
using Obvious.Soap;   // For IntVariable

#if TMP_PRESENT
using TMPro;
#endif

namespace Rikhil.ScoreSystem
{
    /// <summary>
    /// Score UI jo IntVariable ke OnValueChanged event ko listen karta hai.
    /// TMP installed ho to TMP_Text use karega, warna UnityEngine.UI.Text.
    /// </summary>
    public class ScoreUI : MonoBehaviour
    {
        [Header("Score Data (Scriptable Object)")]
        [SerializeField] private IntVariable scoreVariable;

#if TMP_PRESENT
        [Header("UI (TMP)")]
        [SerializeField] private TMP_Text scoreText;
#else
        [Header("UI (Unity Text)")]
        [SerializeField] private Text scoreText;
#endif

        private void OnEnable()
        {
            if (scoreVariable != null)
            {
                // Event subscribe
                scoreVariable.OnValueChanged += UpdateUI;
                // Initial UI update
                UpdateUI(scoreVariable.Value);
            }
        }

        private void OnDisable()
        {
            if (scoreVariable != null)
            {
                // Event unsubscribe
                scoreVariable.OnValueChanged -= UpdateUI;
            }
        }

        /// <summary>
        /// UI text ko new score ke saath update karta hai.
        /// </summary>
        private void UpdateUI(int newScore)
        {
            if (scoreText != null)
                scoreText.text = $"Score: {newScore}";
        }
    }
}
