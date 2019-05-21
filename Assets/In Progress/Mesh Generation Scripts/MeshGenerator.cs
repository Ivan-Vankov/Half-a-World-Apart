using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {

	private MeshFilter meshFilter;
	private Mesh mesh;

	void Start() {
		meshFilter = GetComponent<MeshFilter>();
		mesh = new Mesh();
	}

	private void OnEnable() {
		GenerateMountainCollider.OnColliderGenerate += GenerateMesh;
	}

	private void OnDisable() {
		GenerateMountainCollider.OnColliderGenerate -= GenerateMesh;
	}

	private void GenerateMesh(EdgeCollider2D collider) {
		Vector3[] meshVertices = new Vector3[collider.pointCount + 1];
		Vector2[] uvs = new Vector2[collider.pointCount + 1];

		meshVertices[collider.pointCount] = collider.transform.position;
		uvs[collider.pointCount] = new Vector2(0.5f, 0.5f);

		float angle = Mathf.Deg2Rad * 360f / collider.pointCount;
		float currentAngle;
		float x, y, u, v, xSign, ySign;

		for (int i = 0; i < collider.pointCount; ++i) {
			meshVertices[i] = new Vector3(collider.points[i].x, collider.points[i].y, 0);

			currentAngle = i * angle;

			x = Mathf.Cos(currentAngle);
			y = Mathf.Sin(currentAngle);

			if (Mathf.Abs(x) < Mathf.Abs(y)) {
				ySign = Mathf.Sign(y);
				v = (ySign + 1f) / 2f;
				u = (ySign * (x / y) + 1f) / 2f;
			}
			else {
				xSign = Mathf.Sign(x);
				u = (xSign + 1f) / 2f;
				v = (xSign * (y / x) + 1f) / 2f;
			}

			uvs[i] = new Vector2(u, v);
		}

		int[] meshTriangles = new int[collider.pointCount * 3];

		for (int j = 1, triangleIndex = 0; j < collider.pointCount; ++j) {
			meshTriangles[triangleIndex++] = collider.pointCount;
			meshTriangles[triangleIndex++] = j;
			meshTriangles[triangleIndex++] = j - 1;
		}

		mesh.Clear();

		mesh.vertices = meshVertices;
		mesh.triangles = meshTriangles;
		mesh.uv = uvs;

		//mesh.RecalculateNormals();

		meshFilter.mesh = mesh;
	}

}
