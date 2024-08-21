using UnityEngine;

public class MuzzleFlashAnimate : MonoBehaviour
{
    void Update()
    {
        transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Random.Range(0f, 90f));
    }
}