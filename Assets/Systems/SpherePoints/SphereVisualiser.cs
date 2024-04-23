using System;
using System.Collections.Generic;
using UnityEngine;

public class SphereVisualiser : MonoBehaviour
{
    [SerializeField] private SpherePoint pointPrefab;
    [SerializeField, Range(0, 5f)] private float ratio = BoidExtensions.GoldenRatio;
    [SerializeField, Range(0, 500)] private int directions = 300;
    [SerializeField, Range(0, 10f)] private float radius = 2f;

    private List<SpherePoint> _points = new List<SpherePoint>();
    private Vector3[] _directions = Array.Empty<Vector3>();

    private void Awake()
    {
        EnsureBounds();
    }

    private void Update()
    {
        if (pointPrefab)
        {
            EnsureBounds();
            CalculateDirections();
            SetDirections();
        }
    }
    
    private void EnsureBounds()
    {
        if (_points.Count > directions)
        {
            int numToRemove = _points.Count - directions;
            for (int i = 0; i < numToRemove; ++i)
            {
                int index = _points.Count - (i + 1);
                Destroy(_points[index].gameObject);
                _points.RemoveAt(index);
            }
        }
        else if(_points.Count < directions)
        {
            int numToSpawn = directions - _points.Count;
            for (int i = 0; i < numToSpawn; ++i)
            {
                SpherePoint point = Instantiate(pointPrefab);
                _points.Add(point);
            }
        }
    }

    private void CalculateDirections()
    {
        _directions = new Vector3[directions];
        BoidExtensions.CalculateSpherePoints(_directions, ratio);
    }

    private void SetDirections()
    {
        for (int i = 0; i < directions; ++i)
        {
            _points[i].transform.position = _directions[i] * radius;
        }
    }
}
