using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

[RequireComponent(typeof(PlayerTurnManager))]
public class PlayerMovement : MonoBehaviour {

	private float planetRadius;
	private float currentAngle;
	private float moveAmount;
	private bool isMoving = false;

	public bool IsFacingRight { get; private set; }

	public static readonly int movesPerTurn = 2;
	public int movesLeft;
	private readonly float moveDuration = 0.4f;
	private float moveStartTime;

	private Animator animator;

	[SerializeField]
	private float speed = 50f;

	Vector2 unitCirclePosition = new Vector2();

	private PlayerTurnManager turnManager;

    void Start() {
		turnManager = GetComponent<PlayerTurnManager>();
		planetRadius = transform.position.magnitude;
		currentAngle = transform.rotation.z * 180 + 90;
		//print("PlayerMovement.currentAngle: " + currentAngle);
		//print("rotation.z = " + transform.rotation.z);
		IsFacingRight = transform.localScale.x > 0;
		animator = GetComponentInChildren<Animator>();

		movesLeft = movesPerTurn;
		//TestCollisionWithExtraCollider();

		unitCirclePosition.x = Cos(currentAngle * Deg2Rad);
		unitCirclePosition.y = Sin(currentAngle * Deg2Rad);
	}
	
    void Update() {
		//currentAngle = GetAngle();
		//print("PlayerMovement.currentAngle: " + currentAngle);

		if (turnManager.IsYourTurn) {
			if (Input.GetAxisRaw("Horizontal") != 0 && 
				movesLeft > 0 && 
				!isMoving) {

				isMoving = true;
				--movesLeft;
				StartCoroutine(Move(moveDuration, Input.GetAxisRaw("Horizontal")));
			}
		}

		AdjustHeight();
		Rotate();
	}

	private IEnumerator Move(float duration, float axisInput) {
		AudioManager.instance.PlayStepsSound();

		while (duration > 0) {
			moveAmount = axisInput * speed * Time.deltaTime;
			bool wasMoving = isMoving;
			isMoving = moveAmount != 0;

			animator.SetBool("IsRunning", isMoving);

			currentAngle = (currentAngle - moveAmount) % 360;

			unitCirclePosition.x = Cos(currentAngle * Deg2Rad);
			unitCirclePosition.y = Sin(currentAngle * Deg2Rad);

			CheckForFlip();

			duration -= Time.deltaTime;

			yield return null;
		}

		isMoving = false;
		animator.SetBool("IsRunning", isMoving);
		AudioManager.instance.StopStepsSound();
	}

	private void AdjustHeight() {
		Vector2 directionToCenter = -unitCirclePosition;

		float terrainHeight = TerrainHeight(directionToCenter);

		transform.position = unitCirclePosition * terrainHeight;
	}

	private void Rotate() {
		transform.rotation = Quaternion.Euler(0, 0, currentAngle - 90);
	}

	//private float GetAngle() {
	//	//float x = transform.position.x / planetRadius;
	//	//return Acos(x) * Rad2Deg;
	//	return transform.rotation.z + 90;
	//}

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

	private float TerrainHeight(Vector2 direction) {
		Vector2 rayStart = -direction * 100;
		int layerMask = LayerMask.GetMask("Terrain Collider");

		RaycastHit2D raycastHit = Physics2D.Raycast(rayStart, direction.normalized, 1000, layerMask);
		//Debug.DrawRay(rayStart, direction.normalized * raycastHit.distance, Color.yellow, 2);

		//print("rayStart: " + rayStart + "; " + raycastHit.point);

		return raycastHit.point.magnitude;
	}

	//private void TestCollisionWithExtraCollider() {
	//	int pointCount = 135;

	//	float angle = Deg2Rad * 360 / pointCount;
	//	float currentAngle;
	//	Vector2 direction = new Vector2();
	//	RaycastHit2D raycastHit;
	//	Vector2 origin = Vector2.zero;
	//	int layerMask = LayerMask.GetMask("Terrain Collider", "Explosion Collider");

	//	for (int i = 0; i < pointCount; ++i) {
	//		currentAngle = i * angle;

	//		direction.x = Cos(currentAngle);
	//		direction.y = Sin(currentAngle);
	//		raycastHit = Physics2D.Raycast(origin, direction, 100, layerMask);

	//		Debug.DrawRay(origin, direction * raycastHit.distance, Color.red, 20000);
	//	}
	//}
}
