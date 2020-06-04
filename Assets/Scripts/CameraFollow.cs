using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour {

	[Header("Oject To Follow")]
	public Transform target;

	[Header("Z axis")]
	public float zAxis = -10.0f;

	[Header("Movement Speed")]
	public float speed = 2.0f;

	[Header("Zoom Speed")]
	public float zoomSpeed = 1.0f;

	[Header("What Zoom Level To Show Map Icons At")]
	[Range(0.2f, 179)]
	public float mapZoomLevel = 90;

	public Camera cam { get; private set; }

	public static bool showMapIcons = false;

	public static float scaleMultMatchScreen { get {
			return (GetWorldPositionOnPlane(new Vector3(Screen.width, 0, 0), 0) - Camera.main.transform.position).x/10;
		} 
	}

	public static Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z) {
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
		float distance;
		xy.Raycast(ray, out distance);
		return ray.GetPoint(distance);
	}

	void Start() {
		cam = GetComponent<Camera>();

		transform.position = new Vector3(target.position.x, target.position.y, zAxis);
	}

	
	void FixedUpdate() {
		float xLerp = Mathf.Lerp(transform.position.x, target.position.x, Time.fixedDeltaTime * speed);
		float yLerp = Mathf.Lerp(transform.position.y, target.position.y, Time.fixedDeltaTime * speed);

		transform.position = new Vector3(xLerp, yLerp, zAxis);

		//transform.position = new Vector3(target.position.x, target.position.y, zAxis);

		Vector2 scrollDelta = Input.mouseScrollDelta;

		cam.fieldOfView -= scrollDelta.y * zoomSpeed;

		showMapIcons = cam.fieldOfView >= mapZoomLevel;

	}
}
