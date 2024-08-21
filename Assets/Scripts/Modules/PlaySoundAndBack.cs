using UnityEngine;

public class PlaySoundAndBack : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sound;
    public AudioClip soundReverse;
    public float lengthWithoutTrailing = 0;

    private bool back = false;
    private float normalizedTime = 0;

    private void Awake()
    {
        if (!audioSource && GetComponent<AudioSource>())
            audioSource = GetComponent<AudioSource>();
        if (lengthWithoutTrailing == 0)
            lengthWithoutTrailing = Mathf.Min(sound.length, soundReverse.length);
    }

    public void OnSignal()
    {
        FixTime();
        PlayWithDirection();
    }

    public void OnPlay()
    {
        FixTime();
        back = false;
        PlayWithDirection();
    }

    public void OnPlayReverse()
    {
        FixTime();
        back = true;
        PlayWithDirection();
    }

    private void PlayWithDirection()
    {
        AudioClip clipToPlay;
        float playbackTime;

        if (back)
        {
            clipToPlay = soundReverse;
            playbackTime = (1 - normalizedTime) * lengthWithoutTrailing;
        }
        else
        {
            clipToPlay = sound;
            playbackTime = normalizedTime * lengthWithoutTrailing;
        }

        audioSource.clip = clipToPlay;
        audioSource.time = playbackTime;
        audioSource.Play();

        back = !back;
    }

    private void FixTime()
    {
        if (audioSource.clip)
        {
            normalizedTime = 1.0f;
            if (audioSource.isPlaying)
                normalizedTime = Mathf.Clamp01(audioSource.time / lengthWithoutTrailing);
            if (audioSource.clip == soundReverse)
                normalizedTime = 1 - normalizedTime;
        }
    }
}
