using UnityEngine;

public class GlowChange : MonoBehaviour
{
    public int signalsNeeded = 1;

    public void OnSignal()
    {
        signalsNeeded--;
        if (signalsNeeded == 0)
        {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.SetColor("_TintColor", new Color(0.29f, 0.64f, 0.15f, 0.5f));
            enabled = false;
        }
    }
}
