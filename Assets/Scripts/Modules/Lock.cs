using UnityEngine;

[System.Serializable]
public class Lock : MonoBehaviour
{
    public bool locked = true;
    public SignalSender unlockedSignal;

    public void OnSignal()
    {
        if (locked)
        {
            locked = false;
            unlockedSignal.SendSignals(this);
        }
    }
}
