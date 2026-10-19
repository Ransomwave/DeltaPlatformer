using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace deltaformer {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 5f;

        private Vector2 _moveDirection;

        private Rigidbody2D _rigidBody;
        private bool isGrounded = false;

        private InputAction _moveAction;
        private InputAction _attackAction;
        private InputAction _jumpAction;

        public event Action OnPlayerAttack = delegate { };

        void Awake() {
            _rigidBody = GetComponent<Rigidbody2D>();
            _moveAction = InputSystem.actions.FindAction("Player/Move");
            _attackAction = InputSystem.actions.FindAction("Player/Attack");
            _jumpAction = InputSystem.actions.FindAction("Player/Jump");
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

        private void OnMove(InputAction.CallbackContext context) => _moveDirection = context.ReadValue<Vector2>();

        private void OnJump(InputAction.CallbackContext context) {
            if (!isGrounded) {
                return;
            }

            _rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }

        private void OnAttack(InputAction.CallbackContext context) {
            OnPlayerAttack.Invoke();
            print("Player attacked!!!");
        }

        void FixedUpdate() {
            // Set horizontal speed, but preserve the current falling/rising Y physics speed
            _rigidBody.linearVelocity = new Vector2(_moveDirection.x * speed, _rigidBody.linearVelocity.y);
        }

        void OnCollisionEnter2D(Collision2D collision) {
            if (collision.GetContact(0).normal.y > 0.5f) {
                isGrounded = true;
            }
        }

    }
}
