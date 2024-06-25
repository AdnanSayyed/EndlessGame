using EndlessGame.Player;
using EndlessGame.Service;
using UnityEngine;

namespace EndlessGame.Camera
{
    public class CameraFollow : MonoBehaviour, ICameraFollow
    {
        [SerializeField] private float smoothSpeed = 0.125f;
        [SerializeField] private Vector3 offset;

        private Transform player;

        public void Initialize()
        {
            player = ServiceLocator.GetService<IPlayerController>().GetTransform();
        }

        private void Start()
        {
            ServiceLocator.RegisterService<ICameraFollow>(this);
        }

        private void LateUpdate()
        {
            if (player != null)
            {
                Vector3 targetPosition = player.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }
        }
    }
    public interface ICameraFollow
    {
        void Initialize();
    }
}
