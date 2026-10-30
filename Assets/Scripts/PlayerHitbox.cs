using UnityEngine;

namespace deltaformer {
    public class PlayerHitbox : MonoBehaviour {
        [SerializeField] private int _damage = 1;

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.TryGetComponent<ILivingEntity>(out var entity)) {
                entity.TakeDamage(_damage);
            }
        }
    }
}
