using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace GMTK25
{
    public class Collectable : MonoBehaviour, ICircleable
    {
        [Header("Movement Settings")]
        public float baseSpeed = 3f;
        public float speedVariation = 1f;
        public float lifetime = 12f;

        public UnityEvent onCollected;

        private Vector2 direction;
        private Vector2 startPosition;
        private float actualSpeed;
        private float timeAlive;
        private Vector2 screenBounds;
        private Rigidbody2D rb;

        private float _loopModeMultiplier = 1f;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            rb.gravityScale = 0f;
        }

        private void Start()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector2 bounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
                Initialize(transform.position, bounds);
            }
        }

        public void Initialize(Vector2 spawnPos, Vector2 bounds)
        {
            startPosition = spawnPos;
            screenBounds = bounds;
            actualSpeed = baseSpeed + Random.Range(-speedVariation, speedVariation);

            // Calculate direction toward center or opposite edge
            CalculateDirection();
        }

        void CalculateDirection()
        {
            Vector2 centerScreen = Vector2.zero;
            Vector2 toCenter = (centerScreen - startPosition).normalized;

            // Add some randomness to direction
            float angle = Mathf.Atan2(toCenter.y, toCenter.x) + Random.Range(-0.5f, 0.5f);
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        void Update()
        {
            if (_loopModeMultiplier != 0f)
                timeAlive += Time.deltaTime;

            // Destroy if lifetime exceeded or off screen
            if (timeAlive > lifetime || IsOffScreen())
            {
                DestroyCollectable();
                return;
            }

            UpdateMovement();
        }

        void UpdateMovement()
        {
            Vector2 movement = Vector2.zero;
            
            movement = direction * actualSpeed * _loopModeMultiplier;

            rb.linearVelocity = movement;
        }

        bool IsOffScreen()
        {
            Vector2 pos = transform.position;
            float buffer = 2f;

            return pos.x < -screenBounds.x - buffer ||
                   pos.x > screenBounds.x + buffer ||
                   pos.y < -screenBounds.y - buffer ||
                   pos.y > screenBounds.y + buffer;
        }

        void DestroyCollectable()
        {
            Destroy(gameObject);
        }

        public IEnumerator WaitForJuiceOnCollection(float duration)
        {
            GameManager.Instance.AddScore(100);
            onCollected?.Invoke();
            yield return new WaitForSeconds(duration);
            DestroyCollectable();
        }

        public void OnCircled()
        {
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(WaitForJuiceOnCollection(1f));
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }
        public void SetLoopMode(bool isLooping)
        {
            DOTween.Kill(this);
            DOTween.To(() => _loopModeMultiplier, x => _loopModeMultiplier = x, isLooping ? 0f : 1f, 0.25f);
        }
        void OnEnable()
        {
            EventManager.OnLoopModeToggled += SetLoopMode;
        }

        void OnDisable()
        {
            EventManager.OnLoopModeToggled -= SetLoopMode;
        }
    }
}
