using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace deltaformer {
    public class PauseMenu : MonoBehaviour {
        void Update() {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
                SceneNavigator.previousScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene("UIOptions");
            }
        }
    }
}