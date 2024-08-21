using UnityEngine;

public class SlowBulletFire : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float frequency = 2.0f;
    public float coneAngle = 1.5f;
    public AudioClip fireSound;
    public bool firing = false;

    private float lastFireTime = -1.0f;

    private void Update()
    {
        if (firing)
        {
            if (Time.time > lastFireTime + 1.0f / frequency)
            {
                Fire();
            }
        }
    }

    private void Fire()
    {
        // Spawn bullet
        Quaternion coneRandomRotation = Quaternion.Euler(Random.Range(-coneAngle, coneAngle), Random.Range(-coneAngle, coneAngle), 0);
        Spawner.Spawn(bulletPrefab, transform.position, transform.rotation * coneRandomRotation);

        if (GetComponent<AudioSource>() && fireSound)
        {
            GetComponent<AudioSource>().clip = fireSound;
            GetComponent<AudioSource>().Play();
        }

        lastFireTime = Time.time;
    }

    public void OnStartFire()
    {
        firing = true;
    }

    public void OnStopFire()
    {
        firing = false;
    }
}
