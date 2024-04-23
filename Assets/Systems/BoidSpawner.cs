using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField] private bool threeDimensions = false;
    [SerializeField] private Boid boidPrefab;
    [SerializeField] private float spawnRadius = 1;
    [SerializeField] private float spawnCount = 1;
    [SerializeField] private BoidGroup boidGroup;

    private void Awake()
    {
        for (int i = 0; i < spawnCount; ++i)
        {
            Vector3 randomPoint = default;
            if (threeDimensions)
            {
                randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
            }
            else
            {
                randomPoint = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            }
            Boid boid = Instantiate(boidPrefab);
            boid.InitGroup(boidGroup);
            boid.transform.SetPositionAndRotation(randomPoint, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
