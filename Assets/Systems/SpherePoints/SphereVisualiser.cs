using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

public class SphereVisualiser : MonoBehaviour
{
    [SerializeField] private SpherePoint pointPrefab;
    [SerializeField, Range(0, 500)] private int directions = 300;
    [SerializeField, Range(0, 5f)] private float ratio = BoidExtensions.GoldenRatio;
    [SerializeField, Range(0, 10f)] private float radius = 2f;
    
    private SpherePoint[] _points = Array.Empty<SpherePoint>();
    private Vector3[] _directions = Array.Empty<Vector3>();

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
        if (_points.Length != directions)
        {
            for (int i = 0; i < _points.Length; ++i)
            {
                DestroyImmediate(_points[i].gameObject);
            }
            _points = new SpherePoint[directions];
            for (int i = 0; i < _points.Length; ++i)
            {
                _points[i] = Instantiate(pointPrefab);
            }
        }
    }

    private void CalculateDirections()
    {
        float angleIncrement = Mathf.PI * 2 * ratio;

        _directions = new Vector3[directions];
        for (int i = 0; i < directions; i++) {
            float t = (float) i / directions;
            float inclination = Mathf.Acos (1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin (inclination) * Mathf.Cos (azimuth);
            float y = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
            float z = Mathf.Cos (inclination);
            _directions[i] = new Vector3 (x, y, z);
        }
    }

    private void SetDirections()
    {
        for (int i = 0; i < directions; ++i)
        {
            _points[i].transform.position = _directions[i] * radius;
        }
    }
}
