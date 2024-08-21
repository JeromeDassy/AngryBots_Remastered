using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Mobile Bloom")]
public class MobileBloom : MonoBehaviour
{
    public float intensity = 0.5f;
    public Color colorMix = Color.white;
    public float colorMixBlend = 0.25f;
    public float agonyTint = 0.0f;

    private Shader bloomShader;
    private Material apply = null;
    private RenderTextureFormat rtFormat = RenderTextureFormat.Default;

    private void Start()
    {
        FindShaders();
        CheckSupport();
        CreateMaterials();
    }

    private void FindShaders()
    {
        if (!bloomShader)
            bloomShader = Shader.Find("Hidden/MobileBloom");
    }

    private void CreateMaterials()
    {
        if (!apply)
        {
            apply = new Material(bloomShader);
            apply.hideFlags = HideFlags.DontSave;
        }
    }

    public void OnDamage()
    {
        agonyTint = 1.0f;
    }

    private bool Supported()
    {
        return (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && bloomShader.isSupported);
    }

    private bool CheckSupport()
    {
        if (!Supported())
        {
            enabled = false;
            return false;
        }
        rtFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565) ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default;
        return true;
    }

    private void OnDisable()
    {
        if (apply)
        {
            DestroyImmediate(apply);
            apply = null;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
#if UNITY_EDITOR
        FindShaders();
        CheckSupport();
        CreateMaterials();
#endif

        agonyTint = Mathf.Clamp01(agonyTint - Time.deltaTime * 2.75f);

        RenderTexture tempRtLowA = RenderTexture.GetTemporary(source.width / 4, source.height / 4, (int)rtFormat);//TODO no cast int
        RenderTexture tempRtLowB = RenderTexture.GetTemporary(source.width / 4, source.height / 4, (int)rtFormat);//TODO no cast int

        // prepare data

        apply.SetColor("_ColorMix", colorMix);
        apply.SetVector("_Parameter", new Vector4(colorMixBlend * 0.25f, 0.0f, 0.0f, 1.0f - intensity - agonyTint));

        // downsample & blur

        Graphics.Blit(source, tempRtLowA, apply, agonyTint < 0.5f ? 1 : 5);
        Graphics.Blit(tempRtLowA, tempRtLowB, apply, 2);
        Graphics.Blit(tempRtLowB, tempRtLowA, apply, 3);

        // apply

        apply.SetTexture("_Bloom", tempRtLowA);
        Graphics.Blit(source, destination, apply, QualityManager.quality > QualityManager.Quality.Medium ? 4 : 0);

        RenderTexture.ReleaseTemporary(tempRtLowA);
        RenderTexture.ReleaseTemporary(tempRtLowB);
    }
}
