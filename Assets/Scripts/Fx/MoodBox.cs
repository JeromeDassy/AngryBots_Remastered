using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MoodBox : MonoBehaviour
{
    public MoodBoxData data;
    public Cubemap playerReflection;

    private MoodBoxManager manager;

    private void Start()
    {
        manager = transform.parent.GetComponent<MoodBoxManager>();
        if (!manager)
        {
            Debug.Log("Disabled moodbox " + gameObject.name + " as a MoodBoxManager was not found.", transform);
            enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (transform.parent)
        {
            Gizmos.color = new Color(0.5f, 0.9f, 1.0f, 0.15f);
            Gizmos.DrawCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (transform.parent)
        {
            Gizmos.color = new Color(0.5f, 0.9f, 1.0f, 0.75f);
            Gizmos.DrawCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyMoodBox();
        }
    }

    private void ApplyMoodBox()
    {
        // Optimization: deactivate rain stuff a little earlier
        if (manager.GetData().outside != data.outside)
        {
            foreach (GameObject m in manager.GetRainManagers)
            {
                m.SetActive(data.outside);
            }
            foreach (GameObject m in manager.GetSplashManagers)
            {
                m.SetActive(data.outside);
            }
        }

        manager.GetSetCurrentMoodBox = this;

        if (manager.playerReflectionMaterials.Length > 0)
        {
            foreach (Material m in manager.playerReflectionMaterials)
            {
                m.SetTexture("_Cube", playerReflection ? playerReflection : manager.defaultPlayerReflection);
            }
        }
    }
}
