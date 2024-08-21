using UnityEngine;

public class SelfIlluminationBlink : MonoBehaviour
{
    public float blink = 0.0f;

    private void OnWillRenderObject()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.sharedMaterial.SetFloat("_SelfIllumStrength", blink);
    }

    public void Blink()
    {
        blink = 1.0f - blink;
    }
}
