using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Rikhil.SoundSystem;
using Obvious.Soap;

/// <summary>
/// Represents a slot where an alphabet can be dropped.
/// Validates correct letter, snaps it, plays feedback.
/// </summary>
public class AlphabetSlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Data (SOAP)")]
    [SerializeField] private ScriptableEnumAlphabetSlot expectedSlot; // Slot data (color + expected letter)

    [Header("State Vars")]
    [SerializeField] private BoolVariable placedInSlot;       // Did something get placed here?
        [SerializeField] private ScriptableEventNoParam onVibrateEvent; // vibrate feedback on wrong drop

    [Header("Sound")]
    [SerializeField] private ScriptableEventSoundType onPlaySound; // raises sounds

    [Header("Notify Level Manager")]
    [SerializeField] private ScriptableEventAlphabetLetter onCorrectLetterPlaced;

    private void Start()
    {
        // UI label = expected letter
        var tmp = GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null && expectedSlot.expectedLetter != null)
            tmp.text = expectedSlot.expectedLetter.character;

        // Slot background color
        var img = GetComponentInChildren<Image>();
        if (img != null)
            img.color = expectedSlot.slotColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Alphabet dropped = eventData.pointerDrag.GetComponent<Alphabet>();
        if (dropped != null)
        {
            if (dropped.alpha == expectedSlot.expectedLetter)
            {
                // ✅ Correct drop
                dropped.transform.SetParent(transform);
                dropped.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                placedInSlot.Value = true;

                Debug.Log($"✅ Correct Answer: {dropped.alpha.character}");

                // Notify LevelManager
                onCorrectLetterPlaced.Raise(dropped.alpha);

                // Play slot feedback sound
                if (expectedSlot.soundType != null && onPlaySound != null)
                    onPlaySound.Raise(expectedSlot.soundType);
            }
            else
            {
                // ❌ Wrong drop → reset
                dropped.transform.SetParent(dropped.OriginalParent);
                dropped.GetComponent<RectTransform>().anchoredPosition = dropped.OriginalPosition;

                Debug.Log("❌ Wrong Answer");

                if (onVibrateEvent != null)
                    onVibrateEvent.Raise();
            }
        }
    }
}
