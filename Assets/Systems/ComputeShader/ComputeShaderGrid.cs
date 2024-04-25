using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public struct Cube
{
    public Vector3 position;
    public Color colour;
}

public class ComputeShaderGrid : MonoBehaviour
{
    public ComputeShader computeShader;
    public Mesh mesh;
    public Material material;
    public int count;
    public int repetitions;
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private GameObject[] _objects = Array.Empty<GameObject>();
    private Cube[] _cubes = Array.Empty<Cube>();

    private void Start()
    {
        _cubes = new Cube[count * count];
        _objects = new GameObject[count * count];
        for (int i = 0; i < count; ++i)
        {
            for (int j = 0; j < count; ++j)
            {
                GameObject cube = new GameObject();
                MeshFilter cubeFilter = cube.AddComponent<MeshFilter>();
                MeshRenderer cubeRenderer = cube.AddComponent<MeshRenderer>();
                Color colour = Random.ColorHSV();
                cubeFilter.mesh = mesh;
                cubeRenderer.material = material;
                cubeRenderer.material.SetColor(Color1, colour);
                cube.transform.position = new Vector3(i, j, Random.Range(-0.1f, 0.1f));
                
                Cube cubeData = new Cube();
                cubeData.position = cube.transform.position;
                cubeData.colour = colour;
                _cubes[i * count + j] = cubeData;
                _objects[i * count + j] = cube;
            }
        }
    }

    public void RandomiseGPU()
    {
        int colourSize = sizeof(float) * 4;
        int vector3Size = sizeof(float) * 3;
        int totalSize = colourSize + vector3Size;
        ComputeBuffer cubesBuffer = new ComputeBuffer(_cubes.Length, totalSize);
        cubesBuffer.SetData(_cubes);
        
        computeShader.SetBuffer(0, "cubes", cubesBuffer);
        computeShader.SetFloat("resolution", _cubes.Length);
        computeShader.SetFloat("repetitions", repetitions);
        // Divide by 10 because there are 10 threads
        computeShader.Dispatch(0, _cubes.Length / 10, 1, 1);
        
        cubesBuffer.GetData(_cubes);
        
        for (int i = 0; i < _objects.Length; ++i)
        {
            GameObject obj = _objects[i];
            Cube cube = _cubes[i];
            obj.transform.position = cube.position;
            obj.GetComponent<MeshRenderer>().material.SetColor(Color1, cube.colour);
        }
        
        cubesBuffer.Dispose();
    }

    public void RandomiseCPU()
    {
        for (int i = 0; i < repetitions; ++i)
        {
            for (int j = 0; j < _objects.Length; ++j)
            {
                GameObject obj = _objects[j];
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, Random.Range(-0.1f, 0.1f));
                obj.GetComponent<MeshRenderer>().material.SetColor(Color1, Random.ColorHSV());
            }
        }
    }
}