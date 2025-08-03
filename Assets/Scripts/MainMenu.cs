using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GMTK25
{
    public class MainMenu : MonoBehaviour
    {
        private float delay = 0f;

        [SerializeField] private TextMeshProUGUI uiText;
        private string baseText = "Press any key to play";

        void Start()
        {
            AnimateEllipsis();
        }

        private void AnimateEllipsis()
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
    }
}
