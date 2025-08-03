using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GMTK25
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _loopModeAction;
        private Vector2 _moveInput;

        private Rigidbody2D _rigidbody2D;
        private TrailRenderer _loopTrail;

        [SerializeField] private float _loopModeDuration = 5f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 1; // How much horizontal input affects rotation
        [SerializeField] private float _rotationDamping = 5f; // How quickly rotation slows down

        [Space]
        [SerializeField] private TrailRenderer _normalTrail;

        private float _currentRotation = 0f;
        private float _rotationVelocity = 0f;
        
        private bool _canEnterLoopMode = true;
        private float _loopModeActivationDelay = 2f; // Delay before the player can enter loop mode again
        private Tween _loopModeTimerTween;
        private Tween _loopModeActivationDelayTween;

        void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Move"];
            _loopModeAction = _playerInput.actions["LoopMode"];

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _loopTrail = GetComponent<TrailRenderer>();

            _normalTrail.emitting = true;
            _loopTrail.emitting = false;
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

        private void OnLoopModeStarted(InputAction.CallbackContext context)
        {
            if (!_canEnterLoopMode) return;

            EventManager.OnLoopModeToggled?.Invoke(true);
            _normalTrail.emitting = false;
            _loopTrail.Clear();
            _loopTrail.emitting = true;

            _loopModeTimerTween = DOVirtual.DelayedCall(_loopModeDuration, () =>
            {
                OnLoopModeEnded(new InputAction.CallbackContext());
            });
            
            Debug.Log("Loop mode activated. Time stopped.");
        }
        
        public void OnLoopModeEnded(InputAction.CallbackContext context)
        {
            if (!_canEnterLoopMode) return;

            _loopModeTimerTween?.Kill();
            EventManager.OnLoopModeToggled?.Invoke(false);
            _loopTrail.emitting = false;
            _normalTrail.emitting = true;
            _loopTrail.Clear();
            _normalTrail.Clear();

            _canEnterLoopMode = false;
            _loopModeAction.Disable();

            _loopModeActivationDelayTween = DOVirtual.DelayedCall(_loopModeActivationDelay, () =>
            {
                _canEnterLoopMode = true;  
                _loopModeAction.Enable();
            }); 
            
            Debug.Log("Loop mode deactivated. Time resumed.");
        }

        void OnEnable()
        {
            _loopModeAction.started += OnLoopModeStarted;
            _loopModeAction.canceled += OnLoopModeEnded;
        }


        void OnDisable()
        {
            _loopModeAction.started -= OnLoopModeStarted;
            _loopModeAction.canceled -= OnLoopModeEnded;
        }
    }
}