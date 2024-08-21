using UnityEngine;

public class MoodBoxManager : MonoBehaviour
{
    public static MoodBox current = null;
    public MoodBoxData currentData;

    public MobileBloom bloom;
    public ColoredNoise noise;
    public RenderFogPlane fog;

    public MoodBox startMoodBox;
    public Cubemap defaultPlayerReflection;
    public Material[] playerReflectionMaterials;
    public bool applyNearestMoodBox = false;

    public MoodBox currentMoodBox = null;

    private GameObject[] splashManagers;
    private GameObject[] rainManagers;

    public GameObject[] GetSplashManagers { get { return splashManagers; } }
    public GameObject[] GetRainManagers { get { return rainManagers; } }
    public MoodBox GetSetCurrentMoodBox { get { return current; } set { current = value; } }

    private void Awake()
    {
        splashManagers = GameObject.FindGameObjectsWithTag("RainSplashManager");
        rainManagers = GameObject.FindGameObjectsWithTag("RainBoxManager");
    }

    private void Start()
    {
        if (!bloom)
            bloom = Camera.main.gameObject.GetComponent<MobileBloom>();
        if (!noise)
            noise = Camera.main.gameObject.GetComponent<ColoredNoise>();
        if (!fog)
            fog = Camera.main.gameObject.GetComponentInChildren<RenderFogPlane>();

        current = startMoodBox;
        UpdateFromMoodBox();
    }

    private void Update()
    {
        UpdateFromMoodBox();
    }

    public MoodBoxData GetData()
    {
        return currentData;
    }

    private void UpdateFromMoodBox()
    {
#if UNITY_EDITOR
        ApplyNearestMoodBoxIfDesired();
#endif

        currentMoodBox = current;

        if (current)
        {
            if (!Application.isPlaying)
            {
                currentData.noiseAmount = current.data.noiseAmount;
                currentData.colorMixBlend = current.data.colorMixBlend;
                currentData.colorMix = current.data.colorMix;
                currentData.fogY = current.data.fogY;
                currentData.fogColor = current.data.fogColor;
                currentData.outside = current.data.outside;
            }
            else
            {
                currentData.noiseAmount = Mathf.Lerp(currentData.noiseAmount, current.data.noiseAmount, Time.deltaTime);
                currentData.colorMixBlend = Mathf.Lerp(currentData.colorMixBlend, current.data.colorMixBlend, Time.deltaTime);
                currentData.colorMix = Color.Lerp(currentData.colorMix, current.data.colorMix, Time.deltaTime);
                currentData.fogY = Mathf.Lerp(currentData.fogY, current.data.fogY, Time.deltaTime * 1.5f);
                currentData.fogColor = Color.Lerp(currentData.fogColor, current.data.fogColor, Time.deltaTime * 0.25f);
                currentData.outside = current.data.outside;
            }
        }

        if (bloom && bloom.enabled)
        {
            bloom.colorMix = currentData.colorMix;
            bloom.colorMixBlend = currentData.colorMixBlend;
        }
        if (noise && noise.enabled)
        {
            noise.localNoiseAmount = currentData.noiseAmount;
        }
        if (fog && fog.enabled)
        {
            fog.GetComponent<Renderer>().sharedMaterial.SetFloat("_Y", currentData.fogY);
            fog.GetComponent<Renderer>().sharedMaterial.SetColor("_FogColor", currentData.fogColor);
        }
    }

    private void ApplyNearestMoodBoxIfDesired()
    {
        if (applyNearestMoodBox)
        {
            MoodBox[] boxes = GetComponentsInChildren<MoodBox>();

            if (boxes != null && boxes.Length > 0)
            {
                Vector3 cameraPos = Camera.main.transform.position;
                MoodBox nearestMoodBox = boxes[0];
                float minDistance = Mathf.Infinity;

                foreach (MoodBox moodBox in boxes)
                {
                    float distance = (moodBox.transform.position - cameraPos).sqrMagnitude;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestMoodBox = moodBox;
                    }
                }

                current = nearestMoodBox;
            }
            else
            {
                Debug.Log("No MoodBox components found...");
            }

            applyNearestMoodBox = false;
        }
    }
}

