using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Obvious.Soap;
using Rikhil.SoundSystem;    // for sound events
using Rikhil.AnimationSystem;

/// <summary>
/// Represents a draggable alphabet letter.
/// Handles UI visuals, dragging, and playing sound when picked.
/// </summary>
public class Alphabet : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Alphabet Data (SOAP)")]
    public ScriptableEnumAlphabet alpha;                 // Letter ScriptableEnum (character, color, sound)
    [SerializeField] private ScriptableEventSoundType onPlaySound; // SOAP event to play sound

    [Header("UI References")]
    [SerializeField] private Image targetImage;          // UI background image
    [SerializeField] private TextMeshProUGUI characterText; // UI text for the letter

    [Header("State Tracking")]
    [SerializeField] private BoolVariable placedInSlot;  // Did we place in a slot?

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 dragOffset;

    private Transform originalParent;
    public Transform OriginalParent => originalParent;

    private Vector2 originalPosition;
    public Vector2 OriginalPosition => originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        // Display character and initial color
        if (characterText != null)
            characterText.text = alpha.character;

        if (targetImage != null)
            targetImage.color = alpha.color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Play letter sound on drag start
        if (onPlaySound != null && alpha.soundType != null)
            onPlaySound.Raise(alpha.soundType);

        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        transform.SetParent(transform.root);   // move to top layer
        canvasGroup.blocksRaycasts = false;

        // track drag offset
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out dragOffset
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        // follow mouse / touch
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPos
        );
        rectTransform.localPosition = localPos - dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // if not placed correctly → reset position
        if (!placedInSlot.Value)
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }

        canvasGroup.blocksRaycasts = true;
        placedInSlot.Value = false; // reset for next drag
    }

    /// <summary>
    /// Change letter color dynamically (e.g. feedback).
    /// </summary>
    public void SetColor(Color newColor)
    {
        if (targetImage != null)
            targetImage.color = newColor;
    }
}
