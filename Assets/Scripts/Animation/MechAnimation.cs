using UnityEngine;

public class MechAnimation : MonoBehaviour
{
    public Rigidbody rigid;
    public AnimationClip idle;
    public AnimationClip walk;
    public AnimationClip turnLeft;
    public AnimationClip turnRight;
    public SignalSender footstepSignals;

    private Transform tr;
    private float lastFootstepTime = 0;
    private float lastAnimTime = 0;

    private void OnEnable()
    {
        tr = rigid.transform;

        Animation animationComponent = GetComponent<Animation>();

        animationComponent[idle.name].layer = 0;
        animationComponent[idle.name].weight = 1;
        animationComponent[idle.name].enabled = true;

        animationComponent[walk.name].layer = 1;
        animationComponent[turnLeft.name].layer = 1;
        animationComponent[turnRight.name].layer = 1;

        animationComponent[walk.name].weight = 1;
        animationComponent[turnLeft.name].weight = 0;
        animationComponent[turnRight.name].weight = 0;

        animationComponent[walk.name].enabled = true;
        animationComponent[turnLeft.name].enabled = true;
        animationComponent[turnRight.name].enabled = true;
    }

    private void FixedUpdate()
    {
        float turningWeight = Mathf.Abs(rigid.angularVelocity.y) * Mathf.Rad2Deg / 100.0f;
        float forwardWeight = rigid.velocity.magnitude / 2.5f;
        float turningDir = Mathf.Sign(rigid.angularVelocity.y);

        Animation animationComponent = GetComponent<Animation>();

        animationComponent[walk.name].speed = Mathf.Lerp(1.0f, animationComponent[walk.name].length / animationComponent[turnLeft.name].length * 1.33f, turningWeight);
        animationComponent[turnLeft.name].time = animationComponent[walk.name].time;
        animationComponent[turnRight.name].time = animationComponent[walk.name].time;

        animationComponent[turnLeft.name].weight = Mathf.Clamp01(-turningWeight * turningDir);
        animationComponent[turnRight.name].weight = Mathf.Clamp01(turningWeight * turningDir);
        animationComponent[walk.name].weight = Mathf.Clamp01(forwardWeight);

        if (forwardWeight + turningWeight > 0.1f)
        {
            float newAnimTime = Mathf.Repeat(animationComponent[walk.name].normalizedTime * 2 + 0.1f, 1);
            if (newAnimTime < lastAnimTime)
            {
                if (Time.time > lastFootstepTime + 0.1f)
                {
                    footstepSignals.SendSignals(this);
                    lastFootstepTime = Time.time;
                }
            }
            lastAnimTime = newAnimTime;
        }
    }
}
