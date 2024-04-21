using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid3D : Boid
{
    protected override void InitVelocity(float speed)
    {
        Vector3 random = Random.insideUnitSphere;
        velocity = speed * random.normalized;
    }

    protected override void UpdateTransform()
    {
        transform.forward = velocity.normalized;
    }

    public override bool CanSee(Vector3 position)
    {
        return false;
    }

    protected override Vector3 GetObstacleForce()
    {
        return default;
    }
}
