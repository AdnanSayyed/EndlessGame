using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EndlessGame.Service;
using EndlessGame.Manager;
using EndlessGame.Score;

namespace EndlessGame.UI
{
    public class HUD : MonoBehaviour, IHUD
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button pauseButton;

        private IScoreService scoreManager;

        private void Awake()
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        private void Start()
        {
            scoreManager = ServiceLocator.GetService<IScoreService>();
            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged += UpdateScoreText; 
                UpdateScoreText(scoreManager.CurrentScore); 
            }
        }

        private void OnDestroy()
        {
            pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged -= UpdateScoreText;
            }
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

        private void UpdateScoreText(int score)
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
