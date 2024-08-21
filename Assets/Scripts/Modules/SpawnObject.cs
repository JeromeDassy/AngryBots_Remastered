using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject objectToSpawn;
    public SignalSender onDestroyedSignals;

    private GameObject spawned;

    private void Start()
    {
        // Keep disabled from the beginning
        enabled = false;
    }

    // When we get a signal, spawn the objectToSpawn and store the spawned object.
    // Also enable this behaviour so the Update function will be run.
    public void OnSignal()
    {
        spawned = Spawner.Spawn(objectToSpawn, transform.position, transform.rotation);
        if (onDestroyedSignals.receivers.Length > 0)
            enabled = true;
    }

    // After the object is spawned, check each frame if it's still there.
    // Once it's not, activate the onDestroyedSignals and disable again.
    private void Update()
    {
        if (spawned == null || !spawned.activeInHierarchy)
        {
            onDestroyedSignals.SendSignals(this);
            enabled = false;
        }
    }
}
