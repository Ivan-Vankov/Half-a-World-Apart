using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {

	private MeshFilter meshFilter;

	void Awake() {
		meshFilter = GetComponent<MeshFilter>();

		GetComponent<MeshRenderer>().sortingLayerName = "Player";
	}

	private void OnEnable() {
		GenerateMountainCollider.OnColliderGenerate += GenerateMesh;
	}

	private void OnDisable() {
		GenerateMountainCollider.OnColliderGenerate -= GenerateMesh;
	}

	private void GenerateMesh(Vector2[] points) {
		Mesh mesh = new Mesh();

		Vector3[] meshVertices = new Vector3[points.Length + 1];
		Vector2[] uvs = new Vector2[points.Length + 1];

		meshVertices[points.Length] = Vector3.zero;
		uvs[points.Length] = new Vector2(0.5f, 0.5f);

		float angle = Mathf.Deg2Rad * 360f / points.Length;
		float currentAngle;
		float x, y, u, v, xSign, ySign;

		for (int i = 0; i < points.Length; ++i) {
			meshVertices[i] = new Vector3(points[i].x, points[i].y, 0);

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

		int[] meshTriangles = new int[points.Length * 3];

		for (int j = 1, triangleIndex = 0; j < points.Length; ++j) {
			meshTriangles[triangleIndex++] = points.Length;
			meshTriangles[triangleIndex++] = j;
			meshTriangles[triangleIndex++] = j - 1;
		}
		
		mesh.Clear();

		mesh.vertices = meshVertices;
		mesh.triangles = meshTriangles;
		mesh.uv = uvs;

		//mesh.RecalculateNormals();

		meshFilter.mesh = mesh;

		//SaveMesh(mesh);
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		//print(renderer.sortingLayerName);
		renderer.sortingLayerName = "Player";
		//print(renderer.sortingLayerName);
	}

	//private void SaveMesh(Mesh mesh) {
	//	string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", "TerrainMesh", "asset"); ;
	//	path = FileUtil.GetProjectRelativePath(path);

	//	AssetDatabase.CreateAsset(mesh, path);
	//	AssetDatabase.SaveAssets();
	//}
}
