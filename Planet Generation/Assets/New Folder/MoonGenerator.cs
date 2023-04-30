using UnityEngine;

public class MoonGenerator : MonoBehaviour
{
    [SerializeField] private int _resolution = 50;
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _amplitude = 2f;
    [SerializeField] private float _frequency = 1f;
    [SerializeField] private int _seed = 0;

    private MeshFilter _meshFilter;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        GenerateMoon();
    }

    private void GenerateMoon()
    {
        // Create a new mesh and mesh renderer
        Mesh mesh = new Mesh();
        mesh.name = "Moon Mesh";
        _meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));

        // Generate vertices and triangles for the sphere
        Vector3[] vertices = new Vector3[_resolution * _resolution];
        int[] triangles = new int[(_resolution - 1) * (_resolution - 1) * 6];

        int index = 0;
        for (int i = 0; i < _resolution; i++)
        {
            for (int j = 0; j < _resolution; j++)
            {
                float u = (float)i / (_resolution - 1);
                float v = (float)j / (_resolution - 1);

                float x = _radius * Mathf.Sin(u * Mathf.PI) * Mathf.Cos(v * Mathf.PI * 2);
                float y = _radius * Mathf.Sin(u * Mathf.PI) * Mathf.Sin(v * Mathf.PI * 2);
                float z = _radius * Mathf.Cos(u * Mathf.PI);

                vertices[index] = new Vector3(x, y, z);
                index++;
            }
        }

        index = 0;
        for (int i = 0; i < _resolution - 1; i++)
        {
            for (int j = 0; j < _resolution - 1; j++)
            {
                int a = i * _resolution + j;
                int b = i * _resolution + j + 1;
                int c = (i + 1) * _resolution + j;
                int d = (i + 1) * _resolution + j + 1;

                triangles[index++] = a;
                triangles[index++] = c;
                triangles[index++] = b;

                triangles[index++] = b;
                triangles[index++] = c;
                triangles[index++] = d;
            }
        }

        // Set the mesh vertices and triangles
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Add Perlin noise to the vertices to create craters
        float[] noiseValues = PerlinNoise.GenerateNoiseMap(_resolution, _resolution, _seed, _frequency, _amplitude);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= 1f + noiseValues[i];
        }

        // Recalculate the mesh normals and tangents for lighting
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    // This method is called when the button in the inspector is clicked
    public void CreateMoonWithCraters()
    {
        GenerateMoon();
    }
}

// PerlinNoise class for generating Perlin noise
public static class PerlinNoise
{
    public static float[] GenerateNoiseMap(int width, int height, int seed, float frequency, float amplitude)
    {
        float[] noiseValues = new float[width * height];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[8];

        for (int i = 0; i < 8; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (frequency <= 0)
        {
            frequency = 0.0001f;
        }

        float maxNoiseValue = float.MinValue;
        float minNoiseValue = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitudeSum = 0f;
                float frequencySum = 0f;

                for (int i = 0; i < octaveOffsets.Length; i++)
                {
                    float sampleX = (x - halfWidth) / frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / frequency + octaveOffsets[i].y;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;

                    amplitudeSum += perlinValue * amplitude;
                    frequencySum += amplitude;
                }

                float noiseValue = amplitudeSum / frequencySum;
                noiseValues[y * width + x] = noiseValue;

                if (noiseValue > maxNoiseValue)
                {
                    maxNoiseValue = noiseValue;
                }
                else if (noiseValue < minNoiseValue)
                {
                    minNoiseValue = noiseValue;
                }
            }
        }

        for (int i = 0; i < noiseValues.Length; i++)
        {
            noiseValues[i] = Mathf.InverseLerp(minNoiseValue, maxNoiseValue, noiseValues[i]);
        }

        return noiseValues;
    }
}



/*{
    public float radius = 1f;
    public int resolution = 64;
    public float noiseScale = 1f;
    public float noiseThreshold = 0.5f;
    public int numCraters = 10;
    public float craterDepth = 0.2f;
    float craterScale = 1f;
    GameObject sphere;

    int vertexCount = 0;
    float vertexDistanceTolerance = 0.1f;
    public int subdivisions = 3;

    public Mesh mesh;

    private void Awake()
    {
        mesh = new Mesh();
    }
    public void CreateMoon()
    {
        // Clear the previous mesh, if any
        mesh.Clear();

        // Create the vertices
        vertexCount = (subdivisions + 1) * (subdivisions + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        for (int i = 0, y = 0; y <= subdivisions; y++)
        {
            for (int x = 0; x <= subdivisions; x++, i++)
            {
                Vector2 percent = new Vector2(x, y) / subdivisions;
                Vector3 pointOnUnitSphere = GetPointOnUnitSphere(percent.x, percent.y);

                float noiseValue = Mathf.PerlinNoise(percent.x * noiseScale, percent.y * noiseScale);
                float height = noiseValue > noiseThreshold ? 1f : 0f;

                height = Mathf.Clamp01(height - craterDepth);

                float craterScale = 10f;
                float craterThreshold = 0.6f / ((y * 1f / subdivisions) + 0.2f);
                CreateCrater(vertices, ref triIndex, i * (subdivisions + 1) + j, percent, pointOnUnitSphere, radius, craterThreshold, craterDepth, craterScale);

                vertices[i] = pointOnUnitSphere * (radius + height);
            }
        }

        // Create the triangles
        int[] triangles = new int[subdivisions * subdivisions * 6];
        int triangleIndex = 0;
        for (int i = 0, y = 0; y < subdivisions; y++, i++)
        {
            for (int x = 0; x < subdivisions; x++, i++)
            {
                int topLeft = i;
                int topRight = i + 1;
                int bottomLeft = i + subdivisions + 1;
                int bottomRight = i + subdivisions + 2;

                triangles[triangleIndex] = topLeft;
                triangles[triangleIndex + 1] = bottomLeft;
                triangles[triangleIndex + 2] = topRight;
                triangles[triangleIndex + 3] = topRight;
                triangles[triangleIndex + 4] = bottomLeft;
                triangles[triangleIndex + 5] = bottomRight;

                triangleIndex += 6;
            }
        }

        // Assign the vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalculate bounds and normals
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    void CreateCrater(Vector3[] vertices, ref int triIndex, int index, Vector2 percent, Vector3 pointOnUnitSphere, float radius, float craterThreshold, float craterDepth, float craterScale)
    {
        float craterNoise = Mathf.PerlinNoise(percent.x * craterScale, percent.y * craterScale);
        if (craterNoise > craterThreshold)
        {
            Vector3 craterCenter = pointOnUnitSphere * (1f + craterDepth);
            float craterRadius = UnityEngine.Random.Range(0.05f, 0.1f) * radius;

            for (int i = 0; i < 5; i++)
            {
                if (index + i >= vertices.Length)
                {
                    Debug.LogError("Index out of range: " + (index + i) + " >= " + vertices.Length);
                    break;
                }

                float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
                float distance = UnityEngine.Random.Range(0f, craterRadius);
                Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
                Vector3 vertexPosition = craterCenter + offset;

                // Create a triangle fan for the crater
                int nextIndex = (index + i + 1) % 5;
                int nextVertexIndex = index + nextIndex;
                int[] craterTriangles = new int[] { index + i, index + nextIndex, vertices.Length - 1 };
                mesh.triangles[triIndex] = craterTriangles[0];
                mesh.triangles[triIndex + 1] = craterTriangles[1];
                mesh.triangles[triIndex + 2] = craterTriangles[2];
                triIndex += 3;

                // Update the vertex position for the current vertex
                vertices[index + i] = vertexPosition;

                // Update the vertex position for the next vertex to create the triangle fan
                vertices[nextVertexIndex] = craterCenter;
            }

            // Update the last vertex position to close the triangle fan
            vertices[index + 4] = vertices[index];
            triIndex -= 3; // Remove the last 3 indices since they form a degenerate triangle

            // Increase the vertex count and update the mesh
            vertexCount += 5;
            mesh.vertices = vertices;
            mesh.triangles = mesh.GetTriangles(0);
        }
    }




    Vector3 GetPointOnUnitSphere(float u, float v)
    {
        float latitude = Mathf.Acos(2f * v - 1f) - (Mathf.PI / 2f);
        float longitude = 2f * Mathf.PI * u;
        Vector3 point = new Vector3(Mathf.Cos(latitude) * Mathf.Cos(longitude), Mathf.Cos(latitude) * Mathf.Sin(longitude), Mathf.Sin(latitude));
        return point;
    }

    void Update()
    {
        CreateMoon();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MoonGenerator))]
    public class SphereCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MoonGenerator creator = (MoonGenerator)target;
            if (GUILayout.Button("Create Moon"))
            {
                creator.CreateMoon();
            }
        }
    }
#endif
}

  public int numberOfPoints = 100; // The number of points on the sphere
  public float radius = 1f; // The radius of the sphere
  public float noiseScale = 1f; // The scale of the noise
  public float noiseStrength = 1f; // The strength of the noise

  private List<Vector3> points = new List<Vector3>();
  private Mesh mesh;

  void Start()
  {
      GenerateFibonacciSphere();
  }

  void GenerateFibonacciSphere()
  {
      float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
      float angleIncrement = Mathf.PI * 2 * goldenRatio;
      float phiIncrement = 1.0f / numberOfPoints;

      // Calculate points for the sphere
      List<Vector3> points = new List<Vector3>();
      float phi = goldenRatio * Mathf.PI;
      for (int i = 0; i < numberOfPoints; i++)
      {
          float theta = 2 * Mathf.PI * i / goldenRatio;
          float radius = Mathf.Sqrt(1 - Mathf.Pow((i / goldenRatio), 2));
          float x = radius * Mathf.Cos(theta);
          float y = radius * Mathf.Sin(theta);
          float z = (i / goldenRatio);

          points.Add(new Vector3(x, y, z));
      }

      // Create the mesh
      mesh = new Mesh();
      mesh.name = "Fibonacci Sphere";
      mesh.vertices = points.ToArray();

      // Generate triangles for the mesh
      int[] triangles = new int[(numberOfPoints - 2) * 3];
      int index = 0;
      for (int i = 0; i < numberOfPoints - 2; i++)
      {
          triangles[index++] = 0;
          triangles[index++] = i + 1;
          triangles[index++] = i + 2;
      }

      mesh.triangles = triangles;

  // Apply noise to the vertices
  Vector3[] vertices = mesh.vertices;
      for (int i = 0; i < vertices.Length; i++)
      {
          float noiseValue = Mathf.PerlinNoise(vertices[i].x * noiseScale, vertices[i].y * noiseScale);
          vertices[i] += vertices[i].normalized * noiseValue * noiseStrength;
      }
      mesh.vertices = vertices;

      // Create the mesh renderer
      MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
      meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

      // Create the mesh filter and assign the mesh
      MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
      meshFilter.mesh = mesh;
  }
}
{
public float radius = 1f;
public int latSegments = 24;
public int longSegments = 48;
public float craterThreshold = 0.5f;

private Vector3[] vertices;
private int[] triangles;
private Vector2[] uvs;

private Button generateButton;

private void Start()
{
   GenerateMesh();
   float[] noiseMap = GenerateNoiseMap();
   GenerateCraters(noiseMap, craterThreshold);
   Debug.Log("Generate");
}

private void GenerateMesh()
{
   Mesh mesh = new Mesh();
   GetComponent<MeshFilter>().mesh = mesh;

   // Calculate the number of vertices, triangles, and UVs needed
   int numVerts = (latSegments + 1) * (longSegments + 1);
   int numTris = latSegments * longSegments * 2;
   int numUvs = numVerts;

   // Initialize the arrays to the correct size
   vertices = new Vector3[numVerts];
   triangles = new int[numTris * 3];
   uvs = new Vector2[numUvs];

   // Calculate the angle between latitude and longitude segments
   float latStep = Mathf.PI / latSegments;
   float longStep = 2f * Mathf.PI / longSegments;

   // Loop through each latitude and longitude segment to create the vertices
   int vertIndex = 0;
   for (int lat = 0; lat <= latSegments; lat++)
   {
       for (int lon = 0; lon <= longSegments; lon++)
       {
           // Calculate the spherical coordinates of the vertex
           float x = radius * Mathf.Sin(lat * latStep) * Mathf.Cos(lon * longStep);
           float y = radius * Mathf.Sin(lat * latStep) * Mathf.Sin(lon * longStep);
           float z = radius * Mathf.Cos(lat * latStep);

           // Create a new vertex at the calculated position
           vertices[vertIndex] = new Vector3(x, y, z);

           // Calculate the UV coordinates for the vertex
           uvs[vertIndex] = new Vector2((float)lon / longSegments, (float)lat / latSegments);

           // Move to the next vertex index
           vertIndex++;
       }
   }

   // Loop through each latitude and longitude segment to create the triangles
   int triIndex = 0;
   for (int lat = 0; lat < latSegments; lat++)
   {
       for (int lon = 0; lon < longSegments; lon++)
       {
           // Calculate the indices of the four vertices of the current quad
           int v1 = lat * (longSegments + 1) + lon;
           int v2 = v1 + 1;
           int v3 = (lat + 1) * (longSegments + 1) + lon;
           int v4 = v3 + 1;

           // Create two triangles from the quad
           triangles[triIndex] = v1;
           triangles[triIndex + 1] = v3;
           triangles[triIndex + 2] = v2;
           triangles[triIndex + 3] = v2;
           triangles[triIndex + 4] = v3;
           triangles[triIndex + 5] = v4;

           // Move to the next triangle index
           triIndex += 6;
       }
   }

   // Assign the arrays to the mesh
   mesh.vertices = vertices;
   mesh.triangles = triangles;
   mesh.uv = uvs;

   // Recalculate the normals and bounds of the mesh
   mesh.RecalculateNormals();
   mesh.RecalculateBounds();
   Debug.Log("Create Mesh");
}
private void GenerateCraters(float[] noiseMap, float craterThreshold)
{
   // Loop through each vertex in the mesh
   for (int i = 0; i < vertices.Length; i++)
   {
       // Calculate the height of the current vertex
       float height = vertices[i].magnitude;

       // Calculate the noise value at the current vertex
       float noiseValue = noiseMap[i];

       // Check if the noise value is greater than the crater threshold
       if (noiseValue > craterThreshold)
       {
           // Scale the height of the vertex by the noise value
           float scale = (noiseValue - craterThreshold) * 2f;
           vertices[i] = vertices[i].normalized * (height - scale);
       }
   }

   // Assign the modified vertices to the mesh
   GetComponent<MeshFilter>().mesh.vertices = vertices;

   // Recalculate the normals and bounds of the mesh
   GetComponent<MeshFilter>().mesh.RecalculateNormals();
   GetComponent<MeshFilter>().mesh.RecalculateBounds();
   Debug.Log("Create Craters");
}

private float[] GenerateNoiseMap()
{
   // Create an array to hold the noise values for each vertex
   float[] noiseMap = new float[vertices.Length];

   // Loop through each vertex in the mesh
   for (int i = 0; i < vertices.Length; i++)
   {
       // Calculate the noise value at the current vertex using Perlin noise
       float x = vertices[i].x * 0.01f;
       float y = vertices[i].y * 0.01f;
       float z = vertices[i].z * 0.01f;
       noiseMap[i] = Mathf.PerlinNoise(x, y) * Mathf.PerlinNoise(y, z) * Mathf.PerlinNoise(z, x);
       Debug.Log("GenerateNoiseMap");
   }

   // Return the noise map
   return noiseMap;
}

}*/

