using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GMTK25
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI uiText;
        private string baseText = "Press any key to play";
        private float delay = 0f;

        private void OnEnable()
        {
            int finalScore = PlayerPrefs.GetInt("Score", 0);
            finalScoreText.text = "Final Score: " + finalScore;
            
            AnimateEllipsis();
        }

        void Update()
        {
            if (delay < 2f)
            {
                delay += Time.deltaTime;
                return;
            }

            if (Input.anyKeyDown)
            {
                // Restart the game or go back to the main menu
                PlayerPrefs.SetInt("Score", 0); // Reset score
                UnityEngine.SceneManagement.SceneManager.LoadScene("Charlie");
            }
        }
        
        void AnimateEllipsis()
        {
            Sequence sequence = DOTween.Sequence();
            
            sequence.AppendCallback(() => uiText.text = baseText)
                    .AppendInterval(0.5f)
                    .AppendCallback(() => uiText.text = baseText + ".")
                    .AppendInterval(0.5f)
                    .AppendCallback(() => uiText.text = baseText + "..")
                    .AppendInterval(0.5f)
                    .AppendCallback(() => uiText.text = baseText + "...")
                    .AppendInterval(0.5f)
                    .SetLoops(-1);
        }
    }
}
