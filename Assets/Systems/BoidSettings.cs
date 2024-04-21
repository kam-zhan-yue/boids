using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObject/BoidSettings")]
public class BoidSettings : ScriptableObject
{
    [Header("Boid Settings")]
    public float minSpeed;
    public float maxSpeed;
    public float maxSteerForce;
    [Range(0f, 5f)] public float visionRadius;
    [Range(0f, 360f)] public float visionAngle;
    public LayerMask obstacleLayer;
    
    [Header("Scene Settings")]
    public float width;
    public float height;
    public float depth;

    [Range(0f, 20f)] public float separationWeight = 1f;
    [Range(0f, 20f)] public float alignmentWeight = 1f;
    [Range(0f, 20f)] public float cohesionWeight = 1f;
    [Range(0f, 20f)] public float obstacleWeight = 3f;

    public bool separation = true;
    public bool alignment = true;
    public bool cohesion = true;
}