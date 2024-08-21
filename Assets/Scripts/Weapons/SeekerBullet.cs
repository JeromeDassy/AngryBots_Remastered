﻿using UnityEngine;

public class SeekerBullet : MonoBehaviour
{
    public float speed = 15.0f;
    public float lifeTime = 1.5f;
    public float damageAmount = 5f;
    public float forceAmount = 5f;
    public float radius = 1.0f;
    public float seekPrecision = 1.3f;
    public LayerMask ignoreLayers;
    public float noise = 0.0f;
    public GameObject explosionPrefab;

    private Vector3 dir;
    private float spawnTime;
    private GameObject targetObject;
    private Transform tr;
    private float sideBias;

    private void OnEnable()
    {
        tr = transform;
        dir = transform.forward;
        targetObject = GameObject.FindWithTag("Player");
        spawnTime = Time.time;
        sideBias = Mathf.Sin(Time.time * 5);
    }

    private void Update()
    {
        if (Time.time > spawnTime + lifeTime)
        {
            Spawner.Destroy(gameObject);
            return;
        }

        if (targetObject)
        {
            Vector3 targetPos = targetObject.transform.position;
            targetPos += transform.right * (Mathf.PingPong(Time.time, 1.0f) - 0.5f) * noise;
            Vector3 targetDir = (targetPos - tr.position);
            float targetDist = targetDir.magnitude;
            targetDir /= targetDist;

            if (Time.time - spawnTime < lifeTime * 0.2f && targetDist > 3f)
                targetDir += transform.right * 0.5f * sideBias;

            dir = Vector3.Slerp(dir, targetDir, Time.deltaTime * seekPrecision);

            tr.rotation = Quaternion.LookRotation(dir);
            tr.position += dir * speed * Time.deltaTime;
        }

        Collider[] hits = Physics.OverlapSphere(tr.position, radius, ~ignoreLayers.value);
        bool collided = false;

        foreach (Collider c in hits)
        {
            if (c.isTrigger)
                continue;

            Health targetHealth = c.GetComponent<Health>();

            if (targetHealth)
            {
                targetHealth.OnDamage(damageAmount, -tr.forward);
            }

            if (c.GetComponent<Rigidbody>())
            {
                Vector3 force = tr.forward * forceAmount;
                force.y = 0f;
                c.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            }

            collided = true;
        }

        if (collided)
        {
            Spawner.Destroy(gameObject);
            Spawner.Spawn(explosionPrefab, transform.position, transform.rotation);
        }
    }
}
