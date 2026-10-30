using System.Collections;
using UnityEngine;

namespace deltaformer {
    public class EnemyBehavior : MonoBehaviour, ILivingEntity {
        [Header("Enemy settings")]
        [SerializeField] private int _hp = 2;
        [SerializeField] private int _damage = 1;

        [SerializeField] private float _velocity = 2f;

        [SerializeField] private float _distance = 5f;
        private int _direction = 1;
        [SerializeField] private GameObject _sprite;
        [SerializeField] private bool _canHit;

        private Rigidbody2D _rb;
        private Vector2 _startPos;
        private SpriteRenderer _spriteRenderer;

        private void Start() {
            _rb = GetComponent<Rigidbody2D>();
            _startPos = transform.position;

            if (_sprite != null) {
                _spriteRenderer = _sprite.GetComponent<SpriteRenderer>();
            }
        }

        private void Update() {
            var distanceFromStart = transform.position.x - _startPos.x;

            if (_direction == 1 && distanceFromStart >= _distance) {
                _direction = -1;
                Flip(false);
            } else if (_direction == -1 && distanceFromStart <= 0) {
                _direction = 1;
                Flip(true);
            }

            if (_hp <= 0) {
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

        private void Flip(bool right) {
            if (_spriteRenderer != null) {
                _spriteRenderer.flipX = right;
            }
        }

        private void EnemyDeath() {
            _hp = 0;
            Destroy(gameObject);
        }

        private IEnumerator FlashRed(float duration = 0.3f) {
            var originalColor = _spriteRenderer.color;
            var elapsed = 0f;

            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                _spriteRenderer.color = Color.Lerp(Color.red, originalColor, elapsed / duration);
                yield return null;
            }

            _spriteRenderer.color = originalColor;

        }

        public void TakeDamage(int damage) {
            _hp -= damage;
            print($"Enemy took {damage} damage, HP: {_hp}");
            StartCoroutine(FlashRed());
        }


        private void OnCollisionEnter2D(Collision2D other) {
            if (!_canHit) return;

            if (other.gameObject.CompareTag("Player") && other.gameObject.TryGetComponent<ILivingEntity>(out var livingEntity)) {
                livingEntity.TakeDamage(_damage);
            }
        }
    }
}