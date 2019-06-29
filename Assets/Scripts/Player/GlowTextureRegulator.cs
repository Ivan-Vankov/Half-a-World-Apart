using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowTextureRegulator : MonoBehaviour {

	private SpriteRenderer glowRenderer;
	private PlayerTurnManager turnManager;
	private Color emissionColor;

	private readonly string mkGlowTexture = "_MainTex";
	private readonly string mkEmissionColor = "_EmissionColor";
	//private readonly string mkTint = "_Color";


	void Start() {
		glowRenderer = GetComponent<SpriteRenderer>();
		turnManager = GetComponentInParent<PlayerTurnManager>();
		emissionColor = Color.red;
	}

	void Update() {

		if (turnManager.IsYourTurn) {
			glowRenderer.material.SetColor(mkEmissionColor, emissionColor);
		} else {
			// No glow
			glowRenderer.material.SetColor(mkEmissionColor, Color.black);
		}

		glowRenderer.material.SetTexture(mkGlowTexture, glowRenderer.sprite.texture);
	}
}
