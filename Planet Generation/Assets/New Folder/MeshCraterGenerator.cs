using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCraterGenerator : MonoBehaviour
{
    public Mesh mesh;
    public CraterSettings craterSettings;

    private MeshCraterGenerator craterGenerator;

    private void Start()
    {
        // Add the MeshCraterGenerator component to this GameObject
        craterGenerator = gameObject.AddComponent<MeshCraterGenerator>();

        // Set the mesh and crater settings
        craterGenerator.mesh = mesh;
        craterGenerator.craterSettings = craterSettings;

        // Generate the craters
        craterGenerator.GenerateCraters();

        // Update the crater settings
        craterGenerator.craterSettings.numCraters = 20;
        craterGenerator.craterSettings.minCraterRadius = 0.2f;
        craterGenerator.craterSettings.maxCraterRadius = 0.8f;
    }

    private void GenerateCraters()
    {
        for (int i = 0; i < craterSettings.numCraters; i++)
        {
            // Get a random position within the bounds of the mesh
            Vector3 center = new Vector3(
                Random.Range(mesh.bounds.min.x, mesh.bounds.max.x),
                Random.Range(mesh.bounds.min.y, mesh.bounds.max.y),
                Random.Range(mesh.bounds.min.z, mesh.bounds.max.z));

            // Get a random radius for the crater
            float radius = Random.Range(craterSettings.minCraterRadius, craterSettings.maxCraterRadius);

            // Create the crater
            CreateCrater(center, radius);
        }
    }

    private void CreateCrater(Vector3 center, float radius)
    {
        // Get the vertices of the mesh
        Vector3[] vertices = mesh.vertices;

        // Loop through the vertices and check if they're within the crater radius
        for (int i = 0; i < vertices.Length; i++)
        {
            if (Vector3.Distance(vertices[i], center) < radius)
            {
                // Calculate the depth of the crater at this vertex
                float depth = (radius - Vector3.Distance(vertices[i], center)) / radius * 0.1f;

                // Lower the vertex by the calculated depth
                vertices[i].y -= depth;
            }
        }

        // Update the mesh with the modified vertices
        mesh.vertices = vertices;

        // Recalculate the normals and bounds of the mesh
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
