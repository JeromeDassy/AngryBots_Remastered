﻿using UnityEngine;

public class PatrolMoveController : MonoBehaviour
{
    public MovementMotor motor;
    public PatrolRoute patrolRoute;
    public float patrolPointRadius = 0.5f;

    private Transform character;
    private int nextPatrolPoint = 0;
    private int patrolDirection = 1;

    private void Start()
    {
        character = motor.transform;
        patrolRoute.Register(transform.parent.gameObject);
    }

    private void OnEnable()
    {
        nextPatrolPoint = patrolRoute.GetClosestPatrolPoint(transform.position);
    }

    private void OnDestroy()
    {
        patrolRoute.UnRegister(transform.parent.gameObject);
    }

    private void Update()
    {
        // Early out if there are no patrol points
        if (patrolRoute == null || patrolRoute.patrolPoints.Count == 0)
            return;

        // Find the vector towards the next patrol point.
        Vector3 targetVector = patrolRoute.patrolPoints[nextPatrolPoint].position - character.position;
        targetVector.y = 0;

        // If the patrol point has been reached, select the next one.
        if (targetVector.sqrMagnitude < patrolPointRadius * patrolPointRadius)
        {
            nextPatrolPoint += patrolDirection;
            if (nextPatrolPoint < 0)
            {
                nextPatrolPoint = 1;
                patrolDirection = 1;
            }
            if (nextPatrolPoint >= patrolRoute.patrolPoints.Count)
            {
                if (patrolRoute.pingPong)
                {
                    patrolDirection = -1;
                    nextPatrolPoint = patrolRoute.patrolPoints.Count - 2;
                }
                else
                {
                    nextPatrolPoint = 0;
                }
            }
        }

        // Make sure the target vector doesn't exceed a length of 1
        if (targetVector.sqrMagnitude > 1)
            targetVector.Normalize();

        // Set the movement direction.
        motor.movementDirection = targetVector;
        // Set the facing direction.
        motor.facingDirection = targetVector;
    }
}
