using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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

        void Awake()
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

        
        private void OnTriggerEnter2D(Collider2D collision) 
        { 
            if (_canDetectLoop && collision == _edgeCollider)
            {
                OnLoopDetected();
            }
        }

        private void OnLoopDetected()
        {
            float area = CalculateLoopArea();
            if (area <= _minLoopArea) return;

            Debug.Log("Loop detected with area: " + area);


            ResetTrail();
        }

        private void ResetTrail()
        {
            _trailRenderer.Clear();

            StartNewTrail();
        }

        private float CalculateLoopArea()
        {
            if (_edgeCollider == null || _edgeCollider.points.Length < 3) return 0f;

            // Find the index of the collision point in the edge collider points
            // int index = Array.IndexOf(_edgeCollider.points, collisionPoint);
            Vector2[] points = _edgeCollider.points;  //[index..];

            float area = 0f;

            for (int i = 0; i < points.Length; i++)
            {
                int j = (i + 1) % points.Length;
                area += points[i].x * points[j].y;
                area -= points[j].x * points[i].y;
            }

            return Mathf.Abs(area) / 2f;
        }
        
        public void SetTrailColliderReference(GameObject trailColliderObj, EdgeCollider2D edgeCollider)
        {
            _trailCollider = trailColliderObj;
            _edgeCollider = edgeCollider;
        }
    }
}
