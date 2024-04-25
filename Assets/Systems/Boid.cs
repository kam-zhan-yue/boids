using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Boid : MonoBehaviour
{
    protected Vector3 velocity = Vector2.zero;
    protected BoidSettings settings;
    private Vector3 _separationForce = Vector3.zero;
    private Vector3 _alignmentForce = Vector3.zero;
    private Vector3 _cohesionForce = Vector3.zero;
    private Vector3 _obstacleForce = Vector3.zero;
    private Vector3 _avoidanceForce = Vector3.zero;
    private List<Boid> _boidsInVision = new List<Boid>();
    private int _perceivedBoids;
    private int _groupID;
    private bool _predator;
    public Vector3 Direction => velocity.normalized;
    public int GroupID => _groupID;
    public bool Predator => _predator;

    public virtual void InitGroup(BoidGroup boidGroup)
    {
        if (boidGroup)
        {
            _predator = boidGroup.predator;
            _groupID = boidGroup.name.GetHashCode();
        }
    }

    public void Init(BoidSettings boidSettings)
    {
        settings = boidSettings;
        InitVelocity((settings.minSpeed + settings.maxSpeed) * 0.5f);
    }

    public void InitDirection(Vector3 startDirection)
    {
        velocity = startDirection.normalized;
    }

    protected abstract void InitVelocity(float speed);

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
        Vector3 avoidanceForce = SteerTowards(_avoidanceForce);
        acceleration += separationForce * settings.separationWeight;
        acceleration += alignmentForce * settings.alignmentWeight;
        acceleration += cohesionForce * settings.cohesionWeight;
        acceleration += avoidanceForce * settings.avoidanceWeight;

        // Apply obstacle avoidance
        _obstacleForce = SteerTowards(GetObstacleForce());
        acceleration += _obstacleForce * settings.obstacleWeight;
        
        // Update the velocity by all forces
        velocity += acceleration * Time.deltaTime;


        // Clamp the velocity to a min and max speed
        float speed = velocity.magnitude;
        Vector3 direction = velocity.normalized;
        speed = Mathf.Clamp (speed, settings.minSpeed, settings.maxSpeed);
        velocity = direction * speed;
        // Update the position and rotation of the boid
        transform.position += velocity * Time.deltaTime;
        UpdateTransform();
    }

    protected abstract void UpdateTransform();

    public abstract bool CanSee(Vector3 position);

    protected abstract Vector3 GetObstacleForce();

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

    public void SetAvoidance(Vector3 avoidance)
    {
        _avoidanceForce = avoidance;
    }
    
    private Vector3 SteerTowards(Vector3 vector)
    {
        if (vector == Vector3.zero)
            return Vector3.zero;
        Vector3 v = vector.normalized * settings.maxSpeed - this.velocity;
        return Vector3.ClampMagnitude (v, settings.maxSteerForce);
    }

    private void OnDrawGizmosSelected()
    {
        DebugGizmos();
    }

    protected virtual void DebugGizmos()
    {
        // Debugging the velocity
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + velocity);

        if (_perceivedBoids > 0)
        {
            // Debugging nearby boids
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _boidsInVision.Count; ++i)
            {
                Gizmos.DrawLine(transform.position, _boidsInVision[i].transform.position);
            }

            // Debugging the cohesion force
            // Gizmos.color = Color.cyan;
            // Gizmos.DrawLine(transform.position, transform.position + _cohesionForce);
            // Gizmos.color = Color.red;
            // Gizmos.DrawLine(transform.position, transform.position + _separationForce);\
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + _obstacleForce);
    }
}
