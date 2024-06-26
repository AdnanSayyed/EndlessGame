using EndlessGame.Manager;
using EndlessGame.SaveLoad;
using EndlessGame.Service;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessGame.UI
{
    public class MainMenu : MonoBehaviour, IMenu
    {
        [SerializeField] Button playButton;
        [SerializeField] Button instructionButton;
        [SerializeField] Button closeInstructionButton;
        [SerializeField] Button QuitButton;

        [SerializeField] GameObject instructionsPanel;

        [SerializeField] TMPro.TextMeshProUGUI highScoreText;

        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            QuitButton.onClick.AddListener(OnQuitButtonClicked);

            instructionButton.onClick.AddListener(OnInstructionButtonClicked);
            closeInstructionButton.onClick.AddListener(OnCloseInstructionButtonClicked);

            instructionsPanel.SetActive(false);
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
            highScoreText.text = "High Score: " + data.HighScore;
        }

        private void OnInstructionButtonClicked()
        {
            instructionsPanel.SetActive(true);
        }

        private void OnCloseInstructionButtonClicked()
        {
            instructionsPanel.SetActive(false);
        }

        private void OnPlayButtonClicked()
        {
            ServiceLocator.GetService<IUIService>().HideAllMenus();
            ServiceLocator.GetService<IUIService>().ShowHUD();
            GameManager.Instance.StartGame();
        }

        private void OnQuitButtonClicked()
        {
            GameManager.Instance.QuitGame();
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
            QuitButton.onClick.RemoveListener(OnQuitButtonClicked);

            instructionButton.onClick.RemoveListener(OnInstructionButtonClicked);
            closeInstructionButton.onClick.RemoveListener(OnCloseInstructionButtonClicked);
        }
    }

}