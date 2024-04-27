using UnityEngine;

public struct BoidData
{
    public Vector3 position;
    public Vector3 direction;
    public Vector3 separationForce;
    public Vector3 alignmentForce;
    public Vector3 cohesionForce;
    public Vector3 avoidanceForce;
    public int groupId;
    public uint predator;

    public static int GetStrideLength()
    {
        return sizeof (float) * 3 * 6 + sizeof (int) + sizeof(uint);
    }
}