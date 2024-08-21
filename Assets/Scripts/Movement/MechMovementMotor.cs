using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MechMovementMotor : MovementMotor
{
    public float walkingSpeed = 3.0f;
    public float turningSpeed = 100.0f;
    public float aimingSpeed = 150.0f;

    public Transform head;

    private Vector3 wallHit;
    private bool facingInRightDirection = false;
    private Quaternion headRotation = Quaternion.identity;

    private void FixedUpdate()
    {
        Vector3 adjustedMovementDirection = movementDirection;

        float rotationAngle;
        if (adjustedMovementDirection != Vector3.zero)
            rotationAngle = AngleAroundAxis(transform.forward, adjustedMovementDirection, Vector3.up) * 0.3f;
        else
            rotationAngle = 0;

        Vector3 targetAngularVelocity = Vector3.up * Mathf.Clamp(rotationAngle, -turningSpeed * Mathf.Deg2Rad, turningSpeed * Mathf.Deg2Rad);
        GetComponent<Rigidbody>().angularVelocity = Vector3.MoveTowards(GetComponent<Rigidbody>().angularVelocity, targetAngularVelocity, Time.deltaTime * turningSpeed * Mathf.Deg2Rad * 3);

        float angle = Vector3.Angle(transform.forward, adjustedMovementDirection);
        if (facingInRightDirection && angle > 25)
            facingInRightDirection = false;
        if (!facingInRightDirection && angle < 5)
            facingInRightDirection = true;

        Vector3 targetVelocity;
        if (facingInRightDirection)
            targetVelocity = transform.forward * walkingSpeed + GetComponent<Rigidbody>().velocity.y * Vector3.up;
        else
            targetVelocity = GetComponent<Rigidbody>().velocity.y * Vector3.up;

        GetComponent<Rigidbody>().velocity = Vector3.MoveTowards(GetComponent<Rigidbody>().velocity, targetVelocity, Time.deltaTime * walkingSpeed * 3);
    }

    private void LateUpdate()
    {
        if (facingDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(facingDirection);
            headRotation = Quaternion.RotateTowards(
                headRotation,
                targetRotation,
                aimingSpeed * Time.deltaTime
            );
            head.rotation = headRotation * Quaternion.Inverse(transform.rotation) * head.rotation;
        }
    }

    static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);

        float angle = Vector3.Angle(dirA, dirB);

        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }
}
