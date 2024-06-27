using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EndlessGame.Service;
using EndlessGame.Manager;
using EndlessGame.Powerup;
using System.Collections.Generic;
using EndlessGame.Score;
using System.Collections;

namespace EndlessGame.UI
{
    public class HUD : MonoBehaviour, IHUD
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Transform powerupVisualsContent;

        private IScoreService scoreManager;
        private IPowerUpService powerUpService;

        private Dictionary<PowerUpType, Image> powerUpImages = new Dictionary<PowerUpType, Image>();

        private void Awake()
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        private void Start()
        {
            scoreManager = ServiceLocator.GetService<IScoreService>();
            powerUpService = ServiceLocator.GetService<IPowerUpService>();

            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged += UpdateScoreText;
                UpdateScoreText(scoreManager.CurrentScore);
            }

            if (powerUpService != null)
            {
                powerUpService.OnPowerUpActivated += HandlePowerUpActivated;
                powerUpService.OnPowerUpDeactivated += HandlePowerUpDeactivated;
            }

            InitializePowerUpImages();
        }

        private void OnDestroy()
        {
            pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged -= UpdateScoreText;
            }

            if (powerUpService != null)
            {
                powerUpService.OnPowerUpActivated -= HandlePowerUpActivated;
                powerUpService.OnPowerUpDeactivated -= HandlePowerUpDeactivated;
            }
        }

        private void InitializePowerUpImages()
        {
            foreach (Transform child in powerupVisualsContent)
            {
                var image = child.GetComponent<Image>();
                if (image != null)
                {
                    var powerUpType = child.GetComponent<PowerUpTypeIdentifier>().PowerUpType;
                    powerUpImages[powerUpType] = image;
                    image.gameObject.SetActive(false);
                }
            }
        }

        private void HandlePowerUpActivated(PowerUpType powerUpType)
        {
            if (powerUpImages.TryGetValue(powerUpType, out var image))
            {
                image.gameObject.SetActive(true);
                StartCoroutine(UpdatePowerUpFillAmount(powerUpType, image));
            }
        }

        private void HandlePowerUpDeactivated(PowerUpType powerUpType)
        {
            if (powerUpImages.TryGetValue(powerUpType, out var image))
            {
                image.gameObject.SetActive(false);
                StopCoroutine(UpdatePowerUpFillAmount(powerUpType, image));
            }
        }

        private IEnumerator UpdatePowerUpFillAmount(PowerUpType powerUpType, Image image)
        {
            while (powerUpService.IsPowerUpActive(powerUpType))
            {
                float remainingTime = powerUpService.GetRemainingDuration(powerUpType);
                float totalTime = powerUpService.GetPowerUpDuration(powerUpType);
                image.fillAmount = remainingTime / totalTime;
                yield return null;
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
