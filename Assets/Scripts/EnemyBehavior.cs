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

            if (_hp <= 0) {
                _hp = 0;
                EnemyDeath();
            }
        }

        private void FixedUpdate() {
            _rb.linearVelocityX = _velocity * _direction;
        }


        // Draw a line to see the path of each enemy
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

        private void EnemyDeath() {
            Destroy(gameObject);
        }
    }
}