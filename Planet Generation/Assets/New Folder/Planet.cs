using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;

    public Shape_Settings shapesettings;
    public Colour_Settings coloursettings;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    Terrain_Face[] terrainFaces;

    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0) { 
            meshFilters = new MeshFilter[6]; }
        terrainFaces = new Terrain_Face[6];

        Vector3[] directions = { Vector3.up, Vector2.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            GameObject meshObj = new GameObject("mesh");
            meshObj.transform.parent = transform;

            meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            meshFilters[i] = meshObj.AddComponent<MeshFilter>();
            meshFilters[i].sharedMesh = new Mesh();

            terrainFaces[i] = new Terrain_Face(meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    public void OnShapeSettingsUpdate()
    {
        Initialize();
        GenerateMesh();
    }

    public void OnColourSettingsUpdate()
    {
        Initialize();
        GenerateColours();
    }

    void GenerateMesh()
    {
        foreach (Terrain_Face face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    void GenerateColours()
    {
        foreach (MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = coloursettings.planetColour;
        }
    }
}
