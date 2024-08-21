using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sound;

    private void Awake()
    {
        if (!audioSource && GetComponent<AudioSource>())
            audioSource = GetComponent<AudioSource>();
    }

    public void OnSignal()
    {
        if (sound)
            audioSource.clip = sound;
        audioSource.Play();
    }
}
