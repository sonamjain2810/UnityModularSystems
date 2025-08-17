
using UnityEngine;
using UnityEngine.SceneManagement;
// using Obvious.Soap; // Uncomment when LevelLoad/Complete events are defined

namespace Rikhil.LevelSystem
{
    public class LevelManager : MonoBehaviour
    {
        public void LoadSceneByName(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            SceneManager.LoadScene(sceneName);
        }

        public void ReloadActiveScene()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}
