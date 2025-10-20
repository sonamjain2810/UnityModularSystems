using UnityEngine;
using Obvious.Soap;

/// <summary>
/// SOAP event that broadcasts the selected LevelType (Spelling, FillInBlank, BlankSpelling).
/// </summary>
[CreateAssetMenu(fileName = "Event_LevelType", menuName = "Soap/Events/LevelType Event")]
public class ScriptableEventLevelType : ScriptableEvent<ScriptableEnumLevelType> { }
