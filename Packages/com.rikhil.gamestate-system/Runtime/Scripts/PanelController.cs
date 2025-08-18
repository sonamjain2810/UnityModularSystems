using UnityEngine;
using Obvious.Soap;

namespace Rikhil.GameStateSystem
{
    public class PanelController : MonoBehaviour
    {
        [SerializeField] private ScriptableEnumGameState thisPanelState;                // Which state this panel belongs to
        [SerializeField] private ScriptableEventScriptableEnumGameState onGameStateChanged; // Event raised by InternetChecker or others

        private void Awake()
        {
            // Subscribe to state change events
            onGameStateChanged.OnRaised += HandleStateChanged;
        }

        private void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks
            onGameStateChanged.OnRaised -= HandleStateChanged;
        }

        /// <summary>
        /// Called whenever the GameState event is raised.
        /// If the new state matches this panel's state, show the panel.
        /// Otherwise, hide it.
        /// </summary>
        private void HandleStateChanged(ScriptableEnumGameState newState)
        {
            Debug.Log($"[PanelController] Panel {thisPanelState.name} received state {newState.name}");
            bool shouldShow = (newState == thisPanelState);
            Debug.Log($"[PanelController] -> {(shouldShow ? "SHOW" : "HIDE")} {thisPanelState.name}");
            gameObject.SetActive(shouldShow);
        }
    }
}
