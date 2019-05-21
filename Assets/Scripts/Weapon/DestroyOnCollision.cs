using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour {

	public Exploder explosionEffect;

	private void OnCollisionEnter2D(Collision2D collision) {
		AudioManager.instance.PlayExplosionSound();
		ScreenShaker.instance.ShakeScreen();

		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<CircleCollider2D>().enabled = false;
		GetComponent<Rigidbody2D>().simulated = false;
		float fadeTime = GetComponent<TrailRenderer>().time;
		
		Vector3 rotation = transform.position - collision.collider.transform.position;
		GameObject explosion = Instantiate(explosionEffect, 
			transform.position, Quaternion.LookRotation(rotation)).gameObject;

		Destroy(explosion, fadeTime);
		Destroy(gameObject, fadeTime);
	}
}
