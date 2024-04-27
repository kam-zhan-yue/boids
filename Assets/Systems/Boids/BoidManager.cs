using System;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] private BoidSettings boidSettings;
    [SerializeField] private ComputeShader boidComputeShader;
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
        if (boidSettings.gpu)
            SimulateBoidGPU();
        else
            SimulateBoidCPU();
    }

    private void SimulateBoidGPU()
    {
        // Assign boids into boid data, getting ready for compute shader
        BoidData[] boidData = new BoidData[_boids.Length];
        for (int i = 0; i < _boids.Length; ++i)
        {
            boidData[i].position = _boids[i].transform.position;
            boidData[i].direction = _boids[i].Direction;
            boidData[i].predator = _boids[i].Predator ? (uint)1 : 0;
            boidData[i].groupId = _boids[i].GroupID;
        }
        
        // Create a compute buffer for the boid data
        ComputeBuffer boidsBuffer = new ComputeBuffer(boidData.Length, BoidData.GetStrideLength());
        boidsBuffer.SetData(boidData);
        
        // Set the compute shader variables
        boidComputeShader.SetBuffer(0, "boids", boidsBuffer);
        boidComputeShader.SetFloat("visionRadius", boidSettings.visionRadius);
        boidComputeShader.SetFloat("visionAngle", boidSettings.visionAngle);
        boidComputeShader.SetInt("boidNum", boidData.Length);
        // Dispatch the compute shader
        boidComputeShader.Dispatch(0, boidData.Length, 1, 1);
        
        // Retrieve the data from the compute shader
        boidsBuffer.GetData(boidData);
        
        // Loop through the boid data and set the boid variables
        for (int i = 0; i < _boids.Length; ++i)
        {
            if(boidSettings.separation) _boids[i].SetSeparation(boidData[i].separationForce);
            if(boidSettings.alignment) _boids[i].SetAlignment(boidData[i].alignmentForce);
            if(boidSettings.cohesion) _boids[i].SetCohesion(boidData[i].cohesionForce);
            if(boidSettings.avoidance) _boids[i].SetAvoidance(boidData[i].avoidanceForce);
            _boids[i].Simulate();
            if (boidSettings.infinite)
                Bound(_boids[i]);
        }
        
        boidsBuffer.Release();
    }

    private Vector3 ReadjustCohesion(Vector3 totalCohesionForce, Vector3 originalPosition, int nearbyBoids)
    {
        if (!boidSettings.cohesion) return Vector3.zero;
        // Divide the cohesion force by the number of nearby boids
        return totalCohesionForce / nearbyBoids - originalPosition;
    }

    private void SimulateBoidCPU()
    {
        for (int i = 0; i < _boids.Length; ++i)
        {
            SimulateBoid(_boids[i]);
            if(boidSettings.infinite) 
                Bound(_boids[i]);
        }
    }

    private void SimulateBoid(Boid boid)
    {
        // Get all nearby boids
        List<Boid> nearbyBoids = GetNearbyBoids(boid);
        // Apply forces
        SimulateForces(boid, nearbyBoids);
        // Simulate the boid
        boid.Simulate();
    }

    private void SimulateForces(Boid boid, List<Boid> nearbyBoids)
    {
        if (nearbyBoids.Count == 0) return;
        
        Vector3 separationForce = Vector3.zero;
        Vector3 alignmentForce = Vector3.zero;
        Vector3 totalCohesionForce = Vector3.zero;
        Vector3 avoidanceForce = Vector3.zero;
        for (int i = 0; i < nearbyBoids.Count; ++i)
        {
            if (boid == nearbyBoids[i])
                continue;

            if (SameGroup(boid, nearbyBoids[i]))
            {
                if (boidSettings.separation) separationForce -= GetSeparationForce(boid, nearbyBoids[i]);
                if (boidSettings.alignment) alignmentForce += GetAlignmentForce(boid, nearbyBoids[i]);
                if (boidSettings.cohesion) totalCohesionForce += GetCohesionForce(boid, nearbyBoids[i]);
            }
            if (boidSettings.avoidance)
                avoidanceForce += GetAvoidanceForce(boid, nearbyBoids[i]);
        }

        Vector3 cohesionForce = ReadjustCohesion(totalCohesionForce, boid.transform.position, nearbyBoids.Count);
        boid.SetSeparation(separationForce);
        boid.SetAlignment(alignmentForce);
        boid.SetCohesion(cohesionForce);
        boid.SetAvoidance(avoidanceForce);
    }

    private bool SameGroup(Boid boid1, Boid boid2)
    {
        return boid1.GroupID == boid2.GroupID;
    }

    private Vector3 GetSeparationForce(Boid boid1, Boid boid2)
    {
        Vector3 difference = boid2.transform.position - boid1.transform.position;
        return difference.normalized / difference.sqrMagnitude;
    }

    private Vector3 GetAlignmentForce(Boid boid1, Boid boid2)
    {
        return boid2.Direction;
    }

    private Vector3 GetCohesionForce(Boid boid1, Boid boid2)
    {
        return boid2.transform.position;
    }

    private Vector3 GetAvoidanceForce(Boid boid1, Boid boid2)
    {
        // Ignore if boid2 is not a predator or if boid1 is a predator
        if (!boid2.Predator || boid1.Predator)
            return Vector3.zero;
        // Get direction away from boid2
        Vector3 difference = boid1.transform.position - boid2.transform.position;
        return difference.normalized / difference.sqrMagnitude;
    }
    
    private List<Boid> GetNearbyBoids(Boid boid)
    {
        List<Boid> nearby = new List<Boid>();
        for (int i = 0; i < _boids.Length; ++i)
        {   
            if (_boids[i] == boid)
                continue;
            bool canSee = boid.CanSee(_boids[i].transform.position);
            Debug.Log($"Can See: {canSee}");
            if (canSee)
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
        Bounds bounds = new Bounds(managerPosition, new Vector3(boidSettings.width, boidSettings.height, boidSettings.depth));
        
        if (boidPosition.x > bounds.max.x)
            boidPosition.x = bounds.min.x;
        else if (boidPosition.x < bounds.min.x)
            boidPosition.x = bounds.max.x;
        
        if (boidPosition.y > bounds.max.y)
            boidPosition.y = bounds.min.y;
        else if (boidPosition.y < bounds.min.y)
            boidPosition.y = bounds.max.y;
        
        if (boidPosition.z < bounds.min.z)
            boidPosition.z = bounds.max.z;
        else if (boidPosition.z > bounds.max.z)
            boidPosition.z = bounds.min.z;

        boid.transform.position = boidPosition;
    }

    private void OnDrawGizmos()
    {
        if (boidSettings == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(boidSettings.width, boidSettings.height, boidSettings.depth));
    }
}
