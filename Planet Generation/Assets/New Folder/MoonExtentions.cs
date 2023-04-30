using UnityEngine;

public static class MoonExtentions
{
    public static int[] GetVerticesWithinDistance(this Mesh mesh, Vector3 point, float distance)
    {
        int[] vertices = mesh.triangles;
        Vector3[] meshVertices = mesh.vertices;
        int[] affectedVertices = new int[vertices.Length];

        int index = 0;
        for (int i = 0; i < vertices.Length; i += 3)
        {
            Vector3 v1 = meshVertices[vertices[i]];
            Vector3 v2 = meshVertices[vertices[i + 1]];
            Vector3 v3 = meshVertices[vertices[i + 2]];

            Vector3 center = (v1 + v2 + v3) / 3;
            float dist = Vector3.Distance(center, point);

            if (dist < distance)
            {
                affectedVertices[index++] = vertices[i];
                affectedVertices[index++] = vertices[i + 1];
                affectedVertices[index++] = vertices[i + 2];
            }
        }

        int[] result = new int[index];
        for (int i = 0; i < index; i++)
        {
            result[i] = affectedVertices[i];
        }

        return result;
    }
}
