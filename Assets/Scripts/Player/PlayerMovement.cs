using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	private float planetRadius;
	private float currentAngle;
	private float moveAmount;
	private bool isMoving = false;

	public bool IsFacingRight { get; private set; }

	private Animator animator;

	[SerializeField]
	private float speed = 150f;

    void Start() {
		planetRadius = transform.position.magnitude;
		currentAngle = GetAngle();
		IsFacingRight = transform.localScale.x > 0;
		animator = GetComponent<Animator>();
    }
	
    void Update() {
		Move();
		Rotate();
    }

	private void Move() {
		moveAmount = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
		bool wasMoving = isMoving;
		isMoving = moveAmount != 0;


		if (!wasMoving && isMoving) {
			AudioManager.instance.PlayStepsSound();
		}
		if (wasMoving && !isMoving) {
			AudioManager.instance.StopStepsSound();
		}

		animator.SetBool("IsRunning", isMoving);


		currentAngle = (currentAngle - moveAmount) % 360;

		float newX = planetRadius * Mathf.Cos(currentAngle * Mathf.PI / 180);
		float newY = planetRadius * Mathf.Sin(currentAngle * Mathf.PI / 180);

		transform.position = new Vector3(newX, newY, 0);

		CheckForFlip();
	}

	private void Rotate() {
		transform.rotation = Quaternion.Euler(0, 0, currentAngle - 90);
	}

	private float GetAngle() {
		float x = transform.position.x / planetRadius;
		return Mathf.Acos(x) * Mathf.Rad2Deg;
	}

	private void CheckForFlip() {
		if (moveAmount == 0) {
			return;
		}

		if ((moveAmount < 0 && IsFacingRight) ||
			(moveAmount > 0 && !IsFacingRight)) {
			FlipSprite();
			IsFacingRight = !IsFacingRight;
		}
	}

	private void FlipSprite() {
		Vector3 newScale = transform.localScale;
		newScale.x *= -1;

		transform.localScale = newScale;
	}
}
