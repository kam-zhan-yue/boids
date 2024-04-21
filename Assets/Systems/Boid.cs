using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Vector3 _acceleration;
    private Vector3 _velocity = Vector2.left;
    private float _visionRadius;
    private float _visionAngle;
    private List<Boid> _boidsInVision = new List<Boid>();

    public void Init(BoidSettings settings)
    {
        _velocity = settings.speed * Vector2.up;
        _visionRadius = settings.visionRadius;
        _visionAngle = settings.visionAngle;
    }

    public void DebugVision(List<Boid> visionBoids)
    {
        _boidsInVision = visionBoids;
    }

    public void Simulate()
    {
        transform.position += _velocity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, BoidExtensions.GetAngle(_velocity));
    }

    public bool CanSee(Vector3 position)
    {
        Vector3 difference = position - transform.position;
        // If too far away, then cannot see
        if (difference.magnitude > _visionRadius)
            return false;
        float angleToPosition = BoidExtensions.GetAngle(difference);
        float vision = BoidExtensions.GetAngle(_velocity);
        float minAngle = vision - _visionAngle * 0.5f;
        float maxAngle = vision + _visionAngle * 0.5f;
        
        return angleToPosition > minAngle && angleToPosition > maxAngle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)(transform.position + _velocity));
        
        Gizmos.color = Color.magenta;
        GizmosExtensions.DrawWireArc(transform.position, _velocity, _visionAngle, _visionRadius);
        
        Gizmos.color = Color.yellow;
        for (int i = 0; i < _boidsInVision.Count; ++i)
        {
            Gizmos.DrawLine(transform.position, _boidsInVision[i].transform.position);
        }
    }
}
