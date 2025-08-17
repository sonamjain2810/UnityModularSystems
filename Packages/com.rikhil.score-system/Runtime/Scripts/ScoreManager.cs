using UnityEngine;
using Obvious.Soap;

namespace Rikhil.ScoreSystem
{
    /// <summary>
    /// Score Manager jo SOAP ke ScriptableEventInt ke sath kaam karta hai.
    /// Event ko OnRaised se subscribe/unsubscribe karta hai.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [Header("Score Data")]
        [SerializeField] private IntVariable scoreVariable;

        [SerializeField] private ScriptableEventInt scoreChangedEvent;

        private void OnEnable()
        {
            if (scoreChangedEvent != null)
                scoreChangedEvent.OnRaised += OnScoreChanged; // subscribe
        }

        private void OnDisable()
        {
            if (scoreChangedEvent != null)
                scoreChangedEvent.OnRaised -= OnScoreChanged; // unsubscribe
        }

        private void OnScoreChanged(int amount)
        {
            if (scoreVariable != null)
                scoreVariable.Value += amount;
        }

        [ContextMenu("Reset Score")]
        public void ResetScore()
        {
            if (scoreVariable != null)
                scoreVariable.Value = 0;
        }
    }
}
