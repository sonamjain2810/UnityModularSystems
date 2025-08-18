using UnityEngine;
using Obvious.Soap;
using System.Collections;

namespace Rikhil.GameStateSystem
{
    public class InternetChecker : MonoBehaviour
    {
        [Header("Event Reference")]
        [SerializeField] private ScriptableEventScriptableEnumGameState onGameStateChanged;

        [Header("States")]
        [SerializeField] private ScriptableEnumGameState internetState;
        [SerializeField] private ScriptableEnumGameState mainMenuState;

        private void Awake()
        {
            // 🔹 Immediately raise a "default state" so some panel is always visible
            // Here we assume: if no internet → show Internet panel, else show MainMenu.
            bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;
            var initialState = hasInternet ? mainMenuState : internetState;

            Debug.Log($"[InternetChecker] Awake -> Raising initial state: {initialState.name}");
            onGameStateChanged.Raise(initialState);
        }

        private IEnumerator Start()
        {
            Debug.Log("In InternetChecker Start -> checking internet again after small delay");

            // Optional: wait before re-checking (useful if first check happens too early)
            yield return new WaitForSeconds(1f);

            bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;
            var targetState = hasInternet ? mainMenuState : internetState;

            Debug.Log($"[InternetChecker] Start -> Raising state: {targetState.name}");
            onGameStateChanged.Raise(targetState);
        }
    }
}
