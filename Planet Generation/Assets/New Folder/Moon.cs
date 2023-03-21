using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    MoonTerrainFace[] moonTerrainFaces;

    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        moonTerrainFaces = new MoonTerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector2.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            GameObject meshObj = new GameObject("mesh");
            meshObj.transform.parent = transform;

            meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            meshFilters[i] = meshObj.AddComponent<MeshFilter>();
            meshFilters[i].sharedMesh = new Mesh();

            moonTerrainFaces[i] = new MoonTerrainFace(meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    public void OnShapeSettingsUpdate()
    {
        Initialize();
        GenerateMesh();
    }

    void GenerateMesh()
    {
        foreach (MoonTerrainFace face in moonTerrainFaces)
        {
            face.ConstructMesh();
        }
    }
}