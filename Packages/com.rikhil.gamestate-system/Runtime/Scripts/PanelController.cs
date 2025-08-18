using UnityEngine;
using Obvious.Soap;

namespace Rikhil.GameStateSystem
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelController : MonoBehaviour
    {
        [SerializeField] private ScriptableEnumGameState thisPanelState;               // Which state this panel belongs to
        [SerializeField] private ScriptableEventScriptableEnumGameState onGameStateChanged; // Event raised by InternetChecker or others

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            // Subscribe to state change events
            onGameStateChanged.OnRaised += HandleStateChanged;

            // Start hidden but still subscribed
            Hide();
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

            if (newState == thisPanelState)
                Show();
            else
                Hide();
        }

        /// <summary>
        /// Makes the panel visible and interactive without disabling the GameObject.
        /// </summary>
        private void Show()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// Hides the panel but keeps the GameObject active so it still listens for events.
        /// </summary>
        private void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
