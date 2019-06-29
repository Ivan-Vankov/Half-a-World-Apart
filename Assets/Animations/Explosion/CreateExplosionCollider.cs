using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CreateExplosionCollider : MonoBehaviour {

	public static event Action<Sprite, PolygonCollider2D> OnExplosionOccur;
	public NetworkManager networkManager;

	public int ExplosionDamage { get; set; } = 10;

	private void Start() {
		networkManager = FindObjectOfType<NetworkManager>();
	}

	public void CreateCollider() {
		GetComponent<Animator>().enabled = false;
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		renderer.enabled = false;
		
		PolygonCollider2D collider = gameObject.AddComponent<PolygonCollider2D>();

		OnExplosionOccur?.Invoke(renderer.sprite, collider);
		networkManager.SwitchTurns();
	}
}
