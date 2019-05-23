using UnityEngine;
using System.IO;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRemapper : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	private Texture2D initialRectangularTexture;

    void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		initialRectangularTexture = spriteRenderer.sprite.texture;

		MapTextureWithPolarCoordinates();
    }

	private void MapTextureWithPolarCoordinates() {

		Texture2D outputTexture = WrapCircularTextureToSquare(
			WrapRectangularTextureToCircle(initialRectangularTexture));

		Rect rect = new Rect(0, 0, outputTexture.width, outputTexture.height);
		Vector2 pivot = new Vector2(0.5f, 0.5f);
		outputTexture.filterMode = FilterMode.Point;
		outputTexture.wrapMode = TextureWrapMode.Clamp;
		Sprite sprite = Sprite.Create(outputTexture, rect, pivot);

		//SaveTextureToFile(outputTexture, "Terrain Generation/Sprites");
		
		spriteRenderer.sprite = sprite;
		//spriteRenderer.sortingLayerName = "Default";
	}

	private void SaveTextureToFile(Texture2D texture, string path) {
		File.WriteAllBytes(Application.dataPath + path, texture.EncodeToPNG());
	}

	private Texture2D WrapRectangularTextureToCircle(Texture2D initialTexture) {
		int radius = initialTexture.height / 2;
		Texture2D circleTexture = new Texture2D(radius * 2, radius * 2);

		float squareX01, squareY01;
		float h, phi;
		Vector2 unitCircleXY = new Vector2();
		int initialTextureX, initialTextureY;

		for (int y = 0; y < circleTexture.height; ++y) {
			unitCircleXY.y = 2f * y / circleTexture.height;

			for (int x = 0; x < circleTexture.width; ++x) {
				unitCircleXY.x = 2f * x / circleTexture.width;
				
				h = (unitCircleXY - Vector2.one).magnitude;
				squareY01 = h;

				float cosPhi = (unitCircleXY.x - 1f) / h;
				float sinPhi = (unitCircleXY.y - 1f) / h;
				phi = Mathf.Atan(sinPhi / cosPhi);

				if (unitCircleXY.x < 1) {
					phi -= Mathf.PI;
				}

				squareX01 = phi / (2 * Mathf.PI) + 0.75f;

				initialTextureX = Mathf.FloorToInt(squareX01 * initialTexture.width);
				initialTextureY = Mathf.FloorToInt(squareY01 * initialTexture.height);

				Color targetColor = initialTexture.GetPixel(initialTextureX, initialTextureY);

				circleTexture.SetPixel(x, y, targetColor);
			}
		}

		circleTexture.Apply();

		return circleTexture;
	}

	private Texture2D WrapCircularTextureToSquare(Texture2D circleTexture) {
		Texture2D outputTexture = new Texture2D(circleTexture.width, circleTexture.height);

		Vector2 unitCircleXY = new Vector2();
		Vector2 center = new Vector2(0.5f, 0.5f);
		Vector2 intersection = new Vector2();
		Vector2 direction;
		Vector2 targetPointUnitSquare = new Vector2();
		int circleTextureX, circleTextureY;
		float currentDistance, maxDistance;

		for (int y = 0; y < outputTexture.height; ++y) {
			unitCircleXY.y = (float)y / circleTexture.height;

			for (int x = 0; x < outputTexture.width; ++x) {
				unitCircleXY.x = (float)x / circleTexture.width;
				Vector3 line = GetLineForm2Points(unitCircleXY, center);
				direction = unitCircleXY - center;

				// We split up the unit square into 4 chunks
				//
				// *---------*
				// |\       /|
				// | \  1  / |
				// |  \   /  |
				// | 2 \ / 4 |
				// |    *    |
				// |   / \   |
				// |  /   \  |
				// | /  3  \ |
				// |/       \|
				// *---------*
				// And we check which chunk we are in

				if (Mathf.Abs(unitCircleXY.y - center.y) > Mathf.Abs(unitCircleXY.x - center.x)) {
					// Chunk 1
					if (unitCircleXY.y > 0.5f) {
						intersection.y = 1;
						intersection.x = (-line.y - line.z) / line.x;
					} // Chunk 3
					else {
						intersection.y = 0;
						intersection.x = -line.z / line.x;
					}
				} else {
					// Chunk 2
					if (unitCircleXY.x < 0.5f) {
						intersection.x = 0;
						intersection.y = -line.z / line.y;
					} // Chunk 4
					else {
						intersection.x = 1;
						intersection.y = (-line.x - line.z) / line.y;
					}
				}

				maxDistance = (intersection - center).magnitude;
				currentDistance = direction.magnitude;
				direction.Normalize();

				if (x % 100 == 0 && y % 100 == 0) {
					print("Unit circle x,y: " + unitCircleXY.x + ", " + unitCircleXY.y +
						"; Intersection x, y: " + intersection.x + ", " + intersection.y +
						"; Direction normalized x, y: " + direction.x + ", " + direction.y);
				}

				targetPointUnitSquare = center + direction * (currentDistance / maxDistance) / 2;

				circleTextureX = Mathf.FloorToInt(targetPointUnitSquare.x * circleTexture.width);
				circleTextureY = Mathf.FloorToInt(targetPointUnitSquare.y * circleTexture.height);

				outputTexture.SetPixel(x, y, circleTexture.GetPixel(circleTextureX, circleTextureY));
			}
		}

		outputTexture.Apply();

		return outputTexture;
	}

	// The returned line will be ax + by + c = 0 where
	// the returned vector3 x, y, z will be a, b, and c
	private Vector3 GetLineForm2Points(Vector2 point1, Vector2 point2) {
		return new Vector3(
			point1.y - point2.y, 
			point2.x - point1.x,
			point1.x * point2.y - point1.y * point2.x);
	}
}
