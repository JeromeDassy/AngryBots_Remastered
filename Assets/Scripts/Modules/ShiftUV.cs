using UnityEngine;

public class ShiftUV : MonoBehaviour
{
    public Vector2 offsetVector;

    private void Start()
    {
    }

    public void OnSignal()
    {
        GetComponent<Renderer>().material.mainTextureOffset += offsetVector;
    }
}
