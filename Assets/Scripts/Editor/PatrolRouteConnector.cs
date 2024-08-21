using UnityEditor;
using UnityEngine;

public class PatrolRouteConnector : MonoBehaviour
{
    [MenuItem("Tools/Assign Closest Patrol Routes")]
    static void AssignPatrolRoutes()
    {
        PatrolPoint[] points = FindObjectsOfType<PatrolPoint>();
        PatrolMoveController[] patrollers = FindObjectsOfType<PatrolMoveController>();
        int connected = 0;

        foreach (PatrolMoveController patroller in patrollers)
        {
            float closestDist = Mathf.Infinity;
            PatrolPoint closestPoint = null;

            foreach (PatrolPoint point in points)
            {
                float dist = (patroller.transform.position - point.transform.position).magnitude;
                if (dist < closestDist)
                {
                    closestPoint = point;
                    closestDist = dist;
                }
            }

            if (closestPoint != null)
            {
                patroller.patrolRoute = closestPoint.transform.parent.GetComponent<PatrolRoute>();
                connected++;
            }
        }

        Debug.Log("Successfully connected routes to " + connected + " out of " + patrollers.Length + " patrollers.");
    }
}
