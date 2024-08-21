using UnityEngine;

public class conveyorBelt : MonoBehaviour
{
    public float scrollSpeed = 0.1f;
    public Material mat;

    private void Start()
    {
        enabled = false;
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }

    private void OnBecameInvisible()
    {
        enabled = false;
    }

    private void Update()
    {
        float offset = (Time.time * scrollSpeed) % 1.0f;

        mat.SetTextureOffset("_MainTex", new Vector2(0, -offset));
        mat.SetTextureOffset("_BumpMap", new Vector2(0, -offset));
    }
}
