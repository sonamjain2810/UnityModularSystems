using UnityEngine;
using Obvious.Soap;
using Rikhil.SoundSystem;

[CreateAssetMenu(fileName = "ScriptableEnumAlphabetLetter", menuName = "Soap/ScriptableEnums/AlphabetLetter")]
public class ScriptableEnumAlphabet : ScriptableEnumBase
{
    [Header("Letter Data")]
    public string character;
    public Color color = Color.white;
    public ScriptableEnumSoundTypeRegistry soundType;
}

