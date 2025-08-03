using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK25
{
    public class LoopTimer : MonoBehaviour
    {
        private Slider _slider;
        private float _loopDuration = 5f;

        void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.value = 0f;
        }

        private void ToggleTimer(bool isLoopMode)
        {
            if (isLoopMode)
            {
                _slider.value = 1f;
                _slider.DOValue(0, _loopDuration).SetEase(Ease.Linear);
            }
            else
            {
                _slider.DOKill();
                _slider.value = 0f;
            }
        }

        void OnEnable()
        {
            EventManager.OnLoopModeToggled += ToggleTimer;
        }
        void OnDisable()
        {
            EventManager.OnLoopModeToggled -= ToggleTimer;
        }
    }
}
