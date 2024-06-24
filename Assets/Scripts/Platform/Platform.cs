using UnityEngine;

namespace EndlessGame.Spawnable
{
    public class Platform : SpawnableBase
    {
        public float Length { get; private set; }

        private void Awake()
        {
            Length = CalculateLength();

        }

        private float CalculateLength()
        {
            float width = 0f;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer childRenderer in renderers)
            {
                width += childRenderer.bounds.size.x;
            }
            return width;
        }
    }

}