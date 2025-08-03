using UnityEngine;

namespace GMTK25
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private int score;
        private float timeElapsed;
        private bool gameActive = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            score = 0;
            timeElapsed = 0f;
        }

        private void Update()
        {
            if (gameActive)
            {
                timeElapsed += Time.deltaTime;
            }
        }

        public void AddScore(int points)
        {
            score += points;
            Debug.Log("Score: " + score);
        }

        public void RestartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
