using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Attracted))]
public class ShipMovementController : MonoBehaviour {

	[HideInInspector]
	public Rigidbody2D rb;

	[HideInInspector]
	public Attracted att { get; private set; }

	[HideInInspector]
	public float targetRotation;

	[HideInInspector]
	public float Thrust {
		get { return thrust; }
		set { thrust = Mathf.Clamp(value, 0.0f, 1.0f); }
	}

	private float thrust;

	[Header("Movement properties")]

	public float turnSpeed = 5.0f;
	public float engineThrust = 10.0f;

	public void pointAt(Vector2 pos) {
		Vector2 vec = rb.position - pos;
		targetRotation = Mathf.Atan2(vec.y, vec.x);
	}

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		att = GetComponent<Attracted>();
		targetRotation = rb.rotation;
	}

	void Update() {

		turn();
		move();

	}

	private void turn() {

		Vector2 targetVec = new Vector2(Mathf.Cos(targetRotation), Mathf.Sin(targetRotation));

		float dot = Vector2.Dot(targetVec, transform.right);

		//rb.rotation += ;
		rb.angularVelocity = dot * turnSpeed;

	}

	private void move() {

		rb.AddForce(transform.up * engineThrust * thrust);
		if(thrust != 0) {
			att.InvalidatePrediction();
		}

	}
}
