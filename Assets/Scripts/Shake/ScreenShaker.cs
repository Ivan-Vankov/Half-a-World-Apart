using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShaker : MonoBehaviour {

	public static ScreenShaker instance = null;

	private bool isShaking = false;

	private void Start() {
		if (instance == null) {
			instance = this;
		}
	}

	public void ShakeScreen() {
		ShakeScreen(10, 0.2f, 0.10f);
	}

	public void ShakeScreen(float intensity, float duration, float easeInTime) {
		if (intensity <= 0 || duration <= 0) {
			return;
		}

		if (!isShaking) {
			StartCoroutine(Shake(intensity, duration, easeInTime));
		}
	}

	private IEnumerator Shake(float initialIntensity, float duration, float easeInTime) {
		isShaking = true;

		float easeIn = Time.deltaTime;
		
		float xShakeSeed = Random.value * 100;
		float yShakeSeed = Random.value * 100;
		Vector3 originalPosition = Camera.main.transform.position;

		float start = Time.time;
		while (Time.time < start + duration) {
			Vector3 newPosition = originalPosition;
			float step = (Time.time - start) * initialIntensity;

			float intensity = initialIntensity * (duration - (Time.time - start)) / duration;

			if (Time.time < start + easeIn) {
				intensity *= easeIn;
				easeIn += Time.deltaTime;
			}

			// * 2 - 1 in order to shift the offsets from [0,1] to [-1,1]
			float xOffset = Mathf.PerlinNoise(xShakeSeed + step, 0) * 2 - 1;
			float yOffset = Mathf.PerlinNoise(yShakeSeed + step, 0) * 2 - 1;
			newPosition.x += xOffset * intensity;
			newPosition.y += yOffset * intensity;
			Camera.main.transform.position = newPosition;
			yield return null;
		}

		Camera.main.transform.position = originalPosition;
		isShaking = false;
	}
}
