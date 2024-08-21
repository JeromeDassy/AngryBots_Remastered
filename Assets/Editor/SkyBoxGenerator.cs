using UnityEditor;
using UnityEngine;

class SkyBoxGenerator : ScriptableWizard
{
    public Transform renderFromPosition;

    private string[] skyBoxImage = { "frontImage", "rightImage", "backImage", "leftImage", "upImage", "downImage" };
    private Vector3[] skyDirection = { Vector3.zero, new Vector3(0, -90, 0), new Vector3(0, 180, 0), new Vector3(0, 90, 0), new Vector3(-90, 0, 0), new Vector3(90, 0, 0) };

    private void OnEnable()
    {
        helpString = "Select transform to render from";
    }

    private void OnWizardUpdate()
    {
        isValid = (renderFromPosition != null);
    }

    private void OnWizardCreate()
    {
        GameObject go = new GameObject("SkyboxCamera", typeof(Camera));

        go.GetComponent<Camera>().backgroundColor = Camera.main.backgroundColor;
        go.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        go.GetComponent<Camera>().fieldOfView = 90;
        go.GetComponent<Camera>().aspect = 1.0f;

        go.transform.position = renderFromPosition.position;

        if (renderFromPosition.GetComponent<Renderer>())
        {
            go.transform.position = renderFromPosition.GetComponent<Renderer>().bounds.center;
        }

        go.transform.rotation = Quaternion.identity;

        for (int orientation = 0; orientation < skyDirection.Length; orientation++)
        {
            RenderSkyImage(orientation, go);
        }

        DestroyImmediate(go);
    }

    [MenuItem("Tools/Standard Editor Tools/Render/Render Into Skybox (Unity Pro Only)", false, 4)]
    static void RenderSkyBoxMenuItem()
    {
        DisplayWizard<SkyBoxGenerator>("Render SkyBox", "Render!");
    }

    private void RenderSkyImage(int orientation, GameObject go)
    {
        go.transform.eulerAngles = skyDirection[orientation];
        int screenSize = 512;
        RenderTexture rt = new RenderTexture(screenSize, screenSize, 24);
        go.GetComponent<Camera>().targetTexture = rt;
        Texture2D screenShot = new Texture2D(screenSize, screenSize, TextureFormat.RGB24, false);
        go.GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, screenSize, screenSize), 0, 0);

        RenderTexture.active = null;
        DestroyImmediate(rt);
        byte[] bytes = screenShot.EncodeToPNG();

        string directory = "Assets/Skyboxes";
        if (!System.IO.Directory.Exists(directory))
            System.IO.Directory.CreateDirectory(directory);
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(directory, skyBoxImage[orientation] + ".png"), bytes);
    }
}
