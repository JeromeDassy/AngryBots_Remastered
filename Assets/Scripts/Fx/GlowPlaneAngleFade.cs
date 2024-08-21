using UnityEngine;

public class GlowPlaneAngleFade : MonoBehaviour
{
    public Transform cameraTransform;
    public Color glowColor = Color.grey;

    private float dot = 0.5f;

    private void Start()
    {
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        dot = 1.5f * Mathf.Clamp01(Vector3.Dot(cameraTransform.forward, -transform.up) - 0.25f);
    }

    private void OnWillRenderObject()
    {
        GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", glowColor * dot);
    }
}