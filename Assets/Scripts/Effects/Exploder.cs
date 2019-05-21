using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : MonoBehaviour {

	private void Start() {
		ParticleSystem[] effects = GetComponentsInChildren<ParticleSystem>();

		if (effects.Length == 0) {
			Debug.Log("Please add particle systems to " + name);
			return;
		}

		float maxDuration = 0;
		foreach (ParticleSystem effect in effects) {
			effect.Play();

			if (effect.main.duration > maxDuration) {
				maxDuration = effect.main.duration;
			}
		}

		Destroy(gameObject, maxDuration);
	}
}
