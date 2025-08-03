using UnityEngine;

namespace GMTK25
{
    public class Enemy : MonoBehaviour, ICircleable
    {
        public float moveSpeed = 1f;
        public float separationDistance = .1f;

        private Vector2 moveDirection;
        private Camera cam;
        private PlayerController playerController;

        void Start()
        {
            cam = Camera.main;

            playerController = FindFirstObjectByType<PlayerController>();
            FindDirectionToPlayer();
        }

        void Update()
        {
            FindDirectionToPlayer();

            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            Vector3 pos = transform.position;
            Vector3 screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            pos.x = Mathf.Clamp(pos.x, -screenBounds.x, screenBounds.x);
            pos.y = Mathf.Clamp(pos.y, -screenBounds.y, screenBounds.y);
            transform.position = pos;
        }

        void FindDirectionToPlayer()
        {
            Vector2 finalDirection = Vector2.zero;

            // Direction toward player
            if (playerController != null)
            {
                Vector3 playerPos = playerController.transform.position;
                Vector2 toPlayer = (playerPos - transform.position).normalized;
                finalDirection += toPlayer;
            }

            // Separation from other enemies
            Vector2 separation = GetSeparationForce();
            finalDirection += separation;

            // Normalize final direction
            moveDirection = finalDirection.normalized;
        }

        Vector2 GetSeparationForce()
        {
            Vector2 separationForce = Vector2.zero;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy == this.gameObject) continue; // Skip self

                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < separationDistance && distance > 0)
                {
                    Vector2 awayFromEnemy = (transform.position - enemy.transform.position).normalized;
                    float strength = (separationDistance - distance) / separationDistance;
                    separationForce += awayFromEnemy * strength;
                }
            }

            return separationForce;
        }

        public void DestroyEnemy()
        {
            Destroy(gameObject);
        }

        public void OnCircled()
        {
            GameManager.Instance.AddScore(50);
            Destroy(gameObject);
        }
    }
}
