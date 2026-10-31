using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace deltaformer {
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, ILivingEntity {
        [SerializeField] private float _speed = 6f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _invincibilityTime = 1f; // Invincibility time in seconds after taking damage

        [Header("Attack Settings")]
        [SerializeField] private float _attackCooldown = 0.3f;
        private const float _attackPushForce = 5f; // Force applied when attacking
        private const float _attackDuration = 0.2f;

        private int _hp = 3;

        private Vector2 _moveDirection;
        private Vector2 _currentLookDirection = Vector2.right;

        private Rigidbody2D _rigidBody;
        private Collider2D _playerCollider; // Guardamos el colisionador del cuerpo del jugador
        private Collider2D _hitboxCollider;

        private bool _canAttack = true;
        private bool _isAttacking = false;
        private bool _isGrounded = false;
        private bool _isInvincible = false;

        private InputAction _moveAction;
        private InputAction _attackAction;
        private InputAction _jumpAction;

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        // contacts whose normal points up (floors)
        // better than old method because this also ungrounds the plr when walking off ledges
        private static readonly ContactFilter2D GroundFilter = new() {
            useNormalAngle = true,
            minNormalAngle = 45f,
            maxNormalAngle = 135f
        };

        public event Action OnPlayerAttack = delegate { };
        public event Action OnPlayerDie = delegate { };
        public event Action OnPlayerDamaged = delegate { };

        private void Awake() {
            _animator = GetComponentInChildren<Animator>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _playerCollider = GetComponent<Collider2D>(); // Obtenemos el colisionador principal del cuerpo
            _hitboxCollider = transform.Find("Hitbox").GetComponent<Collider2D>();

            _moveAction = InputSystem.actions.FindAction("Player/Move");
            _attackAction = InputSystem.actions.FindAction("Player/Attack");
            _jumpAction = InputSystem.actions.FindAction("Player/Jump");

            _hitboxCollider.enabled = false;
        }

        private void OnEnable() {
            _moveAction.Enable();
            _attackAction.Enable();
            _jumpAction.Enable();

            _moveAction.performed += OnMove;
            _moveAction.canceled += OnMove;

            _jumpAction.performed += OnJump;

            _attackAction.performed += OnAttack;
        }

        private void OnDisable() {
            _moveAction.Disable();
            _attackAction.Disable();
            _jumpAction.Disable();

            _moveAction.performed -= OnMove;
            _moveAction.canceled -= OnMove;
            _jumpAction.performed -= OnJump;
            _attackAction.performed -= OnAttack;
        }

        private void OnMove(InputAction.CallbackContext context) {
            if (_isAttacking) {
                _moveDirection = Vector2.zero;
                return;
            }

            _moveDirection = context.ReadValue<Vector2>();

            if (_moveDirection.x > 0) {
                _currentLookDirection = Vector2.right;
            } else if (_moveDirection.x < 0) {
                _currentLookDirection = Vector2.left;
            }
        }

        private void OnJump(InputAction.CallbackContext context) {
            if (!_isGrounded || _isAttacking) {
                return;
            }

            _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _isGrounded = false;
        }

        private void OnAttack(InputAction.CallbackContext context) {
            if (_isAttacking || !_canAttack) {
                return;
            }

            _isAttacking = true;

            OnPlayerAttack.Invoke();
            _hitboxCollider.enabled = true;

            _animator.SetTrigger("Attack");

            // push slightly towards the direction we're facing
            _rigidBody.AddForce(_currentLookDirection * _attackPushForce, ForceMode2D.Impulse);

            if (_currentLookDirection.x > 0) {
                _hitboxCollider.transform.localPosition = new Vector3(0.5f, 0, 0);
            } else if (_currentLookDirection.x < 0) {
                _hitboxCollider.transform.localPosition = new Vector3(-0.5f, 0, 0);
            }

            StartCoroutine(OnAttackEnd());
            print("Player attacked!!!");
        }

        private IEnumerator OnAttackEnd() {
            _canAttack = false;
            yield return new WaitForSeconds(_attackDuration);
            _hitboxCollider.enabled = false;
            _isAttacking = false;
            _hitboxCollider.transform.localPosition = new Vector3(0, 0, 0);

            yield return new WaitForSeconds(_attackCooldown);
            _canAttack = true;
        }

        private void FixedUpdate() {
            _isGrounded = _rigidBody.IsTouching(GroundFilter);

            if (_isAttacking) {
                return;
            }

            // Set horizontal speed, but preserve the current falling/rising Y physics speed
            _rigidBody.linearVelocity = new Vector2(_moveDirection.x * _speed, _rigidBody.linearVelocity.y);
        }

        private void Update() {
            if (!_isAttacking) {
                _moveDirection = _moveAction.ReadValue<Vector2>();
            }

            // DETECCION DE BAJADA DE PLATAFORMA (Corregido para ignorar al propio jugador)
            if (_moveDirection.y < -0.5f && _isGrounded) {
                Vector2 posicionPies = new Vector2(transform.position.x, transform.position.y - 1.1f);
                Vector2 tamanoCaja = new Vector2(0.6f, 0.3f);

                // Recogemos todos los colisionadores debajo de los pies
                Collider2D[] colisionesDetectadas = Physics2D.OverlapBoxAll(posicionPies, tamanoCaja, 0f);

                foreach (Collider2D col in colisionesDetectadas) {
                    // Si el colisionador no soy yo mismo y tiene el PlatformEffector2D
                    if (col != _playerCollider && col.GetComponent<PlatformEffector2D>() != null) {
                        StartCoroutine(DesactivarColisionTemporal(col));
                        break; // Salimos del bucle una vez encontrada la plataforma
                    }
                }
            }

            _animator.SetFloat("Speed", Mathf.Abs(_moveDirection.x));
            _spriteRenderer.flipX = _currentLookDirection.x < 0;
            _animator.SetBool("IsGrounded", _isGrounded);
            _animator.SetFloat("VerticalSpeed", _rigidBody.linearVelocity.y);
        }

        private IEnumerator DesactivarColisionTemporal(Collider2D plataformaCollider) {
            // Buscamos el componente colisionador del Tilemap entero
            Collider2D tilemapColliderCompleto = plataformaCollider.gameObject.GetComponent<Collider2D>();

            if (tilemapColliderCompleto != null) {
                Physics2D.IgnoreCollision(_playerCollider, tilemapColliderCompleto, true);
                _isGrounded = false;

                // Tiempo para atravesar el grosor del bloque
                yield return new WaitForSeconds(0.80f);

                if (tilemapColliderCompleto != null) {
                    Physics2D.IgnoreCollision(_playerCollider, tilemapColliderCompleto, false);
                }
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Vector2 posicionPies = new Vector2(transform.position.x, transform.position.y - 1.1f);
            Gizmos.DrawWireCube(posicionPies, new Vector2(0.6f, 0.3f));
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

            StartCoroutine(HandleHitInvincibility(_invincibilityTime));

            OnPlayerDamaged.Invoke();

            if (_hp <= 0) {
                OnPlayerDie.Invoke();
            }
        }
    }
}
