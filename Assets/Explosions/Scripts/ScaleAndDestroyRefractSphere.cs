using UnityEngine;

public class ScaleAndDestroyRefractSphere : MonoBehaviour
{
    public float scaleFactor = 1.2f;
    public float destroyTime = 2.0f;

    private float startTime;
    private Vector3 initialScale;

    private void Start()
    {
        startTime = Time.time;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        float elapsedTime = Time.time - startTime;
        float scale = Mathf.Lerp(1.0f, scaleFactor, elapsedTime / destroyTime);
        transform.localScale = initialScale * scale;

        if (elapsedTime >= destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
