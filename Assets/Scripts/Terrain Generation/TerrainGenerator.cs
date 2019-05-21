using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

	private PolygonCollider2D polygonCollider;
	private Mesh mountainMesh;

    void Start() {
		polygonCollider = GetComponent<PolygonCollider2D>();
		mountainMesh = new Mesh();

		foreach (Vector2 point in polygonCollider.points) {
			print(point);
		}
    }
}
