using EndlessGame.Player;
using EndlessGame.Service;
using UnityEngine;

namespace EndlessGame.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] float smoothSpeed = 0.125f;
        [SerializeField] Vector3 offset;

        private Transform player;

        private void Start()
        {
            player = ServiceLocator.GetService<IPlayerController>().GetTransform();
        }

        void LateUpdate()
        {
            Vector3 targetPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
