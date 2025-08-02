using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GMTK25
{
    [RequireComponent(typeof(TrailRenderer))]
    public class TrailCollisions : MonoBehaviour
    {
        private TrailRenderer _trailRenderer;
        private EdgeCollider2D _edgeCollider;

        private static List<EdgeCollider2D> _unusedColliders = new List<EdgeCollider2D>();
        


        void Awake()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
            _edgeCollider = GetValidCollider();
        }

        void Update()
        {
            SetColliderPointsFromTrail(_trailRenderer, _edgeCollider);
        }


        private EdgeCollider2D GetValidCollider()
        {
            EdgeCollider2D validCollider;
            if (_unusedColliders.Count > 0)
            {
                validCollider = _unusedColliders[0];
                validCollider.enabled = true;
                _unusedColliders.RemoveAt(0);
            }
            else
            {
                validCollider = new GameObject("TrailCollider", typeof(EdgeCollider2D)).GetComponent<EdgeCollider2D>();
                validCollider.isTrigger = true;
                
                if (TryGetComponent(out LoopDetector loopDetector))
                {
                    loopDetector.SetTrailColliderReference(validCollider.gameObject, validCollider);
                }
            }

            return validCollider;
        }

        private void SetColliderPointsFromTrail(TrailRenderer trail, EdgeCollider2D collider)
        {
            List<Vector2> points = new List<Vector2>();

            if (trail.positionCount == 0)
            {
                points.Add(transform.position);
                points.Add(transform.position);
            }

            else for (int i = 0; i < trail.positionCount; i++)
                {
                    points.Add(trail.GetPosition(i));
                }

            collider.SetPoints(points);
        }

        void OnDestroy()
        {
            if (!_edgeCollider)
            {
                _edgeCollider.enabled = false;
                _unusedColliders.Add(_edgeCollider);
            }
        }

    }
}
