using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    public float speed = 4.0f;

    private void OnEnable()
    {
        enabled = true;
    }

    private void OnDisable()
    {
        enabled = false;
    }

    private void Update()
    {
        transform.Rotate(0.0f, 0.0f, Time.deltaTime * speed);
    }
}
