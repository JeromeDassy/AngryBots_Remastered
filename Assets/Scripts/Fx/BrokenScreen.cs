using UnityEngine;

public class BrokenScreen : MonoBehaviour
{
    private static Material brokenMaterial = null;

    private void Start()
    {
        if (brokenMaterial == null)
            brokenMaterial = new Material(GetComponent<Renderer>().sharedMaterial);

        GetComponent<Renderer>().material = brokenMaterial;
    }

    private void OnWillRenderObject()
    {
        brokenMaterial.mainTextureOffset += new Vector2(Random.Range(1.0f, 2.0f), 0);
    }
}
