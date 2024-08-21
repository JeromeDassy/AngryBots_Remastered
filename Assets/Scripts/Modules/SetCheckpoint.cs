using UnityEngine;

public class SetCheckpoint : MonoBehaviour
{
    public Transform spawnTransform;

    private void OnTriggerEnter(Collider other)
    {
        SpawnAtCheckpoint checkpointKeeper = other.GetComponent<SpawnAtCheckpoint>();
        if (checkpointKeeper != null)
        {
            checkpointKeeper.checkpoint = spawnTransform;
        }
    }
}
