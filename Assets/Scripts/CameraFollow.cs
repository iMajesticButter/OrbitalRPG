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

	public Camera cam { get; private set; }

	void Start() {
		cam = GetComponent<Camera>();

		transform.position = new Vector3(target.position.x, target.position.y, zAxis);
	}

	
	void FixedUpdate() {
		//float xLerp = Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * speed);
		//float yLerp = Mathf.Lerp(transform.position.y, target.position.y, Time.deltaTime * speed);

		//transform.position = new Vector3(xLerp, yLerp, zAxis);

		transform.position = new Vector3(target.position.x, target.position.y, zAxis);

		Vector2 scrollDelta = Input.mouseScrollDelta;

		cam.fieldOfView -= scrollDelta.y * zoomSpeed;

	}
}
