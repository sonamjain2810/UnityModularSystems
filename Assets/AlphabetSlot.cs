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

    private void Start()
    {
        var tmp = GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null && expectedSlot.expectedLetter != null)
            tmp.text = expectedSlot.expectedLetter.character;

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
                dropped.transform.SetParent(transform);
                dropped.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                placedInSlot.Value = true;

                Debug.Log($"✅ Correct Answer: {dropped.alpha.character}");
                onCorrectLetterPlaced.Raise(dropped.alpha);

                if (expectedSlot.soundType != null && onPlaySound != null)
                    onPlaySound.Raise(expectedSlot.soundType);
            }
            else
            {
                dropped.transform.SetParent(dropped.OriginalParent);
                dropped.GetComponent<RectTransform>().anchoredPosition = dropped.OriginalPosition;
                if (onVibrateEvent != null)
                    onVibrateEvent.Raise();
            }
        }
    }
}
