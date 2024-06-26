using EndlessGame.Service;
using EndlessGame.UI;
using UnityEngine;

namespace EndlessGame.UI
{
    public class UIService : MonoBehaviour, IUIService
    {
        [SerializeField] private GameObject mainMenuPrefab;
        [SerializeField] private GameObject pauseMenuPrefab;
        [SerializeField] private GameObject gameOverMenuPrefab;
        [SerializeField] private GameObject hudPrefab;

        private IMenu mainMenu;
        private IMenu pauseMenu;
        private IMenu gameOverMenu;
        private IHUD hud;

        private void Awake()
        {
            InstantiateMenus();
            ServiceLocator.RegisterService<IUIService>(this);
            HideAllMenus();
        }

        private void InstantiateMenus()
        {
            var canvas = FindObjectOfType<Canvas>();

            mainMenu = Instantiate(mainMenuPrefab, canvas.transform).GetComponent<IMenu>();
            pauseMenu = Instantiate(pauseMenuPrefab, canvas.transform).GetComponent<IMenu>();
            gameOverMenu = Instantiate(gameOverMenuPrefab, canvas.transform).GetComponent<IMenu>();
            hud = Instantiate(hudPrefab, canvas.transform).GetComponent<IHUD>();
        }

        public void ShowMainMenu()
        {
            HideAllMenus();
            mainMenu?.Show();
        }

        public void ShowPauseMenu()
        {
            HideAllMenus();
            pauseMenu?.Show();
        }

        public void ShowGameOverMenu()
        {
            HideAllMenus();
            gameOverMenu?.Show();
        }

        public void ShowHUD()
        {
            HideAllMenus();
            hud?.Show();
        }

        public void HideAllMenus()
        {
            mainMenu?.Hide();
            pauseMenu?.Hide();
            gameOverMenu?.Hide();
            hud?.Hide();
        }
    }

    public interface IUIService
    {
        void ShowMainMenu();
        void ShowPauseMenu();
        void ShowGameOverMenu();
        void ShowHUD();
        void HideAllMenus();
    }

}