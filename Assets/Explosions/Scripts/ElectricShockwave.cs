using UnityEngine;

public class ElectricShockwave : MonoBehaviour
{
    public float autoDisableAfter = 2.0f;

    private void OnEnable()
    {
        StartCoroutine(DeactivateCoroutine(autoDisableAfter));
    }

    private System.Collections.IEnumerator DeactivateCoroutine(float t)
    {
        yield return new WaitForSeconds(t);

        gameObject.SetActive(false);
    }
}
