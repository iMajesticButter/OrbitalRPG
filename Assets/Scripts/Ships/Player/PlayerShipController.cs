using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipMovementController))]
public class PlayerShipController : MonoBehaviour {

	ShipMovementController ship;

	void Start() {
		ship = GetComponent<ShipMovementController>();
	}

	void Update() {

		pointAtMouse();
		move();

	}

	private void pointAtMouse() {
		ship.pointAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}

	private void move() {
		if(Input.GetKey(KeyCode.W)) {
			ship.Thrust = 1.0f;
		} else {
			ship.Thrust = 0.0f;
		}
	}

}
