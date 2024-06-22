using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Length {  get; private set; }

    public string PlatformTag;

    private void Awake()
    {
        Length = CalculateLength();

    }
   

    private float CalculateLength()
    {
        float width = 0f;
        foreach (Renderer childRenderer in GetComponentsInChildren<Renderer>())
        {
            width += childRenderer.bounds.size.x;
        }
        return width;
    }
}
