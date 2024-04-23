using System.Collections.Generic;
using UnityEngine;

public class Boid2D : Boid
{
    private const int MAX_STEPS = 20;
    private Vector3[] _debugRays = new Vector3[MAX_STEPS];
    protected override void InitVelocity(float speed)
    {
        Vector2 random = Random.insideUnitCircle;
        velocity = speed * random.normalized;
    }

    protected override void UpdateTransform()
    {
        transform.rotation = Quaternion.Euler(0, 0, BoidExtensions.GetTransformAngle(velocity));
    }

    public override bool CanSee(Vector3 position)
    {
        Vector3 difference = position - transform.position;
        // If too far away, then cannot see
        if (difference.magnitude > settings.visionRadius)
            return false;
        
        // Get the angle to the position
        float angleToPosition = BoidExtensions.GetAngle(difference);
        
        // Get the angles of the bounds of the cone
        float vision = BoidExtensions.GetAngle(velocity);
        float angle1 = BoidExtensions.AddAngle(vision, settings.visionAngle * -0.5f);
        float angle2 = BoidExtensions.AddAngle(vision, settings.visionAngle * 0.5f);
        
        // Set the min and max angles
        float minAngle = angle1 > angle2 ? angle2 : angle1;
        float maxAngle = angle1 > angle2 ? angle1 : angle2;

        // If the size of the vision angle is not a reflex angle, check within the angle bounds
        // If the angle is a reflex angle, check if it is NOT within the angle bounds
        bool inBounds = angleToPosition > minAngle && angleToPosition < maxAngle;
        return settings.visionAngle <= 180f ? inBounds : !inBounds;
    }
    
    protected override Vector3 GetObstacleForce()
    {
        return HasCollision() ? GetObstacleRay() : Vector3.zero;
        Vector3 obstacleForce = Vector3.zero;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, settings.visionRadius, settings.obstacleLayer);
        for (int i = 0; i < colliders.Length; ++i)
        {
            Vector3 difference = colliders[i].transform.position - transform.position;
            obstacleForce -= difference.normalized / difference.sqrMagnitude;
        }

        return obstacleForce;
    }

    private bool HasCollision()
    {
        return Physics2D.OverlapCircle(transform.position, settings.obstacleVisionRadius, settings.obstacleLayer);
    }

    private Vector3 GetObstacleRay()
    {
        Vector3 obstacleRay = Vector3.zero;
        Vector3 initialPosition = transform.position;
        float initialAngle = BoidExtensions.GetAngle(velocity);
        float stepAngles = settings.visionAngle / MAX_STEPS;
        float angle = initialAngle - settings.visionAngle * 0.5f;
        for (int i = 0; i < MAX_STEPS; ++i)
        {
            Debug.Log(angle);
            float rad = Mathf.Deg2Rad * angle;
            Vector3 rayDirection = new Vector3(settings.obstacleVisionRadius * Mathf.Cos(rad), settings.obstacleVisionRadius * Mathf.Sin(rad), 0f);
            _debugRays[i] = rayDirection + initialPosition;
            // Shoot a ray to the position
            RaycastHit2D ray = Physics2D.Raycast(initialPosition, rayDirection, settings.obstacleVisionRadius, settings.obstacleLayer);
            if (ray)
            {
                Vector3 raycast = rayDirection.normalized * ray.distance;
                obstacleRay -= raycast.normalized / raycast.sqrMagnitude;
            }
            angle += stepAngles;
        }

        return obstacleRay;
    }

    protected override void DebugGizmos()
    {
        base.DebugGizmos();
        // Debugging the cone of vision
        if (settings)
        {
            Gizmos.color = Color.magenta;
            GizmosExtensions.DrawWireArc(transform.position, velocity, settings.visionAngle, settings.visionRadius);
            Gizmos.color = Color.green;
            if (HasCollision())
            {
                for (int i = 0; i < MAX_STEPS; ++i)
                {
                    Gizmos.DrawLine(transform.position, _debugRays[i]);
                }
            }
        }
    }
}
