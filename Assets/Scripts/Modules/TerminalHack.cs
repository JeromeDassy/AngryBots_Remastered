using UnityEngine;

[RequireComponent(typeof(Health))]
public class TerminalHack : MonoBehaviour
{
    private Health health;
    private Animation animationComp;

    private void Start()
    {
        health = GetComponent<Health>();
        animationComp = GetComponentInChildren<Animation>();

        UpdateHackingProgress();
        enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            health.OnDamage(Time.deltaTime, Vector3.zero);
    }

    public void OnHacking()
    {
        enabled = true;
        UpdateHackingProgress();
    }

    public void OnHackingCompleted()
    {
        GetComponent<AudioSource>().Play();
        animationComp.Stop();
        enabled = false;
    }

    private void UpdateHackingProgress()
    {
        animationComp.clip.SampleAnimation(animationComp.gameObject, (1 - health.health / health.maxHealth) * animationComp.clip.length);
    }

    private void Update()
    {
        UpdateHackingProgress();

        if (health.health == 0 || health.health == health.maxHealth)
        {
            UpdateHackingProgress();
            enabled = false;
        }
    }
}
