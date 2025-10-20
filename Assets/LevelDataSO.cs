using UnityEngine;
using Obvious.Soap;
using Rikhil.SoundSystem;

[CreateAssetMenu(fileName = "LevelData_", menuName = "Game/Level Data")]
public class LevelDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string word;
    public Sprite wordImage;

    [Header("Letters & Slots")]
    public ScriptableEnumAlphabet[] letters;
    public ScriptableEnumAlphabetSlot[] slots;

    [Header("Fill-In-Blank Settings")]
    public bool hasBlank;
    [Range(0, 4)] public int blankIndex = -1; // optional blank slot index

    [Header("Mode Type")]
    public ScriptableEnumLevelType modeType; // tells which mode this level uses

    [Header("Sounds")]
    public ScriptableEnumSoundTypeRegistry spellingSound;
    public ScriptableEnumSoundTypeRegistry completeSound;
}
