using UnityEngine;

public class RenderFogPlane : MonoBehaviour
{
    public Camera cameraForRay;

    private Matrix4x4 frustumCorners;
    private float CAMERA_ASPECT_RATIO = 1.333333f;
    private float CAMERA_NEAR;
    private float CAMERA_FAR;
    private float CAMERA_FOV;

    private Mesh mesh;
    private Vector2[] uv = new Vector2[4];

    private void OnEnable()
    {
        GetComponent<Renderer>().enabled = true;

        if (!mesh)
            mesh = (GetComponent<MeshFilter>() as MeshFilter).sharedMesh;

        // write indices into uv's for fast world space reconstruction

        if (mesh)
        {
            uv[0] = new Vector2(1.0f, 1.0f); // TR
            uv[1] = new Vector2(0.0f, 0.0f); // TL
            uv[2] = new Vector2(2.0f, 2.0f); // BR
            uv[3] = new Vector2(3.0f, 3.0f); // BL
            mesh.uv = uv;
        }

        if (!cameraForRay)
            cameraForRay = Camera.main;
    }

    private bool EarlyOutIfNotSupported()
    {
        if (!Supported())
        {
            enabled = false;
            return true;
        }
        return false;
    }

    private void OnDisable()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private bool Supported()
    {
        return (GetComponent<Renderer>().sharedMaterial.shader.isSupported && SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth));
    }

    private void Update()
    {
        if (EarlyOutIfNotSupported())
        {
            enabled = false;
            return;
        }
        if (!GetComponent<Renderer>().enabled)
            return;

        frustumCorners = Matrix4x4.identity;

        Ray ray;
        Vector4 vec;
        Vector3 corner;

        CAMERA_NEAR = cameraForRay.nearClipPlane;
        CAMERA_FAR = cameraForRay.farClipPlane;
        CAMERA_FOV = cameraForRay.fieldOfView;
        CAMERA_ASPECT_RATIO = cameraForRay.aspect;

        float fovWHalf = CAMERA_FOV * 0.5f;

        Vector3 toRight = cameraForRay.transform.right * CAMERA_NEAR * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * CAMERA_ASPECT_RATIO;
        Vector3 toTop = cameraForRay.transform.up * CAMERA_NEAR * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (cameraForRay.transform.forward * CAMERA_NEAR - toRight + toTop);
        float CAMERA_SCALE = topLeft.magnitude * CAMERA_FAR / CAMERA_NEAR;

        // correctly place transform first

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, CAMERA_NEAR + 0.0001f);
        transform.localScale = new Vector3((toRight * 0.5f).magnitude, 1.0f, (toTop * 0.5f).magnitude);
        transform.localRotation = Quaternion.Euler(270.0f, 0.0f, 0.0f);

        // write view frustum corner "rays"

        topLeft.Normalize();
        topLeft *= CAMERA_SCALE;

        Vector3 topRight = (cameraForRay.transform.forward * CAMERA_NEAR + toRight + toTop);
        topRight.Normalize();
        topRight *= CAMERA_SCALE;

        Vector3 bottomRight = (cameraForRay.transform.forward * CAMERA_NEAR + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= CAMERA_SCALE;

        Vector3 bottomLeft = (cameraForRay.transform.forward * CAMERA_NEAR - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= CAMERA_SCALE;

        frustumCorners.SetRow(0, new Vector4(topLeft.x, topLeft.y, topLeft.z, 0.0f));
        frustumCorners.SetRow(1, new Vector4(topRight.x, topRight.y, topRight.z, 0.0f));
        frustumCorners.SetRow(2, new Vector4(bottomRight.x, bottomRight.y, bottomRight.z, 0.0f));
        frustumCorners.SetRow(3, new Vector4(bottomLeft.x, bottomLeft.y, bottomLeft.z, 0.0f));

        GetComponent<Renderer>().sharedMaterial.SetMatrix("_FrustumCornersWS", frustumCorners);
        GetComponent<Renderer>().sharedMaterial.SetVector("_CameraWS", cameraForRay.transform.position);
    }

}