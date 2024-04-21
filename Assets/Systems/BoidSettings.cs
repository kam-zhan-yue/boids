using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/BoidSettings")]
public class BoidSettings : ScriptableObject
{
    [Header("Boid Settings")]
    public float speed;
    public float visionRadius;
    [Range(0f, 360f)]
    public float visionAngle;
    [Header("Scene Settings")]
    public float width;
    public float height;
}