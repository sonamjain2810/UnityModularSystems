using UnityEngine;
using Obvious.Soap;

namespace Rikhil.VibrationSystem
{
    // Custom wrapper to create a dedicated "Vibration Event" asset in the Create menu.
    [CreateAssetMenu(
        fileName = "ScriptableEventVibration",
        menuName = "Soap/ScriptableEvents/Vibration")]
    public class ScriptableEventVibration : ScriptableEventNoParam
    {
    }
}
