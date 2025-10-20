using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Rikhil.SoundSystem;
using Obvious.Soap;

public class AlphabetSlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Data (SOAP)")]
    [SerializeField] private ScriptableEnumAlphabetSlot expectedSlot;

    [Header("State Vars")]
    [SerializeField] private BoolVariable placedInSlot;
    [SerializeField] private ScriptableEventNoParam onVibrateEvent;

    [Header("Sound")]
    [SerializeField] private ScriptableEventSoundType onPlaySound;

    [Header("Notify Level Manager")]
    [SerializeField] private ScriptableEventAlphabetLetter onCorrectLetterPlaced;

    [Header("Mode System (SOAP)")]
    [Tooltip("Current level type (Spelling / FillInBlank / BlankSpelling)")]
    [SerializeField] private ScriptableEnumLevelTypeVariable currentLevelMode;

    private TextMeshProUGUI tmp;
    private Image img;

    // <<< ADDED: expose the expected letter so LevelLoader and other systems can query it safely
    public ScriptableEnumAlphabet ExpectedLetter => expectedSlot != null ? expectedSlot.expectedLetter : null;

    private void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        img = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        UpdateSlot(expectedSlot);
    }

    // Called by LevelLoader to refresh data when level changes
    public void UpdateSlot(ScriptableEnumAlphabetSlot newSlotData)
    {
        expectedSlot = newSlotData;

        if (tmp != null && expectedSlot != null && expectedSlot.expectedLetter != null)
            tmp.text = expectedSlot.expectedLetter.character;
        else if (tmp != null)
            tmp.text = "";

        if (img != null)
            img.color = expectedSlot != null ? expectedSlot.slotColor : Color.clear;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Allow drops only during gameplay
        if (currentLevelMode == null)
        {
            Debug.LogWarning("❌ No Level Mode assigned, ignoring drop.");
            return;
        }

        // Optional: behavior can vary by mode if you like
        if (currentLevelMode.Value != null)
            Debug.Log($"🎮 Mode: {currentLevelMode.Value.levelName}");

        Alphabet dropped = eventData.pointerDrag.GetComponent<Alphabet>();
        if (dropped == null)
            return;

        if (expectedSlot != null && dropped.alpha == expectedSlot.expectedLetter)
        {
            // ✅ Correct drop
            dropped.transform.SetParent(transform);
            dropped.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            placedInSlot.Value = true;

            dropped.LockInPlace(); // 🔒 Prevent future dragging

            Debug.Log($"✅ Correct Answer: {dropped.alpha.character}");
            onCorrectLetterPlaced?.Raise(dropped.alpha);

            if (expectedSlot.soundType != null && onPlaySound != null)
                onPlaySound.Raise(expectedSlot.soundType);
        }
        else
        {
            // ❌ Wrong drop
            dropped.transform.SetParent(dropped.OriginalParent);
            dropped.GetComponent<RectTransform>().anchoredPosition = dropped.OriginalPosition;
            onVibrateEvent?.Raise();
        }
    }
}
