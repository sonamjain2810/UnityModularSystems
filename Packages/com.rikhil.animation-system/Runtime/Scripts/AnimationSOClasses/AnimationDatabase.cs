using UnityEngine;

namespace Rikhil.AnimationSystem
{
    [CreateAssetMenu(fileName = "AnimationDatabase", menuName = "AnimationSystem/Animation Database")]
    public class AnimationDatabase : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public ScriptableEnumAnimationTypeRegistry animationType; // SOAP registry value
            public AnimationActionSO action; // which action SO to run
        }

        public Entry[] entries;

        public AnimationActionSO GetAction(ScriptableEnumAnimationTypeRegistry type)
        {
            if (type == null || entries == null) return null;
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].animationType == type) return entries[i].action;
            }
            return null;
        }
    }
}
