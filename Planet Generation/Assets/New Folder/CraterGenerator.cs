using UnityEngine;

public class CraterGenerator : MonoBehaviour
{
    public int numCraters = 10;
    public float minCraterSize = 0.1f;
    public float maxCraterSize = 0.5f;
    public float minCraterDepth = 0.1f;
    public float maxCraterDepth = 0.5f;
    public float noiseScale = 1.0f;
    public float noiseThreshold = 0.2f;

    public void GenerateCraters(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        for (int i = 0; i < numCraters; i++)
        {
            Vector3 position = vertices[Random.Range(0, vertices.Length)];
            float size = Random.Range(minCraterSize, maxCraterSize);
            float depth = Random.Range(minCraterDepth, maxCraterDepth);

            for (int j = 0; j < vertices.Length; j++)
            {
                Vector3 vertex = vertices[j];
                float distance = Vector3.Distance(vertex, position);

                if (distance <= size)
                {
                    float delta = 1.0f - (distance / size);

                    // Use Perlin noise to determine how deep the crater should be
                    float noiseValue = Mathf.PerlinNoise((vertex.x + position.x) * noiseScale, (vertex.z + position.z) * noiseScale);
                    if (noiseValue > noiseThreshold)
                    {
                        delta = Mathf.Pow(delta, depth);
                        vertices[j] = Vector3.Lerp(vertex, position, delta);
                    }
                }
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}

