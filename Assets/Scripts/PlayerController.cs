using Unity.VisualScripting;
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
        [SerializeField] private float _rotationSpeed = 1; // How much horizontal input affects rotation
        [SerializeField] private float _rotationDamping = 5f; // How quickly rotation slows down

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
            _rigidbody2D.AddForce(transform.up * _moveInput.y * _moveSpeed, ForceMode2D.Force);

            // Add rotation velocity based on horizontal input
            _rotationVelocity += -_moveInput.x * _rotationSpeed;

            // Damping
            _rotationVelocity = Mathf.Lerp(_rotationVelocity, 0, _rotationDamping * Time.fixedDeltaTime);

            // Apply rotation
            _currentRotation += _rotationVelocity * Time.fixedDeltaTime;
            _rigidbody2D.MoveRotation(_currentRotation);
        }
    }
}