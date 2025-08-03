using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Collectable"))
            {
                Destroy(other.gameObject);
            }
        }

        private void OnLoopDetected()
        {
            float area = CalculateLoopArea();
            List<ICircleable> objs = FindObjectsInLoop(_edgeCollider.points);
            if (area <= _minLoopArea || objs.Count == 0) return;

            Debug.Log("Loop detected with area: " + area);
            foreach (var obj in objs)
            {
                obj.OnCircled();
                Debug.Log("Circled object");
            }



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
            int index = 0;
            for (int i = 0; i < _edgeCollider.points.Length; i++)
            {
                if (Vector2.Distance(_edgeCollider.points[i], _playerDetector.transform.position) < _detectionRadius)
                {
                    index = i;
                    break;
                }
            }
            Vector2[] points = _edgeCollider.points[index..];

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

        private List<ICircleable> FindObjectsInLoop(Vector2[] loop)
        {
            List<ICircleable> capturedObjects = new List<ICircleable>();

            ICircleable[] allCircleables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICircleable>().ToArray();

            foreach (var obj in allCircleables)
            {
                Vector2 objPosition = obj.GetPosition();

                if (IsPointInPolygon(objPosition, loop))
                {
                    capturedObjects.Add(obj);
                }
            }
            return capturedObjects;
        }

        private bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
        {
            int intersectionCount = 0;

            for (int i = 0; i < polygon.Length; i++)
            {
                Vector2 v1 = polygon[i];
                Vector2 v2 = polygon[(i + 1) % polygon.Length];

                if (DoesRayCrossEdge(point, v1, v2))
                {
                    intersectionCount++;
                }
            }

            return (intersectionCount % 2) == 1;
        }

        private bool DoesRayCrossEdge(Vector2 point, Vector2 edgeStart, Vector2 edgeEnd)
        {
            // Cast horizontal ray to the right from point
            // Edge must cross horizontal line through the point
            if (edgeStart.y > point.y == edgeEnd.y > point.y)
                return false;

            // Calculate where edge intersects with horizontal line
            float xIntersection = edgeStart.x + (point.y - edgeStart.y) / (edgeEnd.y - edgeStart.y) * (edgeEnd.x - edgeStart.x);

            // Ray crosses edge if intersection is to the right of point
            return point.x < xIntersection;
        }
    }
}
