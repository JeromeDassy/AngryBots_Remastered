using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 0.5f;
    public float dist = 10000f;
    private float spawnTime = 0f;
    private Transform tr;

    private void OnEnable()
    {
        tr = transform;
        spawnTime = Time.time;
    }

    private void Update()
    {
        tr.position += tr.forward * speed * Time.deltaTime;
        dist -= speed * Time.deltaTime;
        if (Time.time > spawnTime + lifeTime || dist < 0f)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
}