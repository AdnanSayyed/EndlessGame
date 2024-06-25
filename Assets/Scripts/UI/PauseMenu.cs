using EndlessGame.Manager;
using EndlessGame.Service;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessGame.UI
{
    public class PauseMenu : MonoBehaviour, IMenu
    {
        [SerializeField] Button resumeButton;
        [SerializeField] Button mainMenuButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        private void OnDestroy()
        {
            resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);

        }

        public void OnResumeButtonClicked()
        {
            ServiceLocator.GetService<IUIService>().HideAllMenus();
            ServiceLocator.GetService<IUIService>().ShowHUD();
            GameManager.Instance.ResumeGame();
        }

        public void OnMainMenuButtonClicked()
        {
            var uiService = ServiceLocator.GetService<IUIService>();
            uiService.ShowMainMenu();
        }

    }

}