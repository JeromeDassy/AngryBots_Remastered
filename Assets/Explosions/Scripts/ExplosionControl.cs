using UnityEngine;

public class ExplosionControl : MonoBehaviour
{
    public GameObject[] trails;
    public ParticleEmitter emitter;
    public LineRenderer[] lineRenderer;
    public GameObject lightDecal;

    public float autoDisableAfter = 2.0f;

    private void Awake()
    {
        for (int i = 0; i < lineRenderer.Length; i++)
        {
            float lineWidth = Random.Range(0.25f, 0.5f);

            lineRenderer[i].startWidth = lineWidth;
            lineRenderer[i].endWidth = lineWidth;

            Vector3 dir = Random.onUnitSphere;
            dir.y = Mathf.Abs(dir.y);

            lineRenderer[i].SetPosition(1, dir * Random.Range(8.0f, 12.0f));
        }
    }

    private void OnEnable()
    {
        lightDecal.transform.localScale = Vector3.one;
        lightDecal.SetActive(true);

        foreach (GameObject trail in trails)
        {
            trail.transform.localPosition = Vector3.zero;
            trail.SetActive(true);
            trail.GetComponent<ExplosionTrail>().enabled = true;
        }

        foreach (LineRenderer renderer in lineRenderer)
        {
            renderer.transform.localPosition = Vector3.zero;
            renderer.gameObject.SetActive(true);
            renderer.enabled = true;
        }

        emitter.emit = true;
        emitter.enabled = true;
        emitter.gameObject.SetActive(true);

        Invoke("DisableEmitter", emitter.maxEnergy);
        Invoke("DisableStuff", autoDisableAfter);
    }

    private void DisableEmitter()
    {
        emitter.emit = false;
        emitter.enabled = false;
    }

    private void DisableStuff()
    {
        gameObject.SetActive(false);
    }
}
