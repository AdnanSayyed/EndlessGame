using EndlessGame.ObjectPool;
using EndlessGame.Player;
using EndlessGame.Service;
using EndlessGame.Spawner;
using EndlessGame.UI;
using EndlessGame.Camera;
using UnityEngine;
using EndlessGame.Powerup;
using EndlessGame.Score;
using EndlessGame.SaveLoad;

namespace EndlessGame.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameObject objectPoolerPrefab;
        [SerializeField] private GameObject playerControllerPrefab;
        [SerializeField] private GameObject obstacleSpawnerPrefab;
        [SerializeField] private GameObject collectableSpawnerPrefab;
        [SerializeField] private GameObject platformSpawnerPrefab;
        [SerializeField] private GameObject spawnerPrefab;
        [SerializeField] private GameObject cameraFollowPrefab;
        [SerializeField] private GameObject uiServicePrefab;

        private ICameraFollow cameraService;

        private bool isGameRunning = false;
        public bool IsGameRunning => isGameRunning;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeInitialServices();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeInitialServices()
        {
            if (!ServiceLocator.IsServiceRegistered<ISaveLoadService>())
            {
                var saveLoadService = new SaveLoadService();
                ServiceLocator.RegisterService<ISaveLoadService>(saveLoadService);
            }
            if (!ServiceLocator.IsServiceRegistered<ICameraFollow>())
            {
                cameraService = Instantiate(cameraFollowPrefab).GetComponent<ICameraFollow>();
                ServiceLocator.RegisterService<ICameraFollow>(cameraService);
            }

            var uiService = Instantiate(uiServicePrefab).GetComponent<IUIService>();
            ServiceLocator.RegisterService<IUIService>(uiService);
            uiService.ShowMainMenu();
        }

        private void InitializeGameplayServices()
        {
            if (!ServiceLocator.IsServiceRegistered<IInputService>())
            {
                var inputService = new InputService();
                ServiceLocator.RegisterService<IInputService>(inputService);
            }

            if (!ServiceLocator.IsServiceRegistered<IScoreService>())
            {
                var scoreManager = new ScoreService(); // Create a new instance of ScoreManager
                ServiceLocator.RegisterService<IScoreService>(scoreManager);
            }

            if (!ServiceLocator.IsServiceRegistered<IPowerUpService>())
            {
                var powerUpService = new PowerUpService();
                ServiceLocator.RegisterService<IPowerUpService>(powerUpService);
            }

            if (!ServiceLocator.IsServiceRegistered<IObjectPooler>())
            {
                var objectPooler = Instantiate(objectPoolerPrefab).GetComponent<IObjectPooler>();
                ServiceLocator.RegisterService<IObjectPooler>(objectPooler);
            }

            if (!ServiceLocator.IsServiceRegistered<IPlayerController>())
            {
                var playerController = Instantiate(playerControllerPrefab).GetComponent<IPlayerController>();
                ServiceLocator.RegisterService<IPlayerController>(playerController);
            }

            if (!ServiceLocator.IsServiceRegistered<IObstacleSpawner>())
            {
                var obstacleSpawner = Instantiate(obstacleSpawnerPrefab).GetComponent<IObstacleSpawner>();
                ServiceLocator.RegisterService<IObstacleSpawner>(obstacleSpawner);
            }

            if (!ServiceLocator.IsServiceRegistered<ICollectableSpawner>())
            {
                var collectableSpawner = Instantiate(collectableSpawnerPrefab).GetComponent<ICollectableSpawner>();
                ServiceLocator.RegisterService<ICollectableSpawner>(collectableSpawner);
            }

            if (!ServiceLocator.IsServiceRegistered<IPlatformSpawner>())
            {
                var platformSpawner = Instantiate(platformSpawnerPrefab).GetComponent<IPlatformSpawner>();
                ServiceLocator.RegisterService<IPlatformSpawner>(platformSpawner);
                platformSpawner.Initialize();
            }

            if (!ServiceLocator.IsServiceRegistered<ISpawner>())
            {
                var spawner = Instantiate(spawnerPrefab).GetComponent<ISpawner>();
                ServiceLocator.RegisterService<ISpawner>(spawner);
                spawner.Initialize();
            }

            cameraService.Initialize();

        }

        private void ResetGameplayServices()
        {
            var objectPooler = ServiceLocator.GetService<IObjectPooler>();
            var platformSpawner = ServiceLocator.GetService<IPlatformSpawner>();
            var collectableSpawner = ServiceLocator.GetService<ICollectableSpawner>();
            var obstacleSpawner = ServiceLocator.GetService<IObstacleSpawner>();
            var playerController = ServiceLocator.GetService<IPlayerController>();
            var spawner = ServiceLocator.GetService<ISpawner>();

            platformSpawner?.ResetService(objectPooler);
            collectableSpawner?.ResetService(objectPooler);
            obstacleSpawner?.ResetService(objectPooler);
            playerController?.ResetService();
            spawner?.ResetService();
        }


        private void Update()
        {
            if (isGameRunning)
            {
                var spawner = ServiceLocator.GetService<ISpawner>();
                spawner.UpdateSpawner();
            }
        }

        public void StartGame()
        {
            Time.timeScale = 1;

            InitializeGameplayServices();
            ResetGameplayServices();

            isGameRunning = true;
            var uiService = ServiceLocator.GetService<IUIService>();
            uiService.ShowHUD();
            Debug.Log("Game Started");
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            var uiService = ServiceLocator.GetService<IUIService>();
            uiService.ShowPauseMenu();
            Debug.Log("Game Paused");
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            var uiService = ServiceLocator.GetService<IUIService>();
            uiService.ShowHUD();
            Debug.Log("Game Resumed");
        }

        public void EndGame()
        {
            if (isGameRunning)
            {
                // Save high score
                var scoreManager = ServiceLocator.GetService<IScoreService>();
                var saveLoadService = ServiceLocator.GetService<ISaveLoadService>();

                int currentScore = scoreManager.CurrentScore;
                SaveData data = saveLoadService.Load();

                if (currentScore > data.HighScore)
                {
                    data.HighScore = currentScore;
                    saveLoadService.Save(data);
                    Debug.Log("New high score saved: " + currentScore);
                }

                isGameRunning = false;
                var uiService = ServiceLocator.GetService<IUIService>();
                uiService.ShowGameOverMenu();
                //ResetGameplayServices();

                Debug.Log("Game Ended");
            }
        }

        public void QuitGame()
        {
            Application.Quit();
            Debug.Log("Game Quit");
        }
    }
}
