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
        // Check which boid can be seen
        // Separation
        // Alignment
        // Cohesion
        List<Boid> nearbyBoids = GetNearbyBoids(boid);
        boid.DebugVision(nearbyBoids);
        boid.Simulate();
    }

    private List<Boid> GetNearbyBoids(Boid boid)
    {
        List<Boid> nearby = new List<Boid>();
        Vector3 boidPosition = boid.transform.position;
        for (int i = 0; i < _boids.Length; ++i)
        {
            if (_boids[i] == boid)
                continue;
            if (boid.CanSee(_boids[i].transform.position))
            {
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
