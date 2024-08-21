using UnityEngine;
using System.Collections;

public class AutoFire : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float frequency = 10f;
    public float coneAngle = 1.5f;
    public bool firing = false;
    public float damagePerSecond = 20f;
    public float forcePerSecond = 20f;
    public float hitSoundVolume = 0.5f;
    public AudioSource asource;
    public GameObject muzzleFlashFront;

    private float lastFireTime = -1f;
    private PerFrameRaycast raycast;

    private IEnumerator CheckForController()
    {
#if UNITY_IPHONE
    yield return new WaitForSeconds(0.5f);

    GameObject JL;
    GameObject JR;

    while (true)
    {
        JL = GameObject.Find("Joystick Left");
        JR = GameObject.Find("Joystick Right");

        if (JL && JR)
        {
            string[] sticks = Input.GetJoystickNames();

            if (sticks.Length > 0)
            {
                JL.SetActive(false);
                JR.SetActive(false);
                GLOBAL.isJSConnected = true;
                GLOBAL.isJSExtended = !sticks[0].StartsWith("[basic,");
            }
            else
            {
                JL.SetActive(true);
                JR.SetActive(true);
                GLOBAL.isJSConnected = false;
            }
        }

        yield return new WaitForSeconds(1f);
    }
#endif

        yield return new WaitForSeconds(0.5f);
    }

    private void Awake()
    {
        StartCoroutine(CheckForController());
        asource = GetComponent<AudioSource>();
        muzzleFlashFront.SetActive(false);

        raycast = GetComponent<PerFrameRaycast>();
        if (spawnPoint == null)
            spawnPoint = transform;
    }

    private void Update()
    {
        if (GLOBAL.isJSConnected)
        {
            float axisX = Input.GetAxis("RightHorizontal");
            float axisY = Input.GetAxis("RightVertical");

            if (Input.GetButtonDown("Joystick button 15") || (!firing && GLOBAL.isJSExtended && Mathf.Abs(axisX) + Mathf.Abs(axisY) > 0f))
            {
                if (Time.timeScale == 0f)
                    return;

                firing = true;

                muzzleFlashFront.SetActive(true);

                if (asource)
                    asource.Play();
            }

            if (Input.GetButtonUp("Joystick button 15") || (firing && GLOBAL.isJSExtended && Mathf.Abs(axisX) + Mathf.Abs(axisY) < 0.01f))
            {
                firing = false;

                muzzleFlashFront.SetActive(false);

                if (asource)
                    asource.Stop();
            }
        }

        if (firing)
        {
            if (Time.time > lastFireTime + 1f / frequency)
            {
                // Spawn visual bullet
                Quaternion coneRandomRotation = Quaternion.Euler(Random.Range(-coneAngle, coneAngle), Random.Range(-coneAngle, coneAngle), 0f);
                GameObject go = Spawner.Spawn(bulletPrefab, spawnPoint.position, spawnPoint.rotation * coneRandomRotation) as GameObject;
                SimpleBullet bullet = go.GetComponent<SimpleBullet>();

                lastFireTime = Time.time;

                // Find the object hit by the raycast
                RaycastHit hitInfo = raycast.GetHitInfo();
                if (hitInfo.transform != null)
                {
                    // Get the health component of the target if any
                    Health targetHealth = hitInfo.transform.GetComponent<Health>();
                    if (targetHealth != null)
                    {
                        // Apply damage
                        targetHealth.OnDamage(damagePerSecond / frequency, -spawnPoint.forward);
                    }

                    // Get the rigidbody if any
                    if (hitInfo.rigidbody != null)
                    {
                        // Apply force to the target object at the position of the hit point
                        Vector3 force = transform.forward * (forcePerSecond / frequency);
                        hitInfo.rigidbody.AddForceAtPosition(force, hitInfo.point, ForceMode.Impulse);
                    }

                    // Ricochet sound
                    AudioClip sound = MaterialImpactManager.GetBulletHitSound(hitInfo.collider.sharedMaterial);
                    AudioSource.PlayClipAtPoint(sound, hitInfo.point, hitSoundVolume);

                    bullet.dist = hitInfo.distance;
                }
                else
                {
                    bullet.dist = 50f;
                }
            }
        }
    }

    public void OnStartFire()
    {
        if (Time.timeScale == 0f)
            return;

        firing = true;

        muzzleFlashFront.SetActive(true);

        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().Play();
    }

    public void OnStopFire()
    {
        firing = false;

        muzzleFlashFront.SetActive(false);

        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().Stop();
    }
}