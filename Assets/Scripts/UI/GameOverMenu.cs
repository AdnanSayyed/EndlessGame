using EndlessGame.Manager;
using EndlessGame.SaveLoad;
using EndlessGame.Service;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessGame.UI
{
    public class GameOverMenu : MonoBehaviour, IMenu
    {
        [SerializeField] Button retryButton;
        [SerializeField] Button mainMenuButton;
        [SerializeField] TMPro.TextMeshProUGUI highScoreText;

        private void Awake()
        {
            retryButton.onClick.AddListener(OnRetryButtonClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        private void OnDestroy()
        {
            retryButton.onClick.RemoveListener(OnRetryButtonClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            var saveLoadService = ServiceLocator.GetService<ISaveLoadService>();
            SaveData data = saveLoadService.Load();
            highScoreText.text ="High Score: " + data.HighScore;
        }

        public void OnRetryButtonClicked()
        {
            GameManager.Instance.StartGame();
        }

        public void OnMainMenuButtonClicked()
        {
            var uiService = ServiceLocator.GetService<IUIService>();
            uiService.ShowMainMenu();
        }
    }

}