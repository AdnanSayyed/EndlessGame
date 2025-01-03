using UnityEngine;

namespace EndlessGame.Common
{
    public class RotateObject : MonoBehaviour
    {
        public float rotationSpeed = 90f;

        void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
