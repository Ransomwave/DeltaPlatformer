using System.Collections;
using UnityEngine;

namespace deltaformer {
    public class CrusherBehavior : MonoBehaviour {
        [SerializeField] private GameObject _stone;
        private Hitbox _hitbox;
        private Rigidbody2D _stoneRb;
        private Vector2 _stoneStartPos;
        private Vector2 _stoneTargetPos;

        private void Awake() {
            _stoneStartPos = _stone.transform.position;
            _stoneTargetPos = _stoneStartPos + new Vector2(0, -4.8f);
            _hitbox = GetComponentInChildren<Hitbox>();
            _stoneRb = _stone.GetComponent<Rigidbody2D>();
        }

        private void OnEnable() {
            if (_hitbox)
                Hitbox.OnEnter += OnHitBox;
        }

        private void OnDisable() {
            if (_hitbox)
                Hitbox.OnEnter -= OnHitBox;
        }

        private void OnHitBox() {
            Debug.Log("Enemigo enfadar");
            StartCoroutine(MoveStone(_stoneStartPos, _stoneTargetPos));
        }

        private IEnumerator MoveStone(Vector2 origin, Vector2 target) {
            yield return new WaitForSeconds(.5f);

            var duration = 1f;
            var elapsed = 0f;

            while (elapsed < duration) {
                elapsed += Time.deltaTime;

                _stoneRb.MovePosition(Vector2.Lerp(origin, target, easeOutBounce(elapsed)));
                yield return null;
            }

            yield return new WaitForSeconds(1);

            elapsed = 0f;
            while (elapsed < duration) {
                elapsed += Time.deltaTime;

                _stoneRb.MovePosition(Vector2.Lerp(target, origin, easeOutBack(elapsed)));
                yield return null;
            }

            _hitbox.PlayerDetected = false;
        }

        private float easeOutBounce(float x) {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (x < 1f / d1) return n1 * x * x;

            if (x < 2f / d1) {
                var t = x - 1.5f / d1;
                return n1 * t * t + 0.75f;
            }

            if (x < 2.5f / d1) {
                var t = x - 2.25f / d1;
                return n1 * t * t + 0.9375f;
            }

            {
                var t = x - 2.625f / d1;
                return n1 * t * t + 0.984375f;
            }
        }

        private float easeOutBack(float x) {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            x = Mathf.Clamp01(x);

            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }
    }
}