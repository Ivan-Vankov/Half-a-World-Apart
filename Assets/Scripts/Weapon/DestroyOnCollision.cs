using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour {

	public Exploder explosionEffect;
	public GameObject explosionCircle;

	private static int validCollisionLayerMask;

	private void Awake() {
		validCollisionLayerMask = LayerMask.GetMask("Player Collider", "Terrain Collider");
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		int collisionMask = 1 << collision.gameObject.layer;

		if ((validCollisionLayerMask & collisionMask) == 0) {
			//print("Collided with " + collision.gameObject.name);
			return;
		}

		AudioManager.instance.PlayExplosionSound();
		ScreenShaker.instance.ShakeScreen();

		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<CircleCollider2D>().enabled = false;
		GetComponent<Rigidbody2D>().simulated = false;
		float fadeTime = GetComponent<TrailRenderer>().time;

		Vector3 rotation = transform.position - collision.transform.position;
		GameObject explosion = Instantiate(explosionEffect,
			transform.position, Quaternion.LookRotation(rotation)).gameObject;

		SpawnExplosionCircle(collision.transform.position);

		Destroy(explosion, fadeTime);
		Destroy(gameObject, fadeTime);
	}

	public void SpawnExplosionCircle(Vector3 position) {
		GameObject spawnedExplosionCircle = Instantiate(explosionCircle, transform.position, Quaternion.identity);
		Destroy(spawnedExplosionCircle, 0.5f);
	}
}
