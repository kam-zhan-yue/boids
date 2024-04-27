using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid3D : Boid
{
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public override void InitGroup(BoidGroup boidGroup)
    {
        base.InitGroup(boidGroup);
        if (boidGroup)
            _meshRenderer.material = boidGroup.material;
    }
    
    protected override void InitVelocity(float speed)
    {
        if (velocity == Vector3.zero)
        {
            Vector3 random = Random.insideUnitSphere;
            velocity = speed * random.normalized;
        }
        else
        {
            velocity *= speed;
        }
    }

    protected override void UpdateTransform()
    {
        transform.forward = velocity.normalized;
    }

    protected override Vector3 GetObstacleForce()
    {
        if (HasCollision())
        {
            // Debug.Log("Has Collision!");
            return GetObstacleRay();
        }
        else
        {
            // Debug.Log("No Collision!");
            return default;
        }
        return HasCollision() ? GetObstacleRay() : default;
    }

    private bool HasCollision()
    {
        return Physics.SphereCast(transform.position, settings.obstacleVisionRadius, velocity, out _, settings.obstacleVisionRadius,
            settings.obstacleLayer);
    }

    private Vector3 GetObstacleRay()
    {
        Vector3[] directions = BoidExtensions.Directions;
        Vector3 obstacleRay = Vector3.zero;

        for (int i = 0; i < directions.Length; ++i) {
            Vector3 rayDirection = transform.TransformDirection(directions[i]);
            Ray ray = new Ray (transform.position, rayDirection);
            if (!Physics.SphereCast (ray, settings.obstacleVisionRadius, settings.obstacleVisionRadius, settings.obstacleLayer)) {
                obstacleRay += rayDirection;
            }
        }
        return obstacleRay;
    }

    protected override void DebugGizmos()
    {
        base.DebugGizmos();
        if (settings)
        {
            // Gizmos.color = Color.magenta;
            // Vector3[] directions = BoidExtensions.Directions;
            // for (int i = 0; i < directions.Length; ++i)
            // {
            //     Vector3 rayDirection = transform.TransformDirection(directions[i]);
            //     Gizmos.DrawLine(transform.position, transform.position + rayDirection);
            // }
        }
    }
}
