using UnityEngine;
using Obvious.Soap;
using Rikhil.SoundSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using Rikhil.AnimationSystem;
/// <summary>
/// Tracks placed letters and animates them sequentially when the level is complete.
/// Optimized: No runtime FindObjectsOfType! Alphabets register themselves on Start.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Level Variables (SOAP)")]
    [SerializeField] private ScriptableListAlphabetLetterList correctLettersList;
    [SerializeField] private IntVariable desiredSize;

    [Header("UI")]
    [SerializeField] private Button nextButton;

    [Header("Events (SOAP)")]
    [SerializeField] private ScriptableEventAlphabetLetter onCorrectLetterPlaced;
    [SerializeField] private ScriptableEventNoParam onAllCorrectPlaced;

    [Header("Level Completion Sound")]
    [SerializeField] private ScriptableEventSoundType onPlaySound;
    [SerializeField] private ScriptableEnumSoundTypeRegistry levelSoundType;

    [SerializeField]
    private ScriptableEnumSoundTypeRegistry spellingSoundType;

    [Header("Animation")]
    [SerializeField] private ScaleAnimationSO letterScaleAnimation;

    private int currentLetterIndex = 0;

    // ✅ All active alphabets for this level
    private readonly List<Alphabet> allAlphabets = new();
    private readonly List<Alphabet> placedAlphabets = new();

    private void Start()
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        onCorrectLetterPlaced.OnRaised += OnLetterPlaced;
        onAllCorrectPlaced.OnRaised += ShowButton;
    }

    private void OnDisable()
    {
        onCorrectLetterPlaced.OnRaised -= OnLetterPlaced;
        onAllCorrectPlaced.OnRaised -= ShowButton;
    }

    /// <summary>
    /// Called by each Alphabet on Start to register itself.
    /// </summary>
    public void RegisterAlphabet(Alphabet alphabet)
    {
        if (!allAlphabets.Contains(alphabet))
            allAlphabets.Add(alphabet);
    }

    private void OnLetterPlaced(ScriptableEnumAlphabet letter)
    {
        if (!correctLettersList.Contains(letter))
            correctLettersList.Add(letter);

        // Find alphabet instance that matches the placed letter
        foreach (var alpha in allAlphabets)
        {
            if (alpha.alpha == letter && !placedAlphabets.Contains(alpha))
            {
                placedAlphabets.Add(alpha);
                break;
            }
        }

        Debug.Log($"Letter placed: {letter.character} | Progress: {correctLettersList.Count}/{desiredSize.Value}");

        // When all slots are filled
        if (correctLettersList.Count == desiredSize.Value)
        {
            Debug.Log("🎉 All letters placed, starting animation sequence!");

            // Sort by X position for consistent animation order
            placedAlphabets.Sort((a, b) =>
                a.transform.position.x.CompareTo(b.transform.position.x));

            AnimateLettersSequentially();
        
        
        
        }
    }

    private void ShowButton()
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(true);
    }

    private void AnimateLettersSequentially()
    {
        if (placedAlphabets.Count == 0)
        {
            Debug.LogWarning("No letters to animate!");
            return;
        }

        currentLetterIndex = 0;
        AnimateNextLetter();
        
        
    }

    private void AnimateNextLetter()
    {
        if (currentLetterIndex >= placedAlphabets.Count)
        {
            Debug.Log("✅ All letter animations completed!");
            onPlaySound?.Raise(spellingSoundType);
            onPlaySound?.Raise(levelSoundType);
            onAllCorrectPlaced?.Raise();
            return;
        }

        Alphabet alphabet = placedAlphabets[currentLetterIndex];

        // Play the animation and chain the next one
        letterScaleAnimation.Play(alphabet.RuntimeInstance, () =>
        {
            onPlaySound?.Raise(alphabet.alpha.soundType);
            Debug.Log($"Finished animating {alphabet.alpha.character}");
            currentLetterIndex++;
            AnimateNextLetter();
            
        });
       
    }
}
