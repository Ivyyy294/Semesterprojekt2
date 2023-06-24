using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeMesh
{
	public Mesh mesh = new Mesh();
	private Vector3 position = Vector3.zero;
	private Quaternion rotation = Quaternion.identity;
	public Vector3 pivot;

	public static ConeMesh CreateConeMesh(float radius, float height, int segments)
    {
        ConeMesh tmpmesh = new ConeMesh();

        // Vertices
        Vector3[] vertices = new Vector3[segments + 2];

        for (int i = 0; i <= segments; i++)
            vertices[i] = GetVerticePos (i, radius, height, segments);

        vertices[segments + 1] = new Vector3(0f, 0f, 0f);

        // Triangles
        int[] triangles = new int[segments * 3];
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = i;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = segments + 1;
        }

        // Normals
        Vector3[] normals = new Vector3[segments + 2];
        for (int i = 0; i <= segments; i++)
        {
            normals[i] = Vector3.up;
        }
        normals[segments + 1] = Vector3.up;

        // Assigning vertices, triangles, and normals to the mesh
        tmpmesh.mesh.vertices = vertices;
        tmpmesh.mesh.triangles = triangles;
        tmpmesh.mesh.normals = normals;
		tmpmesh.pivot = Vector3.zero;
		tmpmesh.rotation = Quaternion.identity;

        return tmpmesh;
    }

	public void Translate (Vector3 targetPos)
	{
		if (targetPos != position)
		{
			Vector3[] vertices = mesh.vertices;

			// Calculate the translation vector
			Vector3 translation = targetPos - position;

			// Move each vertex by the translation vector
			for (int i = 0; i < vertices.Length; i++)
				vertices[i] += translation;

			// Assign the updated vertices back to the mesh
			mesh.vertices = vertices;
			pivot = mesh.vertices[mesh.vertices.Length -1];

			// Recalculate bounds
			mesh.RecalculateBounds();
			position = targetPos;
		}
	}

	public void RotateMesh (Quaternion targetRotation)
	{
		Vector3 posBak = position;
		Translate (Vector3.zero);

		Quaternion rotationDifference = Quaternion.Inverse(rotation) * targetRotation;

		if (rotationDifference != Quaternion.identity)
		{
			Vector3[] vertices = mesh.vertices;

			// Rotate each vertex
			for (int i = 0; i < vertices.Length; i++)
			{
				//Vector3 displacement = vertices[i] - pivot;
				//Vector3 rotatedDisplacement = rotationDifference * displacement;
				//vertices[i] = pivot + rotatedDisplacement;
				vertices[i] = rotationDifference * vertices[i]/* + pivot*/;
			}

			// Assign the rotated vertices back to the mesh
			mesh.vertices = vertices;

			// Recalculate normals and bounds
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			rotation = targetRotation;
		}

		Translate (posBak);
	}

	private static Vector3 GetVerticePos (int index, float radius, float height, int segments)
	{
		float angleIncrement = 2f * Mathf.PI / segments;
        float angle = index * angleIncrement;
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        return new Vector3(x, z, height);
	}
}
