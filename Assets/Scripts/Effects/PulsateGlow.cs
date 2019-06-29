using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MK.Glow.Legacy;
using static UnityEngine.Mathf;

[RequireComponent(typeof(MKGlow))]
public class PulsateGlow : MonoBehaviour {

	private MKGlow mkGlow;

	private void Start() {
		mkGlow = GetComponent<MKGlow>();
	}

	void Update() {
		mkGlow.bloomIntensity = Sin(3 * Time.time) / 2 + 0.5f;
    }
}
