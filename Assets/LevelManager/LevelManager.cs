using UnityEngine;
using Obvious.Soap;
using Rikhil.SoundSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using Rikhil.AnimationSystem;

/// <summary>
/// Handles level progression, tracks placed letters, 
/// triggers animations, sounds, and level completion.
/// Works with LevelLoader & Alphabet scripts.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Level Data (Runtime)")]
    private LevelDataSO currentLevelData;

    [Header("SOAP Variables")]
    [SerializeField] private ScriptableListAlphabetLetterList correctLettersList;
    [SerializeField] private IntVariable desiredSize;

    [Header("Events (SOAP)")]
    [SerializeField] private ScriptableEventAlphabetLetter onCorrectLetterPlaced;
    [SerializeField] private ScriptableEventNoParam onAllCorrectPlaced;
    [SerializeField] private ScriptableEventNoParam onNextLevel; // raised when Next button is clicked

    [Header("UI")]
    [SerializeField] private Button nextButton;

    [Header("Animation")]
    [SerializeField] private ScaleAnimationSO letterScaleAnimation;

    [Header("Sound System")]
    [SerializeField] private ScriptableEventSoundType onPlaySound;

    // Runtime
    private readonly List<Alphabet> allAlphabets = new();
    private readonly List<Alphabet> placedAlphabets = new();
    private int currentLetterIndex = 0;

    private void Awake()
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(HandleNextLevelButton);
        }
    }

    private void OnEnable()
    {
        if (onCorrectLetterPlaced != null)
            onCorrectLetterPlaced.OnRaised += OnLetterPlaced;

        if (onAllCorrectPlaced != null)
            onAllCorrectPlaced.OnRaised += ShowNextButton;
    }

    private void OnDisable()
    {
        if (onCorrectLetterPlaced != null)
            onCorrectLetterPlaced.OnRaised -= OnLetterPlaced;

        if (onAllCorrectPlaced != null)
            onAllCorrectPlaced.OnRaised -= ShowNextButton;
    }

    // ---------------------------
    // INITIALIZATION
    // ---------------------------

    public void InitializeLevel(LevelDataSO levelData)
    {
        currentLevelData = levelData;
        ResetProgress();

        Debug.Log($"🟢 LevelManager initialized with: {levelData.word}");

        // ✅ Clear old list just in case
        correctLettersList.Clear();

        // ✅ Auto-count prefilled letters (for FillInBlank mode)
        if (levelData.hasBlank)
        {
            for (int i = 0; i < levelData.slots.Length; i++)
            {
                if (i != levelData.blankIndex && levelData.slots[i].expectedLetter != null)
                {
                    var letter = levelData.slots[i].expectedLetter;
                    if (!correctLettersList.Contains(letter))
                    {
                        correctLettersList.Add(letter);

                        // ✅ Make sure event triggers for each prefilled letter (so animation order works)
                        onCorrectLetterPlaced?.Raise(letter);
                    }
                }
            }
        }
    }

    public void ResetProgress()
    {
        correctLettersList.Clear();
        placedAlphabets.Clear();
        currentLetterIndex = 0;

        // Safely reset only active, valid alphabets
        for (int i = allAlphabets.Count - 1; i >= 0; i--)
        {
            var alpha = allAlphabets[i];
            if (alpha == null)
            {
                allAlphabets.RemoveAt(i); // cleanup destroyed entries
                continue;
            }

            if (alpha.gameObject != null && alpha.gameObject.activeInHierarchy)
            {
                try
                {
                    alpha.ResetToOriginal();
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"⚠️ Could not reset alphabet {alpha.name}: {ex.Message}");
                }
            }
        }

        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
    }

    // ---------------------------
    // REGISTRATION
    // ---------------------------

    public void RegisterAlphabet(Alphabet alphabet)
    {
        if (!allAlphabets.Contains(alphabet))
            allAlphabets.Add(alphabet);
    }

    // ---------------------------
    // GAMEPLAY PROGRESSION
    // ---------------------------

    private void OnLetterPlaced(ScriptableEnumAlphabet letter)
    {
        if (!correctLettersList.Contains(letter))
            correctLettersList.Add(letter);

        // Find alphabet instance matching this letter
        foreach (var alpha in allAlphabets)
        {
            if (alpha.alpha == letter && !placedAlphabets.Contains(alpha))
            {
                placedAlphabets.Add(alpha);
                break;
            }
        }

        Debug.Log($"✅ Letter placed: {letter.character} | {correctLettersList.Count}/{desiredSize.Value}");

        // Check for completion
        if (correctLettersList.Count == desiredSize.Value)
        {
            Debug.Log("🎉 All letters placed! Starting spelling animation.");
            placedAlphabets.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
            AnimateLettersSequentially();
        }
    }

    // ---------------------------
    // ANIMATION SEQUENCE
    // ---------------------------

    private void AnimateLettersSequentially()
    {
        if (placedAlphabets.Count == 0)
        {
            Debug.LogWarning("⚠️ No letters to animate!");
            return;
        }

        currentLetterIndex = 0;
        AnimateNextLetter();
    }

    private void AnimateNextLetter()
    {
        if (currentLetterIndex >= placedAlphabets.Count)
        {
            // ✅ All letters animated → play word sound
            Debug.Log("🔊 Playing full word sound!");
            if (onPlaySound != null && currentLevelData != null && currentLevelData.spellingSound != null)
                onPlaySound.Raise(currentLevelData.spellingSound);

            // Level complete sound
            if (onPlaySound != null && currentLevelData.completeSound != null)
                onPlaySound.Raise(currentLevelData.completeSound);

            // Raise level completion event
            onAllCorrectPlaced?.Raise();
            return;
        }

        Alphabet current = placedAlphabets[currentLetterIndex];

        // Play animation and sound sequentially
        letterScaleAnimation.Play(current.RuntimeInstance, () =>
        {
            // Play letter-specific sound
            if (onPlaySound != null && current.alpha.soundType != null)
                onPlaySound.Raise(current.alpha.soundType);

            Debug.Log($"Finished animating: {current.alpha.character}");
            currentLetterIndex++;
            AnimateNextLetter();
        });
    }

    // ---------------------------
    // NEXT LEVEL HANDLING
    // ---------------------------

    private void ShowNextButton()
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(true);
    }

    private void HandleNextLevelButton()
    {
        Debug.Log("➡️ Next button clicked!");
        onNextLevel?.Raise(); // 🔥 Notify LevelLoader to load next level
        ResetProgress();
    }
}
