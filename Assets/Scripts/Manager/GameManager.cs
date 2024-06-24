using EndlessGame.ObjectPool;
using EndlessGame.Player;
using EndlessGame.Service;
using EndlessGame.Spawner;
using UnityEngine;

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

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            InitializeServices();
        }

        private void InitializeServices()
        {
            var inputService = new InputService();
            ServiceLocator.RegisterService<IInputService>(inputService);

            var objectPooler = Instantiate(objectPoolerPrefab).GetComponent<IObjectPooler>();
            ServiceLocator.RegisterService<IObjectPooler>(objectPooler);

            var playerController = Instantiate(playerControllerPrefab).GetComponent<IPlayerController>();
            ServiceLocator.RegisterService<IPlayerController>(playerController);

            var obstacleSpawner = Instantiate(obstacleSpawnerPrefab).GetComponent<IObstacleSpawner>();
            ServiceLocator.RegisterService<IObstacleSpawner>(obstacleSpawner);

            var collectableSpawner = Instantiate(collectableSpawnerPrefab).GetComponent<ICollectableSpawner>();
            ServiceLocator.RegisterService<ICollectableSpawner>(collectableSpawner);

            var platformSpawner = Instantiate(platformSpawnerPrefab).GetComponent<IPlatformSpawner>();
            ServiceLocator.RegisterService<IPlatformSpawner>(platformSpawner);
            platformSpawner.Initialize();

            var spawner = Instantiate(spawnerPrefab).GetComponent<ISpawner>();
            ServiceLocator.RegisterService<ISpawner>(spawner);
            spawner.Initialize();

        }

        private void Update()
        {
            var spawner = ServiceLocator.GetService<ISpawner>();
            spawner.UpdateSpawner();
        }

        public void StartGame()
        {
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}