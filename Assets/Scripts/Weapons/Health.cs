using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100.0f;
    public float health = 100.0f;
    public float regenerateSpeed = 0.0f;
    public bool invincible = false;
    public bool dead = false;

    public GameObject damagePrefab;
    public Transform damageEffectTransform;
    public float damageEffectMultiplier = 1.0f;
    public bool damageEffectCentered = true;

    public GameObject scorchMarkPrefab = null;
    private GameObject scorchMark = null;

    public SignalSender damageSignals;
    public SignalSender dieSignals;

    private float lastDamageTime = 0;
    private ParticleSystem damageEffect;
    private float damageEffectCenterYOffset;

    private float colliderRadiusHeuristic = 1.0f;

    private void Awake()
    {
        enabled = false;
        if (damagePrefab)
        {
            if (damageEffectTransform == null)
                damageEffectTransform = transform;
            GameObject effect = Instantiate(damagePrefab, Vector3.zero, Quaternion.identity);
            effect.transform.parent = damageEffectTransform;
            effect.transform.localPosition = Vector3.zero;
            damageEffect = effect.GetComponent<ParticleSystem>();
            Vector3 tempSize = GetComponent<Collider>().bounds.extents;
            colliderRadiusHeuristic = Mathf.Max(tempSize.x, tempSize.z) * 0.5f;
            damageEffectCenterYOffset = GetComponent<Collider>().bounds.extents.y;
        }
        if (scorchMarkPrefab)
        {
            scorchMark = GameObject.Instantiate(scorchMarkPrefab, Vector3.zero, Quaternion.identity);
            scorchMark.SetActive(false);
        }
    }

    public void OnDamage(float amount, Vector3 fromDirection)
    {
        // Take no damage if invincible, dead, or if the damage is zero
        if (invincible)
            return;
        if (dead)
            return;
        if (amount <= 0)
            return;

        health -= amount;
        damageSignals.SendSignals(this);
        lastDamageTime = Time.time;

        // Enable so the Update function will be called
        // if regeneration is enabled
        if (regenerateSpeed > 0)
            enabled = true;

        // Show damage effect if there is one
        if (damageEffect)
        {
            damageEffect.transform.rotation = Quaternion.LookRotation(fromDirection, Vector3.up);
            if (!damageEffectCentered)
            {
                Vector3 dir = fromDirection;
                dir.y = 0.0f;
                damageEffect.transform.position = (transform.position + Vector3.up * damageEffectCenterYOffset) + colliderRadiusHeuristic * dir;
            }

            damageEffect.Emit(1);
        }

        // Die if no health left
        if (health <= 0)
        {
            GameScore.RegisterDeath(gameObject);

            health = 0;
            dead = true;
            dieSignals.SendSignals(this);
            enabled = false;

            // scorch marks
            if (scorchMark)
            {
                scorchMark.SetActive(true);

                Vector3 scorchPosition = GetComponent<Collider>().ClosestPointOnBounds(transform.position - Vector3.up * 100);
                scorchMark.transform.position = scorchPosition + Vector3.up * 0.1f;
                scorchMark.transform.eulerAngles = new Vector3(0f, Random.Range(0.0f, 90.0f), 0f);
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(Regenerate());
    }

    private System.Collections.IEnumerator Regenerate()
    {
        if (regenerateSpeed > 0.0f)
        {
            while (enabled)
            {
                if (Time.time > lastDamageTime + 3)
                {
                    health += regenerateSpeed;

                    yield return null;

                    if (health >= maxHealth)
                    {
                        health = maxHealth;
                        enabled = false;
                    }
                }
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
