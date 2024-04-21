using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField] private Boid boidPrefab;
    [SerializeField] private float spawnRadius = 1;
    [SerializeField] private float spawnCount = 1;

    private void Awake()
    {
        for (int i = 0; i < spawnCount; ++i)
        {
            Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
            Boid boid = Instantiate(boidPrefab);
            boid.transform.SetPositionAndRotation(randomPoint, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
