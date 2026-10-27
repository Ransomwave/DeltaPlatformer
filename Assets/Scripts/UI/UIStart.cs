using UnityEngine;
using UnityEngine.SceneManagement;

namespace deltaformer {
    public class UIStart : MonoBehaviour {
        public void PlayGame() {
            SceneManager.LoadScene("Level1");
        }

        public void QuitGame() {
            Debug.Log("Saliendo del juego...");
            Application.Quit();
        }
    }

}

