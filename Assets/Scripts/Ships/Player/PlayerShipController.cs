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
	public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z) {
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
		float distance;
		xy.Raycast(ray, out distance);
		return ray.GetPoint(distance);
	}

	private void pointAtMouse() {
		ship.pointAt(GetWorldPositionOnPlane(Input.mousePosition, 0));
	}

	private void move() {
		if(Input.GetKey(KeyCode.W)) {
			ship.Thrust = 1.0f;
		} else {
			ship.Thrust = 0.0f;
		}
	}

}
