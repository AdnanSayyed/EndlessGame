using UnityEngine;

public class SpawnableBase : MonoBehaviour
{
    [SerializeField]
    private string spawnableTag;

    public string SpawnableTag => spawnableTag;


    private void Reset()
    {
        spawnableTag = gameObject.name;
    }

}