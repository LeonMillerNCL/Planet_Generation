using UnityEngine;

public class MoonScript : MonoBehaviour
{
    public int numPoints = 10; // Number of Worley points to generate
    public float pointRadius = 0.1f; // Radius of the Worley points
    public float frequency = 1f; // Frequency of the Worley noise
    public float amplitude = 1f; // Amplitude of the Worley noise

    private MeshRenderer meshRenderer; // Reference to the sphere's mesh renderer
    private MeshFilter meshFilter; // Reference to the sphere's mesh filter
    private Mesh mesh; // Reference to the sphere's mesh

    void Start()
    {
        // Get references to the sphere's components
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        // Generate Worley points
        Vector3[] points = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            points[i] = new Vector3(Random.value, Random.value, Random.value);
        }

        // Generate Worley noise
        Color[] colors = new Color[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 p = mesh.vertices[i];
            float noiseValue = WorleyNoise(p.x * frequency, p.y * frequency, p.z * frequency, points) * amplitude;
        }

        // Apply colors to the sphere's mesh
        meshRenderer.materials[0].color = Color.white; // Set base color
    }

    float WorleyNoise(float x, float y, float z, Vector3[] points)
    {
        float minDist = Mathf.Infinity;
        for (int i = 0; i < points.Length; i++)
        {
            float dist = Vector3.Distance(new Vector3(x, y, z), points[i]);
            if (dist < minDist)
            {
                minDist = dist;
            }
        }
        return minDist;
    }
}


