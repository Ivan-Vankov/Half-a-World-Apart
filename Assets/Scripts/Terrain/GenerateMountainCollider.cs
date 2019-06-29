using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

[RequireComponent(typeof(PolygonCollider2D))]
public class GenerateMountainCollider : MonoBehaviour {

	private PolygonCollider2D polyCollider;

	public int pointCount = 90;
	public float radius = 1f;
	public float amplitude = 1f;
	public float frequency = 1f;

	// Controlls increase in frequency in octaves
	public float lucanarity = 2f;
	// Controlls increase in amplitude in octaves
	public float persistance = 0.15f;

	private float timeOfLastPress = 0f;
	private float waitTime = 1f;
	private bool isConstantlyGenerateVertices = false;
	
	public static event System.Action<Vector2[]> OnColliderGenerate;

	void Start() {
		polyCollider = GetComponent<PolygonCollider2D>();
		GenerateColliderPoints();
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			timeOfLastPress = Time.time;
			GenerateColliderPoints();
		}

		if (Input.GetKey(KeyCode.Space) && 
			Time.time - timeOfLastPress > waitTime &&
			!isConstantlyGenerateVertices) {
			isConstantlyGenerateVertices = true;
			StartCoroutine(ConstantlyGenerateVertices());
		}

		if (Input.GetKeyUp(KeyCode.Space)) {
			StopAllCoroutines();
			isConstantlyGenerateVertices = false;
		}
	}

	private IEnumerator ConstantlyGenerateVertices() {
		float speed = 1f;
		while (true) {
			GenerateColliderPoints();

			speed += 0.05f;
			if (speed >= 2) {
				speed = 2;
			}

			yield return new WaitForSeconds(0.2f / speed);
		}
	}

	private void GenerateColliderPoints() {

		Vector2[] points = new Vector2[pointCount + 1];

		float angle = Deg2Rad * 360f / pointCount;
		float terrainMultiplier;
		float x, y;
		float currentAngle;
		float seed = Random.value * 100;
		float scale = 5f;
		float lucanaritySq = lucanarity * lucanarity;
		float persistanceSq = persistance * persistance;

		for (int i = 0; i < pointCount; ++i) {
			terrainMultiplier = GetTerrainPoint((float)i / pointCount);

			float octave2Offset = PerlinNoise((seed + i / scale) * lucanarity, 0) * persistance;
			float octave3Offset = PerlinNoise((seed + i / scale) * lucanaritySq, 0) * persistanceSq;
			float totalOctaveOffset = octave2Offset + octave3Offset;

			currentAngle = i * angle;
			x = radius * Cos(currentAngle) * terrainMultiplier;
			y = radius * Sin(currentAngle) * terrainMultiplier;
			
			points[i] = new Vector2(x, y) * (1 + totalOctaveOffset);
		}

		points[pointCount] = points[0];

		polyCollider.points = points;

		OnColliderGenerate?.Invoke(polyCollider.points);
		RegenerateCollider();
	}

	private float GetTerrainPoint(float x) {
		// Math magic that varies from 0.5 to 1 
		// and has 2 bumps from 0 to 1
		// https://i.gyazo.com/9868e800cd15aaa6c756c529bd7c1afa.png

		if (x < 0.1167f) {
			return 4f / (Cos(x * PI * 4 - PI) + 5f);
		}
		if (x < 0.3839f) {
			return (Cos(PI * x * 0.9f + 2.44f) + 2.2f) / 4f + 0.5f;
		}
		if (x < 0.6164f) {
			return 4f / (Cos(x * PI * 4 - PI) + 5f);
		} 
		if (x < 0.8835f) {
			return (Cos(PI * x * 0.9f + 1.02f) + 2.2f) / 4f + 0.5f;
		}
		else {
			return 4f / (Cos(x * PI * 4 - PI) + 5f);
		}
	}

	public void RegenerateCollider() {
		Destroy(polyCollider);
		polyCollider = gameObject.AddComponent<PolygonCollider2D>();
	}
}
