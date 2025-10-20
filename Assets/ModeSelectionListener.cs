using UnityEngine;
using Obvious.Soap;

/// <summary>
/// Listens to mode selection events (Spelling / FillInBlank / BlankSpelling)
/// and stores the selected mode into a shared ScriptableVariable.
/// </summary>
public class ModeSelectionListener : MonoBehaviour
{
    [Header("SOAP Events")]
    [Tooltip("Event raised when a mode is selected from the UI.")]
    [SerializeField] private ScriptableEventScriptableEnumLevelType onModeSelected;

    [Header("Runtime Variable")]
    [Tooltip("Stores the currently selected mode for the rest of the game.")]
    [SerializeField] private ScriptableEnumLevelTypeVariable currentMode;

    private void OnEnable()
    {
        if (onModeSelected != null)
            onModeSelected.OnRaised += HandleModeSelected;
    }

    private void OnDisable()
    {
        if (onModeSelected != null)
            onModeSelected.OnRaised -= HandleModeSelected;
    }

    private void HandleModeSelected(ScriptableEnumLevelType mode)
    {
        currentMode.Value = mode;
        Debug.Log($"[ModeSelectionListener] Mode selected → {mode.name}");
    }
}
