using UnityEngine;

namespace deltaformer {
    public class SpinningLeavesHandler : MonoBehaviour {
        [SerializeField] private GameObject _leafPrefab;
        [SerializeField] private int _amount = 4;
        [SerializeField] private float _radius = 2f;
        [SerializeField] private float _rotationSpeed = 90f; // Degrees per second, negative spins clockwise

        private void Start() {
            for (var i = 0; i < _amount; i++) {
                var angle = i * Mathf.PI * 2f / _amount;
                var offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * _radius;

                Instantiate(_leafPrefab, transform.position + offset, Quaternion.identity, transform);
            }
        }

        private void Update() {
            transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
