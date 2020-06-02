using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipMovementController))]
public class PlayerShipController : MonoBehaviour {

	ShipMovementController ship;

	public static Transform playerShip = null;

	void Start() {
		playerShip = transform;
		ship = GetComponent<ShipMovementController>();
	}

	void OnDestroy() {
		playerShip = null;
	}

	void Update() {

		pointAtMouse();
		move();

		if (Input.GetMouseButtonDown(2)) {
			select();
		}

	}

	private void select() {
		Vector2 mousePosWorld = CameraFollow.GetWorldPositionOnPlane(Input.mousePosition, 0);

		var result = Physics2D.Raycast(mousePosWorld, Vector2.zero);

		if(result.collider != null) {
			if(result.collider.bounds.Contains(mousePosWorld)) {
				//mouse is pointing at somthing
				if(result.transform.GetComponent<Attractor>() != null) {
					ship.Target = result.transform;
				}
			}
		}
	}

	private void pointAtMouse() {
		ship.pointAt(CameraFollow.GetWorldPositionOnPlane(Input.mousePosition, 0));
	}

	private void move() {
		if(Input.GetKey(KeyCode.W)) {
			ship.Thrust = 1.0f;
		} else {
			ship.Thrust = 0.0f;
		}
	}

}
