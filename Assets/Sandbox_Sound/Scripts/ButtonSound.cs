using UnityEngine;
using UnityEngine.UI;
using Obvious.Soap;
using Rikhil.SoundSystem;

public class ButtonSound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScriptableEventSoundType onSoundEvent;
    [SerializeField] private ScriptableEnumSoundTypeRegistry clickSoundType;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(PlayClickSound);
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        onSoundEvent.Raise(clickSoundType);
    }
}

