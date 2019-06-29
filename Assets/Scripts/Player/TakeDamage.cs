using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour {

	private PlayerDamageReceived damageReceived;

	private void Start() {
		damageReceived = GetComponentInParent<PlayerDamageReceived>();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Explosion Collider")) {
			damageReceived.DamageReceived += 
				collision.GetComponent<CreateExplosionCollider>().ExplosionDamage;

			AudioManager.instance.PlayHitSound();
		}
	}
}
