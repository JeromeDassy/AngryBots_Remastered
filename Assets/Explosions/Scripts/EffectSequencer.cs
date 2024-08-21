using System.Collections;
using UnityEngine;

public class EffectSequencer : MonoBehaviour
{
    [System.Serializable]
    public class ExplosionPart
    {
        public GameObject gameObject = null;
        public float delay = 0.0f;
        public bool hqOnly = false;
        public float yOffset = 0.0f;
    }

    public ExplosionPart[] ambientEmitters;
    public ExplosionPart[] explosionEmitters;
    public ExplosionPart[] smokeEmitters;

    public ExplosionPart[] miscSpecialEffects;

    private IEnumerator Start()
    {
        float maxTime = 0f;

        foreach (ExplosionPart go in ambientEmitters)
        {
            StartCoroutine(InstantiateDelayed(go));
            if (go.gameObject.GetComponent<ParticleEmitter>())
                maxTime = Mathf.Max(maxTime, go.delay + go.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
        }
        foreach (ExplosionPart go in explosionEmitters)
        {
            StartCoroutine(InstantiateDelayed(go));
            if (go.gameObject.GetComponent<ParticleEmitter>())
                maxTime = Mathf.Max(maxTime, go.delay + go.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
        }
        foreach (ExplosionPart go in smokeEmitters)
        {
            StartCoroutine(InstantiateDelayed(go));
            if (go.gameObject.GetComponent<ParticleEmitter>())
                maxTime = Mathf.Max(maxTime, go.delay + go.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
        }

        if (GetComponent<AudioSource>() && GetComponent<AudioSource>().clip)
            maxTime = Mathf.Max(maxTime, GetComponent<AudioSource>().clip.length);

        yield return null;

        foreach (ExplosionPart go in miscSpecialEffects)
        {
            StartCoroutine(InstantiateDelayed(go));
            if (go.gameObject.GetComponent<ParticleEmitter>())
                maxTime = Mathf.Max(maxTime, go.delay + go.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
        }

        Destroy(gameObject, maxTime + 0.5f);
    }

    private IEnumerator InstantiateDelayed(ExplosionPart go)
    {
        if (go.hqOnly && QualityManager.quality < QualityManager.Quality.High)
            yield break;

        yield return new WaitForSeconds(go.delay);
        Instantiate(go.gameObject, transform.position + Vector3.up * go.yOffset, transform.rotation);
    }
}
