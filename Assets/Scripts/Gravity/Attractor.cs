using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorReturn {
	public Vector2 force;
	public Attractor strongest;

	public AttractorReturn(Vector2 force, Attractor strongest) {
		this.force = force;
		this.strongest = strongest;
	}

	public static implicit operator Vector2(AttractorReturn a) => a.force;
	public static implicit operator Vector3(AttractorReturn a) => a.force;

}

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
	public static AttractorReturn getAttractionVector(Attracted obj) {

		Vector2 totalForce = new Vector2(0, 0);

		if (attractors == null)
			return new AttractorReturn(new Vector2(0,0), null);

		Attractor strongest = null;
		float strongestForce = 0;

		foreach (Attractor a in attractors) {
			if(obj.gameObject != a.gameObject) {
				Vector2 force = ((Vector2)a.transform.position - (Vector2)obj.transform.position).normalized;
				float forceMag = a.getAttractionForce(obj);
				force *= forceMag;
				totalForce += force;

				if(forceMag > strongestForce) {
					strongestForce = forceMag;
					strongest = a;
				}
			}
		}

		return new AttractorReturn(totalForce, strongest);
	}

	public static AttractorReturn getFutureAttractionVectorSteps(Attracted obj, Vector2 pos, int steps) {

		Vector2 totalForce = new Vector2(0, 0);

		if (attractors == null)
			return new AttractorReturn(new Vector2(0, 0), null);

		Attractor strongest = null;
		float strongestForce = 0;

		foreach (Attractor a in attractors) {
			if (obj.gameObject != a.gameObject) {
				Vector2 force = (a.getPosInPhysicsSteps(steps) - pos).normalized;
				float forceMag = a.getAttractionForceInFutureSteps(pos, obj.rb.mass, steps);
				force *= forceMag;
				totalForce += force;

				if (forceMag > strongestForce) {
					strongestForce = forceMag;
					strongest = a;
				}
			}
		}

		return new AttractorReturn(totalForce, strongest);
	}

	public static AttractorReturn getFutureAttractionVectorSeconds(Attracted obj, Vector2 pos, float seconds) {

		Vector2 totalForce = new Vector2(0, 0);

		if (attractors == null)
			return new AttractorReturn(new Vector2(0, 0), null);

		Attractor strongest = null;
		float strongestForce = 0;

		foreach (Attractor a in attractors) {
			if (obj.gameObject != a.gameObject) {
				Vector2 force = (a.getPosInSeconds(seconds) - pos).normalized;
				float forceMag = a.getAttractionForceInFutureSeconds(pos, obj.rb.mass, seconds);
				force *= forceMag;
				totalForce += force;

				if (forceMag > strongestForce) {
					strongestForce = forceMag;
					strongest = a;
				}
			}
		}

		return new AttractorReturn(totalForce, strongest);
	}

	public virtual Vector2 getPosInPhysicsSteps(int stepsFromNow) {
		return transform.position;
	}

	public virtual Vector2 getPosInSeconds(float secondsFromNow) {
		return transform.position;
	}

}
