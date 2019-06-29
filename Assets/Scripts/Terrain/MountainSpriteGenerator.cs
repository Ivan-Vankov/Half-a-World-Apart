using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public class MountainSpriteGenerator : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	private Texture2D initialTexture;

	private void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();

		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
		foreach (var renderer in renderers) {
			if (renderer.gameObject.name.Equals("BaseRemappableTexture")) {
				initialTexture = renderer.sprite.texture;
			}
		}
	}

	private void OnEnable() {
		GenerateMountainCollider.OnColliderGenerate += GenerateSprite;
	}

	private void OnDisable() {
		GenerateMountainCollider.OnColliderGenerate -= GenerateSprite;
	}

	private void GenerateSprite(Vector2[] points) {
		Texture2D newTexture = new Texture2D(initialTexture.width, initialTexture.height);

		float maxDistance = 0;
		float currentDistance;
		foreach (Vector2 point in points) {
			currentDistance = point.magnitude;

			if (currentDistance > maxDistance) {
				maxDistance = currentDistance;
			}
		}

		// -1 because the first point is repeated at the end
		float angle = 360f / (points.Length - 1);
		float currentAngle;
		Vector2 center = new Vector2(1f, 1f);
		Vector2 unitCircleXY = new Vector2();
		Vector3 lineToCenter;
		Vector3 terrainLine = new Vector2();
		Vector2 intersection;
		Vector2 targetPointUnitSquare;
		int terrainPointIndex;
		Color transparant = new Color(0, 0, 0, 0);

		newTexture.SetPixel(newTexture.width / 2, newTexture.height / 2,
			initialTexture.GetPixel(newTexture.width / 2, newTexture.height / 2));

		for (int y = 0; y < newTexture.height; ++y) {
			unitCircleXY.y = 2f * y / newTexture.height;

			for (int x = 0; x < newTexture.width; ++x) {
				unitCircleXY.x = 2f * x / newTexture.width;

				//if (unitCircleXY.x == 1 && unitCircleXY.y == 1) {
				//	newTexture.SetPixel(x, y, initialTexture.GetPixel(x, y));
				//	continue;
				//}
				
				currentAngle = GetAngle(unitCircleXY, center);

				terrainPointIndex = FloorToInt(currentAngle * Rad2Deg * (points.Length - 1) / 360);

				try {
					terrainLine = LineFromTwoPoints(
						points[terrainPointIndex] / maxDistance + center,
						points[terrainPointIndex + 1] / maxDistance + center);
				} catch (System.IndexOutOfRangeException) {
					// x == 1 && y == 1 will be cought here
					// but that pixel has already been set before the loop
					continue;
				}

				
				lineToCenter = LineFromTwoPoints(center, unitCircleXY);

				intersection = LineIntersection(terrainLine, lineToCenter);
			
				Vector2 toCurrentPoint = unitCircleXY - center;

				Vector2 toIntersection = intersection - center;

				float scaleToCurrentPoint = toCurrentPoint.magnitude;
				float scaleToTerrain = toIntersection.magnitude;

				if (scaleToCurrentPoint > scaleToTerrain) {
					newTexture.SetPixel(x, y, transparant);
					continue;
				}

				targetPointUnitSquare = center + toCurrentPoint.normalized * 
					scaleToCurrentPoint / scaleToTerrain;
				
				newTexture.SetPixel(
					x,
					y,
					initialTexture.GetPixel(
						FloorToInt(targetPointUnitSquare.x * newTexture.width / 2),
						FloorToInt(targetPointUnitSquare.y * newTexture.height / 2)));
			}
		}

		newTexture.Apply();

		Rect rect = new Rect(0, 0, newTexture.width, newTexture.height);
		Vector2 pivot = new Vector2(0.5f, 0.5f);
		newTexture.filterMode = FilterMode.Point;
		newTexture.wrapMode = TextureWrapMode.Clamp;
		Sprite sprite = Sprite.Create(newTexture, rect, pivot);

		spriteRenderer.sprite = sprite;

		// Optionally Save to file
		//System.IO.File.WriteAllBytes(Application.dataPath + "\\Sprites\\Terrain\\TerrainFinal.png", newTexture.EncodeToPNG());
	} 

	private float GetAngle(Vector2 unitCircleXY, Vector2 center) {
		float distanceToCenter = (unitCircleXY - center).magnitude;

		float cosPhi = (unitCircleXY.x - center.x) / distanceToCenter;
		float sinPhi = (unitCircleXY.y - center.y) / distanceToCenter;
		float angle = Atan(sinPhi / cosPhi);

		if (unitCircleXY.x < center.x) {
			angle += PI;
		}
		else if (unitCircleXY.y < center.y) {
			angle += 2 * PI;
		}
		

		return angle;
	}

	private Vector3 LineFromTwoPoints(Vector2 point1, Vector2 point2) {
		// The returned line will be ax + by + c = 0 where
		// the returned vector3 x, y, z will be a, b, and c
		return new Vector3(
			point1.y - point2.y,
			point2.x - point1.x,
			point1.x * point2.y - point1.y * point2.x);
	}

	private Vector2 LineIntersection(Vector3 line1, Vector3 line2) {
		// line1: y = ax + b; line2: y = cx + d
		// ax + c = cx + d
		// (a - c)x = d - b
		// x = (d - b) / (a - c)
		float x, y;

		float eps = 0.001f;
		if (Abs(line2.y) < eps) {
			x = -line2.z / line2.x;
			y = (line1.x * -x - line1.z) / line1.y;
			return new Vector2(x, y);
		}

		Vector2 ax_b = new Vector2(line1.x / -line1.y, line1.z / -line1.y);
		Vector2 cx_d = new Vector2(line2.x / -line2.y, line2.z / -line2.y);

		x = (cx_d.y - ax_b.y) / (ax_b.x - cx_d.x);
		y = ax_b.x * x + ax_b.y;

		return new Vector2(x, y);
	}
}
