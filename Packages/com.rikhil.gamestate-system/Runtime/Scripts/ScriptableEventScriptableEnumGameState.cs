using UnityEngine;
using Obvious.Soap;
namespace Rikhil.GameStateSystem
{
    [CreateAssetMenu(fileName = "ScriptableEvent" + nameof(ScriptableEnumGameState), menuName = "Soap/ScriptableEvents/" + nameof(ScriptableEnumGameState))]
    public class ScriptableEventScriptableEnumGameState : ScriptableEvent<ScriptableEnumGameState>
    {

    }
}

