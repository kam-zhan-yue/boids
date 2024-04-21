using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Vector3 _acceleration;
    private Vector2 _velocity = Vector2.left;
    private float _visionRadius;
    private float _visionAngle;
    private List<Boid> _boidsInVision = new List<Boid>();
    private BoidSettings _settings;
    private int _perceivedBoids = 0;
    private Vector3 _separationForce = Vector3.zero;
    private Vector3 _alignmentForce = Vector3.zero;
    private Vector3 _cohesionForce = Vector3.zero;
    public Vector3 Direction => _velocity.normalized;

    public void Init(BoidSettings settings)
    {
        _velocity = settings.minSpeed * Vector2.up;
        _visionRadius = settings.visionRadius;
        _visionAngle = settings.visionAngle;
        _settings = settings;
    }

    public void DebugVision(List<Boid> visionBoids)
    {
        _boidsInVision = visionBoids;
    }

    public void Simulate()
    {
        Vector2 acceleration = Vector2.zero;
        if (_perceivedBoids > 0)
        {
            Vector2 separationForce = SteerTowards(_separationForce);
            Vector2 alignmentForce = SteerTowards(_alignmentForce);
            Vector2 cohesionForce = SteerTowards(_cohesionForce);
            acceleration += separationForce;
            acceleration += alignmentForce;
            acceleration += cohesionForce;
        }

        // Update the velocity by all forces
        _velocity += acceleration * Time.deltaTime;
        
        // Clamp the velocity to a min and max speed
        float speed = _velocity.magnitude;
        Vector3 direction = _velocity.normalized;
        speed = Mathf.Clamp (speed, _settings.minSpeed, _settings.maxSpeed);
        _velocity = direction * speed;
        // Update the position and rotation of the boid
        transform.position += (Vector3)_velocity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, BoidExtensions.GetTransformAngle(_velocity));
    }

    public bool CanSee(Vector3 position)
    {
        Vector3 difference = position - transform.position;
        // If too far away, then cannot see
        if (difference.magnitude > _visionRadius)
            return false;
        
        // Get the angle to the position
        float angleToPosition = BoidExtensions.GetAngle(difference);
        
        // Get the angles of the bounds of the cone
        float vision = BoidExtensions.GetAngle(_velocity);
        float angle1 = BoidExtensions.AddAngle(vision, _visionAngle * -0.5f);
        float angle2 = BoidExtensions.AddAngle(vision, _visionAngle * 0.5f);
        
        // Set the min and max angles
        float minAngle = angle1 > angle2 ? angle2 : angle1;
        float maxAngle = angle1 > angle2 ? angle1 : angle2;

        // If the size of the vision angle is not a reflex angle, check within the angle bounds
        // If the angle is a reflex angle, check if it is NOT within the angle bounds
        bool inBounds = angleToPosition > minAngle && angleToPosition < maxAngle;
        return _visionAngle <= 180f ? inBounds : !inBounds;
    }

    public void SetPerceivedBoids(int perceivedBoids)
    {
        _perceivedBoids = perceivedBoids;
    }
    
    public void SetSeparation(Vector2 separation)
    {
        _separationForce = separation;
    }

    public void SetAlignment(Vector2 separation)
    {
        _alignmentForce = separation;
    }
    
    public void SetCohesion(Vector2 separation)
    {
        _cohesionForce = separation;
    }
    
    private Vector2 SteerTowards(Vector2 vector)
    {
        Vector2 velocity = vector.normalized * _settings.maxSpeed - _velocity;
        return Vector2.ClampMagnitude (velocity, _settings.maxSteerForce);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, ((Vector2)transform.position + _velocity));
        
        Gizmos.color = Color.magenta;
        GizmosExtensions.DrawWireArc(transform.position, _velocity, _visionAngle, _visionRadius);
        
        Gizmos.color = Color.yellow;
        for (int i = 0; i < _boidsInVision.Count; ++i)
        {
            Gizmos.DrawLine(transform.position, _boidsInVision[i].transform.position);
        }
    }
}
