using System.Collections;
using UnityEngine;

// handler for leaves that make the player fall off them if they stay on them for too long
namespace deltaformer {
    [RequireComponent(typeof(BoxCollider2D))] // makes sure the leaf has a collider for you, really cool!
    [RequireComponent(typeof(SpriteRenderer))]
    public class LeafHandler : MonoBehaviour {
        [SerializeField] private bool _shouldFall = false;
        [SerializeField] private float _recoverAfter = 1f;
        [SerializeField] private float _animationTime = .2f;
        [SerializeField] private Sprite[] _leafFrames;

        private BoxCollider2D _collider;
        private SpriteRenderer _spriteRenderer;

        private int _currentFrame = 0;
        private bool _isPlayerOnLeaf = false;
        private Coroutine _fallCoroutine;

        private void Awake() {
            _collider = GetComponent<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private IEnumerator PlayFrames() {
            for (var i = 0; i < _leafFrames.Length; i++) {
                _currentFrame = i;
                _spriteRenderer.sprite = _leafFrames[i];

                if (i != _leafFrames.Length) {
                    yield return new WaitForSeconds(_animationTime);
                }
            }

            yield return StartCoroutine(Fall());
        }

        private IEnumerator PlayFramesReverse() {
            for (var i = _currentFrame; i >= 0; i--) {
                _currentFrame = i;
                _spriteRenderer.sprite = _leafFrames[i];
                yield return new WaitForSeconds(_animationTime);
            }
        }

        private IEnumerator Fall() {
            if (!_isPlayerOnLeaf) {
                yield return null;
            }
            _isPlayerOnLeaf = false;

            _collider.enabled = false;

            yield return new WaitForSeconds(_recoverAfter);

            _collider.enabled = true;
        }

        private void OnCollisionStay2D(Collision2D collision) {
            if (!collision.gameObject.name.Contains("Player") || !_shouldFall || _fallCoroutine != null) {
                return;
            }

            // don't trigger the leaf if the player is passing through it
            if (collision.collider.bounds.min.y < _collider.bounds.max.y - 0.05f) {
                return;
            }

            _isPlayerOnLeaf = true;

            _fallCoroutine = StartCoroutine(PlayFrames());
        }

        private void OnCollisionExit2D(Collision2D collision) {
            if (!collision.gameObject.name.Contains("Player")) {
                return;
            }

            if (_fallCoroutine != null) {
                StopCoroutine(_fallCoroutine);
                _fallCoroutine = null;
                StartCoroutine(PlayFramesReverse());
            }

            _isPlayerOnLeaf = false;
        }
    }
}