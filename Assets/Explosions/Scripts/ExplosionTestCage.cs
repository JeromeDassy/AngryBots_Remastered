using UnityEngine;

public class ExplosionTestCage : MonoBehaviour
{
    public GameObject explPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(explPrefab, transform.position, transform.rotation);
        }
    }
}
