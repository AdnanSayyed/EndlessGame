using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EndlessGame.Service;
using EndlessGame.Manager;

namespace EndlessGame.UI
{
    public class HUD : MonoBehaviour, IHUD
    {
        [SerializeField] TextMeshProUGUI scoreText;

        [SerializeField] Button pauseButton;

        private void Awake()
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }
        private void OnDestroy()
        {
            pauseButton.onClick.RemoveListener(OnPauseButtonClicked);

        }
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }

        private void OnPauseButtonClicked()
        {
            var uiService = ServiceLocator.GetService<IUIService>();
            uiService.ShowPauseMenu();
            GameManager.Instance.PauseGame();
        }
    }


    public interface IHUD : IMenu
    {
        void UpdateScore(int score);
    }

}