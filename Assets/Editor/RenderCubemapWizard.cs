using UnityEditor;
using UnityEngine;

public class RenderCubemapWizard : ScriptableWizard
{
    public Transform renderFromPosition;
    public Cubemap cubemap;

    public Material mySkyBoxMat;

    private void OnEnable()
    {
        helpString = "Select transform to render from and cubemap to render into";
    }

    private void OnWizardUpdate()
    {
        isValid = renderFromPosition != null && cubemap != null;
    }

    private void OnWizardCreate()
    {
        // Create temporary camera for rendering
        var go = new GameObject("CubemapCamera", typeof(Camera));

        go.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        if (!go.GetComponent<Skybox>())
            go.AddComponent<Skybox>();

        go.GetComponent<Skybox>().material = mySkyBoxMat;

        // Place the camera at the specified position
        go.transform.position = renderFromPosition.position;
        if (renderFromPosition.GetComponent<Renderer>())
            go.transform.position = renderFromPosition.GetComponent<Renderer>().bounds.center;

        go.transform.rotation = Quaternion.identity;

        go.GetComponent<Camera>().fieldOfView = 90.0f;
        go.GetComponent<Camera>().aspect = 1.0f;

        // Render into cubemap
        go.GetComponent<Camera>().RenderToCubemap(cubemap);

        // Destroy temporary camera
        DestroyImmediate(go);
    }

    [MenuItem("Tools/Standard Editor Tools/Render/Render Into Cubemap", false, 4)]
    static void RenderCubemapMenuItem()
    {
        DisplayWizard<RenderCubemapWizard>("Render cubemap", "Render!");
    }
}
