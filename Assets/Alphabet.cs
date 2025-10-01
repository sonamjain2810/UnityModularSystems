using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Obvious.Soap;
using Rikhil.SoundSystem;
using Rikhil.AnimationSystem;

public class Alphabet : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Alphabet Data (SOAP)")]
    public ScriptableEnumAlphabet alpha;
    [SerializeField] private ScriptableEventSoundType onPlaySound;

    [Header("UI References")]
    [SerializeField] private Image targetImage;
    [SerializeField] private TextMeshProUGUI characterText;

    [Header("State Tracking")]
    [SerializeField] private BoolVariable placedInSlot;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 dragOffset;

    private Transform originalParent;
    public Transform OriginalParent => originalParent;

    private Vector2 originalPosition;
    public Vector2 OriginalPosition => originalPosition;

    // For LevelManager to animate
    public GameObject RuntimeInstance => gameObject;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        // Register with LevelManager
        LevelManager manager = FindObjectOfType<LevelManager>();
        if (manager != null)
            manager.RegisterAlphabet(this);

        // Display initial UI
        if (characterText != null)
            characterText.text = alpha.character;

        if (targetImage != null)
            targetImage.color = alpha.color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
        if (!placedInSlot.Value)
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }

        canvasGroup.blocksRaycasts = true;
        placedInSlot.Value = false;
    }

    public void SetColor(Color newColor)
    {
        if (targetImage != null)
            targetImage.color = newColor;
    }
}
