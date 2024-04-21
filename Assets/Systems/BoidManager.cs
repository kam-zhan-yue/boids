using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] private BoidSettings boidSettings;
    private Boid[] _boids = Array.Empty<Boid>();

    private void Start()
    {
        _boids = FindObjectsOfType<Boid>();
        for (int i = 0; i < _boids.Length; ++i)
        {
            _boids[i].Init(boidSettings);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _boids.Length; ++i)
        {
            Bound(_boids[i]);
            SimulateBoid(_boids[i]);
        }
    }

    private void SimulateBoid(Boid boid)
    {
        // Get all nearby boids
        List<Boid> nearbyBoids = GetNearbyBoids(boid);
        boid.SetPerceivedBoids(nearbyBoids.Count);
        boid.DebugVision(nearbyBoids);
        // Apply forces
        SimulateForces(boid, nearbyBoids);
        // Simulate the boid
        boid.Simulate();
    }

    private void SimulateForces(Boid boid, List<Boid> nearbyBoids)
    {
        Vector3 separationForce = Vector3.zero;
        Vector3 alignmentForce = Vector3.zero;
        Vector3 cohesionForce = Vector3.zero;
        for (int i = 0; i < nearbyBoids.Count; ++i)
        {
            if (boid == nearbyBoids[i])
                continue;

            if (boidSettings.separation)
                separationForce -= GetSeparationForce(boid, nearbyBoids[i]);
            if (boidSettings.alignment)
                alignmentForce += GetAlignmentForce(boid, nearbyBoids[i]);
            if (boidSettings.cohesion)
                cohesionForce += GetCohesionForce(boid, nearbyBoids[i]);
        }
        boid.SetSeparation(separationForce);
        boid.SetAlignment(alignmentForce);
        boid.SetCohesion(cohesionForce);
    }

    private Vector3 GetSeparationForce(Boid boid1, Boid boid2)
    {
        // Get the difference between boid1 and boid2
        Vector3 difference = boid2.transform.position - boid1.transform.position;
        // Calculate a ratio based on relative distance in regards to the vision radius
        float ratio = Mathf.Clamp01(difference.magnitude / boidSettings.visionRadius); 
        return ratio * difference;
    }

    private Vector3 GetAlignmentForce(Boid boid1, Boid boid2)
    {
        // Get the ratio between the relative distances
        Vector3 difference = boid2.transform.position - boid1.transform.position;
        float ratio = Mathf.Clamp01(difference.magnitude / boidSettings.visionRadius);
        // Return the direction of the second boid adjusted by the ratio
        return ratio * boid2.Direction;
    }

    private Vector3 GetCohesionForce(Boid boid1, Boid boid2)
    {
        return Vector3.zero;
    }
    
    private List<Boid> GetNearbyBoids(Boid boid)
    {
        List<Boid> nearby = new List<Boid>();
        for (int i = 0; i < _boids.Length; ++i)
        {
            if (_boids[i] == boid)
                continue;
            if (boid.CanSee(_boids[i].transform.position))
            {
                // Add separation force
                nearby.Add(_boids[i]);
            }
        }

        return nearby;
    }

    private void Bound(Boid boid)
    {
        Vector3 boidPosition = boid.transform.position;
        Vector3 managerPosition = transform.position;
        Bounds bounds = new Bounds(managerPosition, new Vector3(boidSettings.width, boidSettings.height, 0f));
        if (boidPosition.x > bounds.max.x)
            boidPosition.x = bounds.min.x;
        else if (boidPosition.x < bounds.min.x)
            boidPosition.x = bounds.max.x;
        else if (boidPosition.y > bounds.max.y)
            boidPosition.y = bounds.min.y;
        else if (boidPosition.y < bounds.min.y)
            boidPosition.y = bounds.max.y;
        boid.transform.position = boidPosition;
    }

    private void OnDrawGizmos()
    {
        if (boidSettings == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(boidSettings.width, boidSettings.height, 0f));
    }
}
