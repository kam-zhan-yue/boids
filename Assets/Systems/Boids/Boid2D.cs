using System.Collections.Generic;
using UnityEngine;

public class Boid2D : Boid
{
    private SpriteRenderer _spriteRenderer;
    private const int MAX_STEPS = 20;
    private Vector3[] _debugRays = new Vector3[MAX_STEPS];

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void InitGroup(BoidGroup boidGroup)
    {
        base.InitGroup(boidGroup);
        if(boidGroup)
            _spriteRenderer.color = boidGroup.color;
    }

    protected override void InitVelocity(float speed)
    {
        if (velocity == Vector3.zero)
        {
            Vector2 random = Random.insideUnitCircle;
            velocity = speed * random.normalized;
        }
        else
        {
            velocity *= speed;
        }
    }

    protected override void UpdateTransform()
    {
        transform.rotation = Quaternion.Euler(0, 0, BoidExtensions.GetTransformAngle(velocity));
    }
    
    protected override Vector3 GetObstacleForce()
    {
        return HasCollision() ? GetObstacleRay() : Vector3.zero;
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
