using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float destroyDelay = 1f;

    void Start()
    {
        // Destroy this GameObject after 2 seconds
        Destroy(gameObject, destroyDelay);
    }
}
