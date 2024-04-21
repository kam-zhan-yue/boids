using UnityEngine;

public class Boid2D : Boid
{
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
        Vector3 obstacleForce = Vector3.zero;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, settings.visionRadius, settings.obstacleLayer);
        for (int i = 0; i < colliders.Length; ++i)
        {
            Vector3 difference = colliders[i].transform.position - transform.position;
            obstacleForce -= difference.normalized / difference.sqrMagnitude;
        }

        return obstacleForce;
    }

    protected override void Debug()
    {
        base.Debug();
        // Debugging the cone of vision
        if (settings)
        {
            Gizmos.color = Color.magenta;
            GizmosExtensions.DrawWireArc(transform.position, velocity, settings.visionAngle, settings.visionRadius);
        }
    }
}
