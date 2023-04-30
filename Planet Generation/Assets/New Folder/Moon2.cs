using UnityEngine;

public class Moon2 : MonoBehaviour
{
    public float radius = 1.0f;
    public int latitudeSegments = 32;
    public int longitudeSegments = 32;
    //public CraterGenerator craterGenerator; // Reference to your crater generation script

    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[(latitudeSegments + 1) * (longitudeSegments + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[latitudeSegments * longitudeSegments * 6];

        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float theta = lat * Mathf.PI / latitudeSegments;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float phi = lon * 2 * Mathf.PI / longitudeSegments;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                float x = cosPhi * sinTheta;
                float y = cosTheta;
                float z = sinPhi * sinTheta;
                int index = lat * (longitudeSegments + 1) + lon;
                vertices[index] = new Vector3(x, y, z) * radius;
                uv[index] = new Vector2((float)lon / longitudeSegments, (float)lat / latitudeSegments);
            }
        }

        int i = 0;
        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                int index = lat * (longitudeSegments + 1) + lon;
                triangles[i++] = index;
                triangles[i++] = index + 1;
                triangles[i++] = index + longitudeSegments + 2;

                triangles[i++] = index;
                triangles[i++] = index + longitudeSegments + 2;
                triangles[i++] = index + longitudeSegments + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        /*if (craterGenerator != null)
        {
            craterGenerator.GenerateCraters(mesh); // Call the GenerateCraters method in your crater generation script
        }*/
    }
}

