using UnityEngine;

public class PlayAnimationAndBack : MonoBehaviour
{
    public string clip = "MyAnimation";
    public float speed = 1.0f;

    public void OnSignal()
    {
        FixTime();

        PlayWithSpeed();
    }

    public void OnPlay()
    {
        FixTime();

        // Set the speed to be positive
        speed = Mathf.Abs(speed);

        PlayWithSpeed();
    }

    public void OnPlayReverse()
    {
        FixTime();

        // Set the speed to be negative
        speed = Mathf.Abs(speed) * -1;

        PlayWithSpeed();
    }

    private void PlayWithSpeed()
    {
        // Play the animation with the desired speed.
        Animation anim = GetComponent<Animation>();
        anim[clip].speed = speed;
        anim[clip].weight = 1;
        anim[clip].enabled = true;

        // Reverse the speed so it's ready for playing the other way next time.
        speed = -speed;
    }

    private void FixTime()
    {
        Animation anim = GetComponent<Animation>();

        // If the animation played to the end last time, it got automatically rewinded.
        // We don't want that here, so set the time back to 1.
        if (speed < 0 && anim[clip].time == 0)
            anim[clip].normalizedTime = 1;

        // In other cases, just clamp the time so it doesn't exceed the bounds of the animation.
        else
            anim[clip].normalizedTime = Mathf.Clamp01(anim[clip].normalizedTime);
    }
}
