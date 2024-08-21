using UnityEngine;

public class QualityManager : MonoBehaviour
{
    public enum Quality
    {
        Lowest = 100,
        Poor = 190,
        Low = 200,
        Medium = 210,
        High = 300,
        Highest = 500
    }

    public bool autoChoseQualityOnStart = true;
    public Quality currentQuality = Quality.Highest;

    public MobileBloom bloom;
    public HeightDepthOfField depthOfField;
    public ColoredNoise noise;
    public RenderFogPlane heightFog;
    public MonoBehaviour reflection;
    public ShaderDatabase shaders;
    public GameObject heightFogBeforeTransparentGO;

    public static Quality quality = Quality.Highest;

    private void Start()
    {
        if (heightFogBeforeTransparentGO != null)
            heightFogBeforeTransparentGO.SetActive(true);

        if (!bloom)
            bloom = GetComponent<MobileBloom>();
        if (!noise)
            noise = GetComponent<ColoredNoise>();
        if (!depthOfField)
            depthOfField = GetComponent<HeightDepthOfField>();
        if (!heightFog)
            heightFog = gameObject.GetComponentInChildren<RenderFogPlane>();
        if (!shaders)
            shaders = GetComponent<ShaderDatabase>();
        if (!reflection)
            reflection = GetComponent<ReflectionFx>() as MonoBehaviour;

        ApplyAndSetQuality(currentQuality);
    }

#if UNITY_EDITOR
    private void Update()
    {
        Quality newQuality = currentQuality;
        if (newQuality != quality)
            ApplyAndSetQuality(newQuality);
    }
#endif

    private void ApplyAndSetQuality(Quality newQuality)
    {
        quality = newQuality;

        GetComponent<Camera>().cullingMask = -1 & ~(1 << LayerMask.NameToLayer("Adventure"));
        GameObject textAdventure = GameObject.Find("TextAdventure");
        if (textAdventure != null)
            textAdventure.GetComponent<TextAdventureManager>().enabled = false;

        DisableAllFx();

        switch (quality)
        {
            case Quality.Lowest:
                if (textAdventure != null)
                    textAdventure.GetComponent<TextAdventureManager>().enabled = true;
                GetComponent<Camera>().cullingMask = 1 << LayerMask.NameToLayer("Adventure");
                break;
            case Quality.Poor:
                //Nothing to do here
                break;
            case Quality.Low:
                EnableFx(reflection, true);
                break;
            case Quality.Medium:
                EnableFx(bloom, true);
                EnableFx(reflection, true);
                break;
            case Quality.High:
                EnableFx(bloom, true);
                EnableFx(noise, true);
                EnableFx(reflection, true);
                break;
            case Quality.Highest:
                EnableFx(depthOfField, true);
                EnableFx(heightFog, true);
                EnableFx(bloom, true);
                EnableFx(reflection, true);
                EnableFx(noise, true);
                if ((heightFog != null && heightFog.enabled) || (depthOfField != null && depthOfField.enabled))
                    GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
                break;
            default:
                break;
        }
        Debug.Log("AngryBots: setting shader LOD to " + quality);

        Shader.globalMaximumLOD = (int)quality;
        foreach (Shader s in shaders.shaders)
        {
            s.maximumLOD = (int)quality;
        }
    }

    private void DisableAllFx()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
        EnableFx(reflection, false);
        EnableFx(depthOfField, false);
        EnableFx(heightFog, false);
        EnableFx(bloom, false);
        EnableFx(noise, false);
    }

    private void EnableFx(MonoBehaviour fx, bool enable)
    {
        if (fx != null)
            fx.enabled = enable;
    }
}
