using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Database", fileName = "LevelDatabase")]
public class LevelDatabaseSO : ScriptableObject
{
    public LevelDataSO[] spellingLevels;
    public LevelDataSO[] fillInBlankLevels;
    public LevelDataSO[] blankSpellingLevels;
}
