using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Obvious.Soap;
using Rikhil.SoundSystem;
using Rikhil.AnimationSystem;

/// <summary>
/// Represents a draggable alphabet tile in the game.
/// Handles drag/drop, visuals, sounds, and resets properly.
/// Locks itself once correctly placed in a slot.
/// </summary>
public class Alphabet : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Alphabet Data (SOAP)")]
    public ScriptableEnumAlphabet alpha; // Letter data (character, color, sound)
    [SerializeField] private ScriptableEventSoundType onPlaySound;

    [Header("UI References")]
    [SerializeField] private Image targetImage;
    [SerializeField] private TextMeshProUGUI characterText;

    [Header("State Tracking")]
    [SerializeField] private BoolVariable placedInSlot; // shared state var (SOAP)

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 dragOffset;

    // For resetting to initial state
    private Transform originalParent;
    private Vector2 originalPosition;

    // To prevent dragging once placed
    private bool isLocked = false;

    // For LevelManager animation
    public GameObject RuntimeInstance => gameObject;
    public Transform OriginalParent => originalParent;
    public Vector2 OriginalPosition => originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
    }

    private void Start()
    {
        // Register with LevelManager
        LevelManager manager = FindObjectOfType<LevelManager>();
        if (manager != null)
            manager.RegisterAlphabet(this);

        // Initialize visuals
        if (alpha != null)
        {
            if (characterText != null)
                characterText.text = alpha.character;

            if (targetImage != null)
                targetImage.color = alpha.color;
        }
    }

    // --- Drag Handling ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return; // ❌ Don’t allow drag after correct placement
        if (alpha == null) return;

        // Play sound when dragging starts
        if (onPlaySound != null && alpha.soundType != null)
            onPlaySound.Raise(alpha.soundType);

        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out dragOffset
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        Vector2 localPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPos))
        {
            rectTransform.localPosition = localPos - dragOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        if (!placedInSlot.Value)
        {
            // Return to original position if not correctly placed
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }

        canvasGroup.blocksRaycasts = true;
        placedInSlot.Value = false;
    }

    // --- State Helpers ---

    /// <summary>
    /// Called by AlphabetSlot when this alphabet is dropped correctly.
    /// Locks movement.
    /// </summary>
    public void LockInPlace()
    {
        isLocked = true;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Resets this alphabet to its initial state.
    /// </summary>
    public void ResetToOriginal()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;

        isLocked = false;
        canvasGroup.blocksRaycasts = true;
        placedInSlot.Value = false;

        if (alpha != null)
        {
            if (characterText != null)
                characterText.text = alpha.character;

            if (targetImage != null)
                targetImage.color = alpha.color;
        }
    }

    public void SetColor(Color newColor)
    {
        if (targetImage != null)
            targetImage.color = newColor;
    }
}
