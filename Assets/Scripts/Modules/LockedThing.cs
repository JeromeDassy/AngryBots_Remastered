using UnityEngine;

public class LockedThing : MonoBehaviour
{
    public Lock[] locks;
    public SignalSender conditionalSignal;

    private void OnSignal()
    {
        bool locked = false;
        foreach (Lock lockObj in locks)
        {
            if (lockObj.locked)
            {
                locked = true;
                break;
            }
        }

        if (!locked)
        {
            conditionalSignal.SendSignals(this);
        }
    }
}
