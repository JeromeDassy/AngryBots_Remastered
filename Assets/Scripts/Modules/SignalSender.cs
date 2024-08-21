using System.Collections;
using UnityEngine;

[System.Serializable]
public class ReceiverItem
{
    public GameObject receiver;
    public string action = "OnSignal";
    public float delay;
    public MonoBehaviour targetMonoBehaviour;

    public IEnumerator SendWithDelay(MonoBehaviour sender)
    {
        yield return new WaitForSeconds(delay);
        if (receiver)
        {
            if (targetMonoBehaviour != null)
            {
                targetMonoBehaviour.SendMessage(action);
            }
            else
            {
                if (receiver)
                    receiver.SendMessage(action);
                else
                    Debug.LogWarning("No receiver of signal \"" + action + "\" on object " + sender.name + " (" + sender.GetType().Name + ")", sender);
            }
        }
        else
        {
            Debug.LogWarning("No receiver of signal \"" + action + "\" on object " + sender.name + " (" + sender.GetType().Name + ")", sender);
        }
    }
}

//public class ReceiverItem
//{
//    public GameObject receiver;
//    public string action = "OnSignal";
//    public float delay;

//    public IEnumerator SendWithDelay(MonoBehaviour sender)
//    {
//        yield return new WaitForSeconds(delay);
//        if (receiver)
//            receiver.SendMessage(action);
//        else
//            Debug.LogWarning("No receiver of signal \"" + action + "\" on object " + sender.name + " (" + sender.GetType().Name + ")", sender);
//    }
//}

[System.Serializable]
public class SignalSender
{
    public bool onlyOnce;
    public ReceiverItem[] receivers;

    private bool hasFired = false;

    public void SendSignals(MonoBehaviour sender)
    {
        if (hasFired == false || onlyOnce == false)
        {
            foreach (ReceiverItem receiverItem in receivers)
            {
                sender.StartCoroutine(receiverItem.SendWithDelay(sender));
            }
            hasFired = true;
        }
    }
}
