using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Moon : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public float radius = 1f;
    public int numCraters = 10;
    public float minCraterRadius = 0.1f;
    public float maxCraterRadius = 0.5f;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;
    public CraterSettings craterSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;
    [HideInInspector]
    public bool craterSettingFoldout;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    Colour_Generator colourGenerator = new Colour_Generator();
    CraterCreator craterGenerator;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    Terrain_Face[] terrainFaces;

    MoonTerrainFace[] moonTerrainFaces;

    void Initialize()
    {
        shapeGenerator.updateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);
        craterGenerator = new CraterCreator(craterSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new Terrain_Face[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new Terrain_Face(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    void CreateCraters(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int numVertices = vertices.Length;

        for (int i = 0; i < numCraters; i++)
        {
            float craterRadius = Random.Range(minCraterRadius, maxCraterRadius);
            Vector3 center = Random.onUnitSphere * radius;

            for (int j = 0; j < numVertices; j++)
            {
                Vector3 vertex = vertices[j];
                float distanceToCenter = Vector3.Distance(vertex, center);

                if (distanceToCenter < craterRadius)
                {
                    vertices[j] = vertex - (craterRadius - distanceToCenter) * vertex.normalized;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }

    public void GenerateMoon()
    {
        Initialize();
        GenerateMesh();
        GenerateMoonColours();
    }

    public void OnCratersSettingsUpdate()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMoonColours();
        }
    }

    void GenerateMesh()
    {
        foreach (Terrain_Face face in terrainFaces)
        {
            face.ConstructMesh();
        }
        colourGenerator.UpdateElevation(shapeGenerator.ElevationMinMax);
    }


    void GenerateMoonColours()
    {
        colourGenerator.UpdateColours();
    }

}
