using UnityEngine;
using Obvious.Soap;

namespace Rikhil.SoundSystem
{
    [CreateAssetMenu(
        fileName = "ScriptableEventSoundType", 
        menuName = "Soap/ScriptableEvents/SoundType")]
    public class ScriptableEventSoundType : ScriptableEvent<ScriptableEnumSoundTypeRegistry>
    {
    }
}
