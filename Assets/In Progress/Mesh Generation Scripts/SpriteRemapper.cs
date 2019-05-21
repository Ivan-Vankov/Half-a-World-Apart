using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRemapper : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	private Texture2D initialTexture;

    void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		initialTexture = spriteRenderer.sprite.texture;

		MapTextureWithPolarCoordinates();
    }

	private void MapTextureWithPolarCoordinates() {
		int radius = initialTexture.height / 2;
		Texture2D circleTexture = new Texture2D(radius * 2, radius * 2);

		float unitSquareX, unitSquareY;
		float h, phi;
		int circleX, circleY;

		for (int y = 0; y < initialTexture.height; ++y) {
			unitSquareY = (float)y / initialTexture.height;
			h = unitSquareY;

			for (int x = 0; x < initialTexture.width; ++x) {
				unitSquareX = (float)x / initialTexture.width;
				phi = (unitSquareX - 0.75f) * 2 * Mathf.PI;

				circleX = Mathf.FloorToInt((Mathf.Cos(phi) * h + 1) * radius);
				circleY = Mathf.FloorToInt((Mathf.Sin(phi) * h + 1) * radius);
				circleTexture.SetPixel(circleX, circleY, initialTexture.GetPixel(x, y));
			}
		}

		circleTexture.Apply();
		Rect rect = new Rect(0, 0, circleTexture.width, circleTexture.height);
		Vector2 pivot = new Vector2(0.5f, 0.5f);
		circleTexture.filterMode = FilterMode.Point;
		circleTexture.wrapMode = TextureWrapMode.Clamp;
		Sprite sprite = Sprite.Create(circleTexture, rect, pivot);

		//File.WriteAllBytes(Application.dataPath + "/Sprites/CircleTexture.png", circleTexture.EncodeToPNG());

		spriteRenderer.sprite = sprite;
	}
}
