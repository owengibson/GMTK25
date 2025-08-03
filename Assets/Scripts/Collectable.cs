using UnityEngine;

namespace GMTK25
{
    public class Collectable : MonoBehaviour, ICircleable
    {
        [Header("Movement Settings")]
        public float baseSpeed = 3f;
        public float speedVariation = 1f;
        public float lifetime = 12f;

        private Vector2 direction;
        private Vector2 startPosition;
        private float actualSpeed;
        private float timeAlive;
        private Vector2 screenBounds;
        private Rigidbody2D rb;

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
            
            movement = direction * actualSpeed;

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

        public void OnCircled()
        {
            GameManager.Instance.AddScore(10);
            DestroyCollectable();
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }
    }
}
