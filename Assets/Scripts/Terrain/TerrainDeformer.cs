using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TerrainDeformer : MonoBehaviour {

	private SpriteRenderer terrainRenderer;
	private GenerateMountainCollider mountainColliderGenerator;

	private void Awake() {
		terrainRenderer = GetComponent<SpriteRenderer>();
		mountainColliderGenerator = GetComponent<GenerateMountainCollider>();
	}

	//private void Update() {
	//	Sprite sprite = terrainRenderer.sprite;
	//	Vector2 center = transform.position;

	//	float spriteWidth  = transform.lossyScale.x * sprite.texture.width  / sprite.pixelsPerUnit;
	//	float spriteHeight = transform.lossyScale.y * sprite.texture.height / sprite.pixelsPerUnit;

	//	Vector2 bottomLeft = new Vector2 {
	//		x = center.x - spriteWidth / 2,
	//		y = center.y - spriteWidth / 2
	//	};

	//	print(bottomLeft);
	//}

	private void OnEnable() {
		CreateExplosionCollider.OnExplosionOccur += DeformTerrain;
	}

	private void OnDisable() {
		CreateExplosionCollider.OnExplosionOccur -= DeformTerrain;
	}

	private void DeformTerrain(Sprite explosionSprite, PolygonCollider2D explosionCollider) {

		Transform explosionTransform = explosionCollider.transform;
		Texture2D explosionTexture = explosionSprite.texture;
		
		SetSpriteData(explosionTransform, explosionSprite,
			out float explosionPixelWidth, 
			out float explosionPixelHeight,
			out Vector2 explosionTextureStartPosition);


		Sprite terrainSprite = terrainRenderer.sprite;
		Texture2D terrainTexture = terrainSprite.texture;

		SetSpriteData(transform, terrainSprite,
			out float terrainPixelWidth,
			out float terrainPixelHeight,
			out Vector2 terrainTextureStartPosition);

		Color pixel;
		Vector2 pixelInWorldCoordinates = new Vector2();
		Vector2Int terrainTextureCoordinates = new Vector2Int();
		Color transparent = new Color(0, 0, 0, 0);

		for (int y = 0; y < explosionTexture.height; ++y) {
			for (int x = 0; x < explosionTexture.width; ++x) {
				pixel = explosionTexture.GetPixel(x, y);
				if (pixel.a == 0) {
					continue;
				}

				TextureToWorldCoordinates(
					ref pixelInWorldCoordinates,
					x, y,
					explosionPixelWidth,
					explosionPixelHeight,
					explosionTextureStartPosition);
				
				WorldToTextureCoordinates(
					ref terrainTextureCoordinates,
					pixelInWorldCoordinates,
					terrainPixelWidth,
					terrainPixelHeight,
					terrainTextureStartPosition);

				terrainTexture.SetPixel(
					terrainTextureCoordinates.x,
					terrainTextureCoordinates.y,
					transparent);
			}
		}

		terrainTexture.Apply();

		Rect rect = new Rect(0, 0, terrainTexture.width, terrainTexture.height);
		Vector2 pivot = new Vector2(0.5f, 0.5f);
		terrainTexture.filterMode = FilterMode.Point;
		terrainTexture.wrapMode = TextureWrapMode.Clamp;
		Sprite sprite = Sprite.Create(terrainTexture, rect, pivot);

		terrainRenderer.sprite = sprite;

		mountainColliderGenerator.RegenerateCollider();
	}

	private void SetSpriteData(
		Transform spriteTransform, 
		Sprite sprite, 
		out float pixelWidth,
		out float pixelHeight,
		out Vector2 textureStartPosition) {

		pixelWidth = spriteTransform.lossyScale.x / sprite.pixelsPerUnit;
		pixelHeight = spriteTransform.lossyScale.y / sprite.pixelsPerUnit;

		textureStartPosition = new Vector2 {
			x = spriteTransform.position.x - sprite.texture.width * pixelWidth / 2,
			y = spriteTransform.position.y - sprite.texture.height * pixelWidth / 2
		};
	}

	private void TextureToWorldCoordinates(
		ref Vector2 worldCoordinates,
		int x, int y, 
		float pixelWidth, 
		float pixelHeight, 
		Vector2 textureStartPosition) {

		worldCoordinates.x = textureStartPosition.x + x * pixelWidth;
		worldCoordinates.y = textureStartPosition.y + y * pixelHeight;
	}

	private void WorldToTextureCoordinates(
		ref Vector2Int textureCoordinates,
		Vector2 worldCoordinates,
		float pixelWidth,
		float pixelHeight,
		Vector2 textureStartPosition) {

		textureCoordinates.x = (int)((worldCoordinates.x - textureStartPosition.x) / pixelWidth);
		textureCoordinates.y = (int)((worldCoordinates.y - textureStartPosition.y) / pixelHeight);
	}
}
