using UnityEngine;

namespace deltaformer {
    public class SpinningLeaf : MonoBehaviour, ILivingEntity {
        private const int DAMAGE = 1;

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player") && other.TryGetComponent<ILivingEntity>(out var entity)) {
                entity.TakeDamage(DAMAGE);
            }
        }

        public void TakeDamage(int damage) {
            Destroy(gameObject);
        }
    }
}
