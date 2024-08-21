using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    public List<GameObject> affected = new List<GameObject>();

    private bool playerInsideArea = false;

    void Start()
    {
        ActivateAffected(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideArea = true;
            ActivateAffected(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideArea = false;
            ActivateAffected(false);
        }
    }

    void ActivateAffected(bool state)
    {
        foreach (GameObject go in affected)
        {
            if (go == null)
                continue;
            go.SetActive(state);
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(state);
        }
    }
}
