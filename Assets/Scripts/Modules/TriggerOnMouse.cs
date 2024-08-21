using UnityEngine;

public class TriggerOnMouse : MonoBehaviour
{
    public SignalSender mouseDownSignals;
    public SignalSender mouseUpSignals;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownSignals.SendSignals(this);
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseUpSignals.SendSignals(this);
        }
    }
}
