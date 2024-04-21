using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/BoidSettings")]
public class BoidSettings : ScriptableObject
{
    [Header("Boid Settings")]
    public float minSpeed;
    public float maxSpeed;
    public float maxSteerForce;
    public float visionRadius;
    [Range(0f, 360f)]
    public float visionAngle;
    [Header("Scene Settings")]
    public float width;
    public float height;

    public bool separation = true;
    public bool alignment = true;
    public bool cohesion = true;
}