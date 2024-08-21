using UnityEngine;

public class TriggerOnTag : MonoBehaviour
{
    public string triggerTag = "Player";
    public SignalSender enterSignals;
    public SignalSender exitSignals;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (other.gameObject.tag == triggerTag || triggerTag == "")
        {
            enterSignals.SendSignals(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        if (other.gameObject.tag == triggerTag || triggerTag == "")
        {
            exitSignals.SendSignals(this);
        }
    }
}
