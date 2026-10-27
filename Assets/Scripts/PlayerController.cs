using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace deltaformer {
    public class PlayerController : MonoBehaviour, ILivingEntity {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float invincibilityTime = 1f; // Invincibility time in seconds after taking damage

        private int _hp = 3;

        private Vector2 _moveDirection;
        private Vector2 _currentLookDirection = Vector2.right;

        private Rigidbody2D _rigidBody;
        private Collider2D _hitboxCollider;

        private bool _isAttacking = false;
        private bool _isGrounded = false;
        private bool _isInvincible = false;

        private InputAction _moveAction;
        private InputAction _attackAction;
        private InputAction _jumpAction;

        public event Action OnPlayerAttack = delegate { };
        public event Action OnPlayerDie = delegate { };
        public event Action OnPlayerDamaged = delegate { };

        void Awake() {
            _rigidBody = GetComponent<Rigidbody2D>();
            _hitboxCollider = transform.Find("Hitbox").GetComponent<Collider2D>();
            _moveAction = InputSystem.actions.FindAction("Player/Move");
            _attackAction = InputSystem.actions.FindAction("Player/Attack");
            _jumpAction = InputSystem.actions.FindAction("Player/Jump");

            _hitboxCollider.enabled = false;
        }

        void OnEnable() {
            _moveAction.Enable();
            _attackAction.Enable();
            _jumpAction.Enable();

            _moveAction.performed += OnMove;
            _moveAction.canceled += OnMove;

            _jumpAction.performed += OnJump;

            _attackAction.performed += OnAttack;
        }

        void OnDisable() {
            _moveAction.Disable();
            _attackAction.Disable();
            _jumpAction.Disable();

            _moveAction.performed -= OnMove;
            _moveAction.canceled -= OnMove;
            _jumpAction.performed -= OnJump;
            _attackAction.performed -= OnAttack;
        }

        private void OnMove(InputAction.CallbackContext context) {
            _moveDirection = context.ReadValue<Vector2>();

            if (_moveDirection.x > 0) {
                _currentLookDirection = Vector2.right;
            } else if (_moveDirection.x < 0) {
                _currentLookDirection = Vector2.left;
            }
        }

        private void OnJump(InputAction.CallbackContext context) {
            if (!_isGrounded) {
                return;
            }

            _rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _isGrounded = false;
        }

        private void OnAttack(InputAction.CallbackContext context) {
            if (_isAttacking) {
                return;
            }

            _isAttacking = true;

            OnPlayerAttack.Invoke();
            _hitboxCollider.enabled = true;

            if (_currentLookDirection.x > 0) {
                _hitboxCollider.transform.localPosition = new Vector3(0.5f, 0, 0);
            } else if (_currentLookDirection.x < 0) {
                _hitboxCollider.transform.localPosition = new Vector3(-0.5f, 0, 0);
            }

            StartCoroutine(OnAttackEnd());
            print("Player attacked!!!");
        }

        private IEnumerator OnAttackEnd() {
            yield return new WaitForSeconds(0.2f);
            _hitboxCollider.enabled = false;
            _isAttacking = false;
            _hitboxCollider.transform.localPosition = new Vector3(0, 0, 0);
        }

        void FixedUpdate() {
            // Set horizontal speed, but preserve the current falling/rising Y physics speed
            _rigidBody.linearVelocity = new Vector2(_moveDirection.x * speed, _rigidBody.linearVelocity.y);
        }

        void OnCollisionEnter2D(Collision2D collision) {
            if (collision.GetContact(0).normal.y > 0.5f) {
                _isGrounded = true;
            }
        }

        public IEnumerator HandleHitInvincibility(float duration) {
            _isInvincible = true;
            yield return new WaitForSeconds(duration);
            _isInvincible = false;
        }

        public void TakeDamage(int damage) {
            _hp -= damage;

            print($"Player took {damage} damage! HP: {_hp}");

            if (_isInvincible) {
                return;
            }

            StartCoroutine(HandleHitInvincibility(invincibilityTime));

            OnPlayerDamaged.Invoke();

            if (_hp <= 0) {
                OnPlayerDie.Invoke();
            }
        }
    }
}
