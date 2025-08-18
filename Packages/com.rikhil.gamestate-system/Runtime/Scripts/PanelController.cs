using UnityEngine;
using Obvious.Soap; // SOAP events

public class PanelController : MonoBehaviour
{
    [SerializeField] private GameState thisPanelState; 
    [SerializeField] private ScriptableEvent<GameState> onGameStateChanged;

    private void OnEnable()
    {
        onGameStateChanged.AddListener(HandleStateChanged);
    }

    private void OnDisable()
    {
        onGameStateChanged.RemoveListener(HandleStateChanged);
    }

    private void HandleStateChanged(GameState newState)
    {
        gameObject.SetActive(newState == thisPanelState);
    }
}
