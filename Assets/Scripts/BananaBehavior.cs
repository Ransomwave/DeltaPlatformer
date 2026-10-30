using UnityEngine;

namespace deltaformer {
    public class BananaBehavior : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.tag.Contains("Player")) Destroy(gameObject);
        }
    }
}