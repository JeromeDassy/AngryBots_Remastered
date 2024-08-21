using UnityEngine;

public class TriggerOnMouseOrJoystick : MonoBehaviour
{
    public SignalSender mouseDownSignals;
    public SignalSender mouseUpSignals;

    private bool state = false;

    private void Update()
    {
#if UNITY_PSP2
        // On consoles use the right trigger to fire
        float fireAxis = Input.GetAxis("TriggerFire");
        if (state == false && (fireAxis >= 0.2f || Input.GetKeyDown(KeyCode.JoystickButton4)))
        {
            mouseDownSignals.SendSignals(this);
            state = true;
        }
        else if (state == true && (fireAxis < 0.2f || Input.GetKeyUp(KeyCode.JoystickButton4)))
        {
            mouseUpSignals.SendSignals(this);
            state = false;
        }
#else
        if (state == false && Input.GetMouseButtonDown(0))
        {
            mouseDownSignals.SendSignals(this);
            state = true;
        }

        else if (state == true && Input.GetMouseButtonUp(0))
        {
            mouseUpSignals.SendSignals(this);
            state = false;
        }
#endif
    }
}