using UnityEngine;

namespace deltaformer {
    public class EnemyBehavior : MonoBehaviour {
        [Header("Enemy settings")] [SerializeField]
        private int _hp = 2;

        [SerializeField] private float _velocity = 2f;

        [SerializeField] private float _distance = 5f;
        private int _direction = 1;
        private bool _isFacingRight = true;

        private Rigidbody2D _rb;
        private Vector2 _startPos;

        private void Start() {
            _rb = GetComponent<Rigidbody2D>();
            _startPos = transform.position;
        }

        private void Update() {
            var distanceFromStart = transform.position.x - _startPos.x;

            if (_direction == 1 && distanceFromStart >= _distance) {
                _direction = -1;
                Flip();
            } else if (_direction == -1 && distanceFromStart <= 0) {
                _direction = 1;
                Flip();
            }
        }

        private void FixedUpdate() {
            _rb.linearVelocityX = _velocity * _direction;
        }


        // Dibuja una línea roja en la pestaña 'Scene' para ver la ruta horizontal en el editor
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            var start = Application.isPlaying ? _startPos : (Vector2)transform.position;
            var end = start + Vector2.right * _distance;
            Gizmos.DrawLine(start, end);
        }

        private void Flip() {
            _isFacingRight = !_isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
}