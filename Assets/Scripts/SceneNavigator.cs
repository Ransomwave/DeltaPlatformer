using UnityEngine;
using UnityEngine.SceneManagement;

namespace deltaformer {
    public class SceneNavigator : MonoBehaviour {
        public static string previousScene;

        public void OpenOptions(string optionsSceneName) {
            previousScene = SceneManager.GetActiveScene().name;

            
            SceneManager.LoadScene(optionsSceneName);
        }

        public void CloseOptions() {
            if (!string.IsNullOrEmpty(previousScene)) {
                SceneManager.LoadScene(previousScene);
            } else {
                SceneManager.LoadScene("UIStart");
            }
        }
    }
}