using UnityEngine;
using Obvious.Soap;
using Rikhil.SoundSystem;


[CreateAssetMenu(fileName = "ScriptableEnumAlphabetSlot", menuName = "Soap/ScriptableEnums/AlphabetSlot")]
public class ScriptableEnumAlphabetSlot : ScriptableEnumBase
{
    [Header("Slot Data")]
    public Color slotColor = Color.grey;

    // Which letter is expected in this slot
    public ScriptableEnumAlphabet expectedLetter;
    [Header("Correct Sound")]
    public ScriptableEnumSoundTypeRegistry soundType;
}
