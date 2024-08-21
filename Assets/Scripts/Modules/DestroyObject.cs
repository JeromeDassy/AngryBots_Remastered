using System.Collections;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public GameObject objectToDestroy;

    public void OnSignal()
    {
        Destroy(objectToDestroy);
    }
}
