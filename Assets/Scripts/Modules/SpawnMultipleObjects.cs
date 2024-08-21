using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SignalSender))]
public class SpawnMultipleObjects : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float delayInBetween = 0;
    public SignalSender onDestroyedSignals;

    private List<GameObject> spawned = new List<GameObject>();

    // Keep disabled from the beginning
    private void Start()
    {
        enabled = false;
    }

    // When we get a signal, spawn the objectToSpawn objects and store them.
    // Also enable this behaviour so the Update function will be run.
    public void OnSignal()
    {
        foreach (Transform child in transform)
        {
            // Spawn with the position and rotation of the child transform
            GameObject spawnedObject = Spawner.Spawn(objectToSpawn, child.position, child.rotation);
            spawned.Add(spawnedObject);

            // Delay
            StartCoroutine(DelaySpawn());
        }
        enabled = true;
    }

    private IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(delayInBetween);
    }

    // After the objects are spawned, check each frame if they're still there.
    // Once they're not,
    private void Update()
    {
        // Once the list is empty, activate the onDestroyedSignals and disable again.
        if (spawned.Count == 0)
        {
            onDestroyedSignals.SendSignals(this);
            enabled = false;
        }
        // As long as the list is not empty, check if the first object in the list
        // has been destroyed, and remove it from the list if it has.
        // We don't need to check the rest of the list. All of the entries will
        // end up being the first one eventually.
        // Note that only one object can be removed per frame, so if there's
        // a really high amount, there may be a slight delay before the list is empty.
        else if (spawned[0] == null || !spawned[0].activeInHierarchy)
        {
            spawned.RemoveAt(0);
        }
    }
}
