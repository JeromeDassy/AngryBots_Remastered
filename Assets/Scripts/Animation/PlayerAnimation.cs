using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [System.Serializable]
    public class MoveAnimation
    {
        public AnimationClip clip;
        public Vector3 velocity;
        [HideInInspector]
        public float weight;
        [HideInInspector]
        public bool currentBest = false;
        [HideInInspector]
        public float speed;
        [HideInInspector]
        public float angle;

        public void Init()
        {
            velocity.y = 0;
            speed = velocity.magnitude;
            angle = HorizontalAngle(velocity);
        }
    }

    public Rigidbody rigid;
    public Transform rootBone;
    public Transform upperBodyBone;
    public float maxIdleSpeed = 0.5f;
    public float minWalkSpeed = 2.0f;
    public AnimationClip idle;
    public AnimationClip turn;
    public AnimationClip shootAdditive;
    public MoveAnimation[] moveAnimations;
    public SignalSender footstepSignals;

    private Transform tr;
    private Vector3 lastPosition = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 localVelocity = Vector3.zero;
    private float speed = 0f;
    private float angle = 0f;
    private float lowerBodyDeltaAngle = 0f;
    private float idleWeight = 0f;
    private Vector3 lowerBodyForwardTarget = Vector3.forward;
    private Vector3 lowerBodyForward = Vector3.forward;
    private MoveAnimation bestAnimation = null;
    private float lastFootstepTime = 0f;
    private float lastAnimTime = 0f;

    public Animation animationComponent;

    private void Awake()
    {
        tr = rigid.transform;
        lastPosition = tr.position;

        foreach (MoveAnimation moveAnimation in moveAnimations)
        {
            moveAnimation.Init();
            animationComponent[moveAnimation.clip.name].layer = 1;
            animationComponent[moveAnimation.clip.name].enabled = true;
        }
        animationComponent.SyncLayer(1);

        animationComponent[idle.name].layer = 2;
        animationComponent[turn.name].layer = 3;
        animationComponent[idle.name].enabled = true;

        animationComponent[shootAdditive.name].layer = 4;
        animationComponent[shootAdditive.name].weight = 1f;
        animationComponent[shootAdditive.name].speed = 0.6f;
        animationComponent[shootAdditive.name].blendMode = AnimationBlendMode.Additive;
    }

    public void OnStartFire()
    {
        if (Time.timeScale == 0f)
            return;

        animationComponent[shootAdditive.name].enabled = true;
    }

    public void OnStopFire()
    {
        animationComponent[shootAdditive.name].enabled = false;
    }

    private void FixedUpdate()
    {
        velocity = (tr.position - lastPosition) / Time.deltaTime;
        localVelocity = tr.InverseTransformDirection(velocity);
        localVelocity.y = 0f;
        speed = localVelocity.magnitude;
        angle = HorizontalAngle(localVelocity);

        lastPosition = tr.position;
    }

    private void Update()
    {
        idleWeight = Mathf.Lerp(idleWeight, Mathf.InverseLerp(minWalkSpeed, maxIdleSpeed, speed), Time.deltaTime * 10f);
        animationComponent[idle.name].weight = idleWeight;

        if (speed > 0f)
        {
            float smallestDiff = Mathf.Infinity;
            foreach (MoveAnimation moveAnimation in moveAnimations)
            {
                float angleDiff = Mathf.Abs(Mathf.DeltaAngle(angle, moveAnimation.angle));
                float speedDiff = Mathf.Abs(speed - moveAnimation.speed);
                float diff = angleDiff + speedDiff;
                if (moveAnimation == bestAnimation)
                    diff *= 0.9f;

                if (diff < smallestDiff)
                {
                    bestAnimation = moveAnimation;
                    smallestDiff = diff;
                }
            }

            animationComponent.CrossFade(bestAnimation.clip.name);
        }
        else
        {
            bestAnimation = null;
        }

        if (lowerBodyForward != lowerBodyForwardTarget && idleWeight >= 0.9f)
            animationComponent.CrossFade(turn.name, 0.05f);

        if (bestAnimation != null && idleWeight < 0.9f)// TODO :(bestAnimation && idleWeight < 0.9f)
        {
            float newAnimTime = Mathf.Repeat(animationComponent[bestAnimation.clip.name].normalizedTime * 2f + 0.1f, 1f);
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

    private void LateUpdate()
    {
        float idle = Mathf.InverseLerp(minWalkSpeed, maxIdleSpeed, speed);

        if (idle < 1f)
        {
            Vector3 animatedLocalVelocity = Vector3.zero;
            foreach (MoveAnimation moveAnimation in moveAnimations)
            {
                if (animationComponent[moveAnimation.clip.name].weight == 0f)
                    continue;

                if (Vector3.Dot(moveAnimation.velocity, localVelocity) <= 0f)
                    continue;

                animatedLocalVelocity += moveAnimation.velocity * animationComponent[moveAnimation.clip.name].weight;
            }

            float lowerBodyDeltaAngleTarget = Mathf.DeltaAngle(
                HorizontalAngle(tr.rotation * animatedLocalVelocity),
                HorizontalAngle(velocity)
            );

            lowerBodyDeltaAngle = Mathf.LerpAngle(lowerBodyDeltaAngle, lowerBodyDeltaAngleTarget, Time.deltaTime * 10f);

            lowerBodyForwardTarget = tr.forward;
            lowerBodyForward = Quaternion.Euler(0f, lowerBodyDeltaAngle, 0f) * lowerBodyForwardTarget;
        }
        else
        {
            lowerBodyForward = Vector3.RotateTowards(lowerBodyForward, lowerBodyForwardTarget, Time.deltaTime * 520f * Mathf.Deg2Rad, 1f);

            lowerBodyDeltaAngle = Mathf.DeltaAngle(
                HorizontalAngle(tr.forward),
                HorizontalAngle(lowerBodyForward)
            );

            if (Mathf.Abs(lowerBodyDeltaAngle) > 80f)
                lowerBodyForwardTarget = tr.forward;
        }

        Quaternion lowerBodyDeltaRotation = Quaternion.Euler(0f, lowerBodyDeltaAngle, 0f);

        rootBone.rotation = lowerBodyDeltaRotation * rootBone.rotation;

        upperBodyBone.rotation = Quaternion.Inverse(lowerBodyDeltaRotation) * upperBodyBone.rotation;
    }

    static float HorizontalAngle(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }
}
