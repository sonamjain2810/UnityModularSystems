using UnityEngine;
using Obvious.Soap;
namespace Rikhil.GameStateSystem {
[CreateAssetMenu(fileName = "ScriptableEnumGameState", menuName = "Soap/ScriptableEnums/GameState")]
public class ScriptableEnumGameState : ScriptableEnumBase
{
    public string Name;
    public Sprite Icon;
    public Color color;
}
}
