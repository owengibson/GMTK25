using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GMTK25
{
    public class LoopDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float _detectionRadius = 0.15f;
        [SerializeField] private float _minLoopTime = 1f;
        [SerializeField] private float _minLoopArea = 0.5f;

        private TrailRenderer _trailRenderer;
        private GameObject _trailCollider;
        private EdgeCollider2D _edgeCollider;
        private CircleCollider2D _playerDetector;
        private float _trailStartTime;
        private bool _canDetectLoop = false;

        void Start()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
            _playerDetector = GetComponent<CircleCollider2D>();
            StartNewTrail();
        }

        private void StartNewTrail()
        {
            _trailStartTime = Time.time;
            _canDetectLoop = false;
        }

        void Update()
        {
            if (!_canDetectLoop && Time.time - _trailStartTime >= _minLoopTime)
            {
                _canDetectLoop = true;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (_canDetectLoop && collision == _edgeCollider)
            {
                OnLoopDetected();
            }
        }

        private void OnLoopDetected()
        {
            Debug.Log("Loop detected!");

            // area calculation, other logic

            ResetTrail();
        }

        private void ResetTrail()
        {
            _trailRenderer.Clear();

            StartNewTrail();
        }
        
        public void SetTrailColliderReference(GameObject trailColliderObj, EdgeCollider2D edgeCollider)
        {
            _trailCollider = trailColliderObj;
            _edgeCollider = edgeCollider;

            _edgeCollider.isTrigger = true;
        }
    }
}
