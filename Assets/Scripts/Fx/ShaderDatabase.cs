using System.Collections;
using UnityEngine;

public class ShaderDatabase : MonoBehaviour
{
    public Shader[] shaders;
    public bool cookShadersOnMobiles = true;
    public Material cookShadersCover;
    private GameObject cookShadersObject;

    private void Awake()
    {
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_WP_8_1 || UNITY_BLACKBERRY || UNITY_TIZEN
        Screen.sleepTimeout = 0.0f;

        if (!cookShadersOnMobiles)
            return;

        if (!cookShadersCover.HasProperty("_TintColor"))
            Debug.LogWarning("Dualstick: the CookShadersCover material needs a _TintColor property to properly hide the cooking process", transform);

        CreateCameraCoverPlane();
        cookShadersCover.SetColor("_TintColor", new Color(0.0f, 0.0f, 0.0f, 1.0f));
#endif
    }

    private GameObject CreateCameraCoverPlane()
    {
        cookShadersObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cookShadersObject.GetComponent<Renderer>().material = cookShadersCover;
        cookShadersObject.transform.parent = transform;
        cookShadersObject.transform.localPosition = Vector3.zero;
        cookShadersObject.transform.localPosition += new Vector3(0, 0, 1.55f);
        cookShadersObject.transform.localRotation = Quaternion.identity;
        cookShadersObject.transform.localEulerAngles += new Vector3(0, 0, 180);
        cookShadersObject.transform.localScale = Vector3.one * 1.5f;
        cookShadersObject.transform.localScale += new Vector3(0.6f, 0, 0);

        return cookShadersObject;
    }

    public void WhiteOut()
    {
        StartCoroutine(WhiteOutCoroutine());
    }

    private IEnumerator WhiteOutCoroutine()
    {
        CreateCameraCoverPlane();
        Material mat = cookShadersObject.GetComponent<Renderer>().sharedMaterial;
        mat.SetColor("_TintColor", new Color(1.0f, 1.0f, 1.0f, 0.0f));

        yield return null;

        Color c = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        while (c.a < 1.0f)
        {
            c.a += Time.deltaTime * 0.25f;
            mat.SetColor("_TintColor", c);
            yield return null;
        }

        DestroyCameraCoverPlane();
    }

    public void WhiteIn()
    {
        StartCoroutine(WhiteInCoroutine());
    }

    private IEnumerator WhiteInCoroutine()
    {
        CreateCameraCoverPlane();
        Material mat = cookShadersObject.GetComponent<Renderer>().sharedMaterial;
        mat.SetColor("_TintColor", new Color(1.0f, 1.0f, 1.0f, 1.0f));

        yield return null;

        Color c = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        while (c.a > 0.0f)
        {
            c.a -= Time.deltaTime * 0.25f;
            mat.SetColor("_TintColor", c);
            yield return null;
        }

        DestroyCameraCoverPlane();
    }

    private void DestroyCameraCoverPlane()
    {
        if (cookShadersObject)
            DestroyImmediate(cookShadersObject);
        cookShadersObject = null;
    }

    private void Start()
    {
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_WP_8_1 || UNITY_BLACKBERRY || UNITYTIZEN
        if (cookShadersOnMobiles)
            StartCoroutine(CookShaders());
#endif
    }

    private IEnumerator CookShaders()
    {
        if (shaders.Length > 0)
        {
            Material m = new Material(shaders[0]);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            cube.transform.parent = transform;
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localPosition += new Vector3(0, 0, 4.0f);

            yield return null;

            foreach (Shader s in shaders)
            {
                if (s != null)
                {
                    m.shader = s;
                    cube.GetComponent<Renderer>().material = m;
                }
                yield return null;
            }

            Destroy(m);
            Destroy(cube);

            yield return null;
            Color c = Color.black;
            c.a = 1.0f;
            while (c.a > 0.0f)
            {
                c.a -= Time.deltaTime * 0.5f;
                cookShadersCover.SetColor("_TintColor", c);
                yield return null;
            }
        }

        DestroyCameraCoverPlane();
    }
}
