using UnityEngine;
using Obvious.Soap;
using Rikhil.SoundSystem;
using UnityEngine.UI;


/// <summary>
/// Manages a single spelling level:
/// - Subscribes to SOAP event when a letter is correctly placed
/// - Tracks progress in a ScriptableList
/// - Raises "all correct placed" event when level is complete
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Level Variables (SOAP)")]
    [SerializeField] private ScriptableListAlphabetLetterList correctLettersList; // holds letters placed
    [SerializeField] private IntVariable desiredSize;                             // total slots in level

    [Header("Events (SOAP)")]
    [SerializeField] private Button nextButton;               // assign in inspector

    [SerializeField] private ScriptableEventAlphabetLetter onCorrectLetterPlaced; // raised by slots
    [SerializeField] private ScriptableEventNoParam onAllCorrectPlaced;           // broadcast when full

    [Header("Level Completion Sound")]
    [SerializeField] private ScriptableEventSoundType onPlaySound;
    [SerializeField] private ScriptableEnumSoundTypeRegistry levelSoundType;

    private void Start()
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(false); // hide by default
        
    }

    private void OnEnable()
    {
        // ✅ subscribe to SOAP event
        if (onCorrectLetterPlaced != null)
        {
            onCorrectLetterPlaced.OnRaised += OnLetterPlaced;
            onAllCorrectPlaced.OnRaised += ShowButton;

        }

    }

    private void OnDisable()
    {
        // ❌ unsubscribe (avoid leaks)
        if (onCorrectLetterPlaced != null)
        {
            onCorrectLetterPlaced.OnRaised -= OnLetterPlaced;
            onAllCorrectPlaced.OnRaised -= ShowButton;
        }

    }

    /// <summary>
    /// Called whenever a slot reports a correct letter.
    /// Adds to the list and checks for completion.
    /// </summary>
    private void OnLetterPlaced(ScriptableEnumAlphabet letter)
    {
        if (!correctLettersList.Contains(letter))
            correctLettersList.Add(letter);

        Debug.Log($"Letter placed: {letter.character} | Progress: {correctLettersList.Count}/{desiredSize.Value}");

        if (correctLettersList.Count == desiredSize.Value)
        {
            Debug.Log("🎉 All letters placed, level complete!");

            onAllCorrectPlaced?.Raise();                // raise SOAP no-param event
            onPlaySound?.Raise(levelSoundType);         // play level success sound
        }
    }

    private void ShowButton()
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
            Debug.Log("Next Button is now visible!");
        }
    }
}
