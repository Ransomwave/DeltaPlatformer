using UnityEngine;

namespace deltaformer {
    public class BananaBehavior : MonoBehaviour {

        private GameManager _gameManager;

        private void Awake() {
            _gameManager = FindAnyObjectByType<GameManager>();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.tag.Contains("Player")) {
                _gameManager.IncrementBananaCount();
                Destroy(gameObject);
            }
        }
    }
}