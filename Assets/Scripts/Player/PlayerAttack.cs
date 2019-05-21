using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

	public GameObject weapon;

	private PlayerMovement playerDirection;

	public Transform weaponSpawnPositionRight;
	public Transform weaponSpawnPositionLeft;

	private float initialForce = 700f;

	private AudioSource throwSound;

	private void Start() {
		playerDirection = GetComponent<PlayerMovement>();
		throwSound = GetComponent<AudioSource>();
	}
	
	private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
			ThrowWeapon();
		}
    }

	private void ThrowWeapon() {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 weaponSpawnPosition = GetWeaponSpawnPosition(mousePosition);

		Vector3 direction = mousePosition - weaponSpawnPosition;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		Rigidbody2D spawnedWeapon = Instantiate(weapon, weaponSpawnPosition, 
			Quaternion.Euler(new Vector3(0, 0, angle - 90))).GetComponent<Rigidbody2D>();

		AudioManager.instance.PlayThrowSound();

		Vector2 force = new Vector2(direction.x, direction.y);
		spawnedWeapon.AddForce(force.normalized * initialForce);
		spawnedWeapon.AddTorque(100);
	}

	private Vector3 GetWeaponSpawnPosition(Vector3 mousePosition) {
		Vector3 right = weaponSpawnPositionRight.position - transform.position;
		Vector3 toMouse = mousePosition - transform.position;

		if (Vector3.Dot(right, toMouse) > 0) {
			return weaponSpawnPositionRight.position;
		} else {
			return weaponSpawnPositionLeft.position;
		}
	}
}
