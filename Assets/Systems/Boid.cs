using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Boid : MonoBehaviour
{
    private Vector3 _acceleration;
    private Vector3 _velocity = Vector2.left;
    private List<Boid> _boidsInVision = new List<Boid>();
    private BoidSettings _settings;
    private int _perceivedBoids = 0;
    private Vector3 _separationForce = Vector3.zero;
    private Vector3 _alignmentForce = Vector3.zero;
    private Vector3 _cohesionForce = Vector3.zero;
    public Vector3 Direction => _velocity.normalized;
    
    public void Init(BoidSettings settings)
    {
        Vector3 random = Random.insideUnitCircle;
        _velocity = settings.minSpeed * random.normalized;
        _settings = settings;
    }

    public void DebugVision(List<Boid> visionBoids)
    {
        _boidsInVision = visionBoids;
    }

    public void Simulate()
    {
        Vector3 acceleration = Vector3.zero;
        
        // Apply all forces
        Vector3 separationForce = SteerTowards(_separationForce);
        Vector3 alignmentForce = SteerTowards(_alignmentForce);
        Vector3 cohesionForce = SteerTowards(_cohesionForce);
        acceleration += separationForce * _settings.separationWeight;
        acceleration += alignmentForce * _settings.alignmentWeight;
        acceleration += cohesionForce * _settings.cohesionWeight;

        // Apply obstacle avoidance
        Vector3 obstacleForce = SteerTowards(GetObstacleForce());
        acceleration += obstacleForce * _settings.obstacleWeight;
        
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
        if (difference.magnitude > _settings.visionRadius)
            return false;
        
        // Get the angle to the position
        float angleToPosition = BoidExtensions.GetAngle(difference);
        
        // Get the angles of the bounds of the cone
        float vision = BoidExtensions.GetAngle(_velocity);
        float angle1 = BoidExtensions.AddAngle(vision, _settings.visionAngle * -0.5f);
        float angle2 = BoidExtensions.AddAngle(vision, _settings.visionAngle * 0.5f);
        
        // Set the min and max angles
        float minAngle = angle1 > angle2 ? angle2 : angle1;
        float maxAngle = angle1 > angle2 ? angle1 : angle2;

        // If the size of the vision angle is not a reflex angle, check within the angle bounds
        // If the angle is a reflex angle, check if it is NOT within the angle bounds
        bool inBounds = angleToPosition > minAngle && angleToPosition < maxAngle;
        return _settings.visionAngle <= 180f ? inBounds : !inBounds;
    }

    private Vector3 GetObstacleForce()
    {
        Vector3 obstacleForce = Vector3.zero;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _settings.visionRadius, _settings.obstacleLayer);
        for (int i = 0; i < colliders.Length; ++i)
        {
            Vector3 difference = colliders[i].transform.position - transform.position;
            obstacleForce -= difference.normalized / difference.sqrMagnitude;
            // obstacleForce -= BoidExtensions.GetAttractiveForce(transform.position, colliders[i].transform.position,
            //     _settings.visionRadius);
        }

        return obstacleForce;
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
    
    private Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 velocity = vector.normalized * _settings.maxSpeed - _velocity;
        return Vector3.ClampMagnitude (velocity, _settings.maxSteerForce);
    }

    private void OnDrawGizmosSelected()
    {
        // Debugging the velocity
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + _velocity);
        
        // Debugging the cone of vision
        Gizmos.color = Color.magenta;
        GizmosExtensions.DrawWireArc(transform.position, _velocity, _settings.visionAngle, _settings.visionRadius);

        if (_perceivedBoids > 0)
        {
            // Debugging nearby boids
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _boidsInVision.Count; ++i)
            {
                Gizmos.DrawLine(transform.position, _boidsInVision[i].transform.position);
            }

            // Debugging the cohesion force
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + _cohesionForce);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _separationForce);
        }
    }
}
