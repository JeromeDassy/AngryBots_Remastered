using System.Collections.Generic;
using UnityEngine;

public class TriggerOnPresence : MonoBehaviour
{
    public SignalSender enterSignals;
    public SignalSender exitSignals;

    [SerializeField] private List<GameObject> objects;

    private void Awake()
    {
        objects = new List<GameObject>();
        enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        bool wasEmpty = (objects.Count == 0);

        objects.Add(other.gameObject);

        if (wasEmpty)
        {
            enterSignals.SendSignals(this);
            enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        if (objects.Contains(other.gameObject))
            objects.Remove(other.gameObject);

        if (objects.Count == 0)
        {
            exitSignals.SendSignals(this);
            enabled = false;
        }
    }
}
