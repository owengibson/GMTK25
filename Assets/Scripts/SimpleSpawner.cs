using UnityEngine;
using System.Collections.Generic;

namespace GMTK25
{
    public class SimpleSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject enemyPrefab;
        public GameObject collectablePrefab;

        [Header("Spawn Rates")]
        public float enemySpawnRate = 6f;
        public float collectableSpawnRate = 4f;
        public int maxEnemies = 5;
        public int maxCollectables = 8;

        [Header("Spawn Zones")]
        public bool spawnFromEdges = true;
        public float edgeBuffer = 1f;

        private float enemyTimer;
        private float collectableTimer;
        private Camera cam;

        private List<GameObject> enemies = new List<GameObject>();
        private List<GameObject> collectables = new List<GameObject>();

        private bool _isLoopMode = false;

        void Start()
        {
            cam = Camera.main;
        }

        void Update()
        {
            CleanupDestroyedObjects();
            SpawnEnemies();
            SpawnCollectables();
        }

        void SpawnEnemies()
        {
            if (enemyPrefab == null) return;
            if (enemies.Count >= maxEnemies) return;
            if (_isLoopMode) return;

            enemyTimer += Time.deltaTime;
            if (enemyTimer >= enemySpawnRate)
            {
                Vector2 spawnPos = spawnFromEdges ? GetEdgeSpawnPosition() : GetRandomSpawnPosition();
                GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                enemies.Add(newEnemy);
                enemyTimer = 0f;
            }
        }

        void SpawnCollectables()
        {
            if (collectablePrefab == null) return;
            if (collectables.Count >= maxCollectables) return;
            if (_isLoopMode) return;

            collectableTimer += Time.deltaTime;
            if (collectableTimer >= collectableSpawnRate)
            {
                Vector2 spawnPos = spawnFromEdges ? GetEdgeSpawnPosition() : GetRandomSpawnPosition();
                GameObject newCollectable = Instantiate(collectablePrefab, spawnPos, Quaternion.identity);
                collectables.Add(newCollectable);
                collectableTimer = 0f;
            }
        }

        Vector2 GetRandomSpawnPosition()
        {
            Vector3 screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            return new Vector2(
                Random.Range(-screenBounds.x * 0.8f, screenBounds.x * 0.8f),
                Random.Range(-screenBounds.y * 0.8f, screenBounds.y * 0.8f)
            );
        }

        Vector2 GetEdgeSpawnPosition()
        {
            Vector3 screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            // Choose random edge
            int edge = Random.Range(0, 4);

            switch (edge)
            {
                case 0: // Top
                    return new Vector2(Random.Range(-screenBounds.x, screenBounds.x), screenBounds.y + edgeBuffer);
                case 1: // Bottom  
                    return new Vector2(Random.Range(-screenBounds.x, screenBounds.x), -screenBounds.y - edgeBuffer);
                case 2: // Left
                    return new Vector2(-screenBounds.x - edgeBuffer, Random.Range(-screenBounds.y, screenBounds.y));
                case 3: // Right
                    return new Vector2(screenBounds.x + edgeBuffer, Random.Range(-screenBounds.y, screenBounds.y));
                default:
                    return GetRandomSpawnPosition();
            }
        }

        void CleanupDestroyedObjects()
        {
            enemies.RemoveAll(enemy => enemy == null);
            collectables.RemoveAll(collectable => collectable == null);
        }

        void OnEnable()
        {
            EventManager.OnLoopModeToggled += (x) => _isLoopMode = x;
        }
        void OnDisable()
        {
            EventManager.OnLoopModeToggled -= (x) => _isLoopMode = x;
        }
    }
}