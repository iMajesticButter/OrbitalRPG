using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Attractor : MonoBehaviour {

	public static List<Attractor> attractors = null;

	[HideInInspector]
	public Rigidbody2D rb;

	private void OnDestroy() {
		if(attractors != null)
			attractors.Remove(this);
	}

	void Start() {
		if(attractors == null) {
			attractors = new List<Attractor>();
		}

		attractors.Add(this);
		rb = GetComponent<Rigidbody2D>();

		Init();
    }

	protected virtual void Init() {

	}

	//get attraction force from this object
    public float getAttractionForce(Attracted other) {

		float massProduct = rb.mass * other.rb.mass;

		Vector2 pos = transform.position;
		Vector2 otherPos = other.transform.position;

		float distanceSq = Vector2.SqrMagnitude(pos - otherPos);

		return Gravity.G * (massProduct / distanceSq);
	}

	public float getAttractionForceInFutureSteps(Vector2 pos, float mass, int steps) {

		float massProduct = rb.mass * mass;

		float distanceSq = Vector2.SqrMagnitude(getPosInPhysicsSteps(steps) - pos);

		return Gravity.G * (massProduct / distanceSq);
	}

	public float getAttractionForceInFutureSeconds(Vector2 pos, float mass, float seconds) {

		float massProduct = rb.mass * mass;

		float distanceSq = Vector2.SqrMagnitude(getPosInSeconds(seconds) - pos);

		return Gravity.G * (massProduct / distanceSq);
	}

	//get attraction vector for all objects
	public static Vector2 getAttractionVector(Attracted obj) {

		Vector2 totalForce = new Vector2(0, 0);

		if (attractors == null)
			return new Vector2(0, 0);

		foreach (Attractor a in attractors) {
			if(obj.gameObject != a.gameObject) {
				Vector2 force = ((Vector2)a.transform.position - (Vector2)obj.transform.position).normalized;
				force *= a.getAttractionForce(obj);
				totalForce += force;
			}
		}

		return totalForce;
	}

	public static Vector2 getFutureAttractionVectorSteps(Attracted obj, Vector2 pos, int steps) {

		Vector2 totalForce = new Vector2(0, 0);

		if (attractors == null)
			return new Vector2(0,0);

		foreach (Attractor a in attractors) {
			if (obj.gameObject != a.gameObject) {
				Vector2 force = (a.getPosInPhysicsSteps(steps) - pos).normalized;
				force *= a.getAttractionForceInFutureSteps(pos, obj.rb.mass, steps);
				totalForce += force;
			}
		}

		return totalForce;
	}

	public static Vector2 getFutureAttractionVectorSeconds(Attracted obj, Vector2 pos, float seconds) {

		Vector2 totalForce = new Vector2(0, 0);

		if (attractors == null)
			return new Vector2(0, 0);

		foreach (Attractor a in attractors) {
			if (obj.gameObject != a.gameObject) {
				Vector2 force = (a.getPosInSeconds(seconds) - pos).normalized;
				force *= a.getAttractionForceInFutureSeconds(pos, obj.rb.mass, seconds);
				totalForce += force;
			}
		}

		return totalForce;
	}

	public virtual Vector2 getPosInPhysicsSteps(int stepsFromNow) {
		return transform.position;
	}

	public virtual Vector2 getPosInSeconds(float secondsFromNow) {
		return transform.position;
	}

}
