using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleVisualiser : MonoBehaviour
{
    public float visionRadius;
    public float visionAngle;
    public Transform point1;
    public Transform point2;

    private bool InRange()
    {
        // Get the vectors from the transforms
        Vector3 vector1 = point1.position - transform.position;
        Vector3 vector2 = point2.position - transform.position;
        // If point is far away, not valid
        if (vector2.magnitude > visionRadius)
            return false;

        // Calculate the angle between the vectors
        float directionAngle = Vector3.Angle(transform.position, vector1);
        float angle = Vector3.Angle(vector1, vector2);

        Debug.Log($"Direction Angle: {directionAngle} Angle Between: {angle}");
        // Return if the angle is within range
        return angle <= visionAngle * 0.5f;
    }

    private void OnDrawGizmos()
    {
        if (point1 && point2)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, point1.position);
            Gizmos.color = Color.magenta;
            GizmosExtensions.DrawWireArc(transform.position, point1.position - transform.position, visionAngle, visionRadius);
            Gizmos.color = InRange() ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, point2.position);
        }
    }
}
