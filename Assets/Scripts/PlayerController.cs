using UnityEngine;
using UnityEngine.InputSystem;

namespace GMTK25
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private Vector2 _moveInput;
        
        private Rigidbody2D _rigidbody2D;

        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _wiggleStrength = 30f; // How much force movement applies to rotation
        [SerializeField] private float _springStrength = 10f; // How strong the spring back is
        [SerializeField] private float _damping = 2f;         // How much to dampen the wiggle

        private float _currentRotation = 0f;
        private float _rotationVelocity = 0f;

        void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Move"];
            
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            _moveInput = _moveAction.ReadValue<Vector2>();
        }

        void FixedUpdate()
        {
            _rigidbody2D.AddForce(_moveInput * _moveSpeed, ForceMode2D.Force);

            // Wiggle effect
            if (_moveInput.sqrMagnitude > 0.001f)
            {
                // Apply force to rotation based on movement direction
                float targetAngle = Mathf.Atan2(_moveInput.y, _moveInput.x) * Mathf.Rad2Deg;
                float angleDelta = Mathf.DeltaAngle(_currentRotation, targetAngle);
                _rotationVelocity += angleDelta * _wiggleStrength * Time.fixedDeltaTime;
            }

            // Spring back to 0 rotation (default orientation)
            _rotationVelocity += -_currentRotation * _springStrength * Time.fixedDeltaTime;

            // Damping
            _rotationVelocity *= Mathf.Exp(-_damping * Time.fixedDeltaTime);

            // Apply rotation
            _currentRotation += _rotationVelocity * Time.fixedDeltaTime;
            _rigidbody2D.MoveRotation(_currentRotation);
        }
    }
}