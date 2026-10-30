using UnityEngine;

namespace deltaformer {
    public class SpinningLeaf : MonoBehaviour {
        private const int DAMAGE = 1;

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.name == "Hitbox" && other.transform.parent.CompareTag("Player")) {
                Destroy(gameObject);
            } else if (other.CompareTag("Player") && other.TryGetComponent<ILivingEntity>(out var entity)) {
                entity.TakeDamage(DAMAGE);
            }
        }
    }
}
