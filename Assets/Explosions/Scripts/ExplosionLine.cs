using UnityEngine;

public class ExplosionLine : MonoBehaviour
{
    public int frames = 2;
    private int _frames = 0;

    private void OnEnable()
    {
        _frames = 0;
    }

    private void Update()
    {
        _frames++;

        if (_frames > frames)
        {
            gameObject.SetActive(false);
        }
    }
}
