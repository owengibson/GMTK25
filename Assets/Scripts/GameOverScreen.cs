using TMPro;
using UnityEngine;

namespace GMTK25
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI finalScoreText;

        private void OnEnable()
        {
            int finalScore = PlayerPrefs.GetInt("Score", 0);
            finalScoreText.text = "Final Score: " + finalScore;
        }

        void Update()
        {
            if (Input.anyKeyDown)
            {
                // Restart the game or go back to the main menu
                PlayerPrefs.SetInt("Score", 0); // Reset score
                UnityEngine.SceneManagement.SceneManager.LoadScene("Charlie");
            }
        }
    }
}
