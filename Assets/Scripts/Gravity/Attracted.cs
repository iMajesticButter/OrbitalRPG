using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PredictionStep {
	
	public Vector2 pos;
	public Vector2 vel;

	public PredictionStep(Vector2 pos, Vector2 vel) {
		this.pos = pos;
		this.vel = vel;
	}

	public static implicit operator Vector2(PredictionStep s) => s.pos;
	public static implicit operator Vector3(PredictionStep s) => s.pos;
}

[RequireComponent(typeof(Rigidbody2D))]
public class Attracted : MonoBehaviour {

	public const int numPre = 10000;

	[HideInInspector]
	public Rigidbody2D rb;

	[HideInInspector]
	public Attractor myAttractor;

	public List<PredictionStep> prediction = new List<PredictionStep>();

	public List<Vector2> getPosArray() {
		List<Vector2> newList = new List<Vector2>(prediction.Count);
		foreach(PredictionStep step in prediction) {
			newList.Add(step);
		}
		return newList;
	}

	public List<Vector3> getPosArrayVec3() {
		List<Vector3> newList = new List<Vector3>(prediction.Count);
		foreach (PredictionStep step in prediction) {
			newList.Add(step);
		}
		return newList;
	}


	private bool validPrediction = false;

	private Vector2 calc_pos = new Vector2(0, 0);
	private Vector2 calc_vel = new Vector2(0, 0);

	private int stepsSinceLastPrediction = 0;


	//list of attracteds that are attached to a game object that contains an attractor
	public static List<Attracted> BothAttr = null;

	private void OnDestroy() {
		if (BothAttr != null && myAttractor != null)
			BothAttr.Remove(this);
	}

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		myAttractor = GetComponent<Attractor>();
		if(myAttractor != null) {
			if(BothAttr == null) {
				BothAttr = new List<Attracted>();
			}
			BothAttr.Add(this);
		}
	}

	void Update() {

	}

	void FixedUpdate() {
		
		validPrediction = false;
		++stepsSinceLastPrediction;

		Vector2 force = Attractor.getAttractionVector(this);
		rb.AddForce(force);
		//rb.velocity += (force / rb.mass) * Time.fixedDeltaTime;

	}

	public void CalculatePrediction() {
		if (validPrediction) {
			//already calculated
			return;
		}
		
		CalculateBothPrediction();
		if (myAttractor == null) {

			int startIndex = 0;
			Vector2 pos;
			Vector2 vel;

			if (prediction.Count != 0 && prediction[stepsSinceLastPrediction-1] == rb.position) {
				startIndex = prediction.Count - stepsSinceLastPrediction;
				pos = prediction[prediction.Count - 1].pos;
				vel = prediction[prediction.Count - 1].vel;
				prediction.RemoveRange(0, stepsSinceLastPrediction);
			} else {
				prediction.Clear();
				pos = rb.position;
				vel = rb.velocity;
			}

			for (int i = startIndex; i < numPre; ++i) {
				//calculate prediction step

				Vector2 force = Attractor.getFutureAttractionVectorSteps(this, pos, i);
				vel += (force / rb.mass) * Time.fixedDeltaTime;
				pos += vel * Time.fixedDeltaTime;

				prediction.Add(new PredictionStep(pos, vel));
			}
		}
		validPrediction = true;
		stepsSinceLastPrediction = 0;
	}

	public static void CalculateBothPrediction() {
		if (BothAttr == null || BothAttr.Count == 0 || BothAttr[0].validPrediction)
			return;

		for (int i = 0; i < numPre; ++i) {
			for (int j = 0; j < BothAttr.Count; ++j) {
				//calculate prediction step including the movement of attractor objects
				if (i == 0) {
					BothAttr[j].calc_pos = BothAttr[j].transform.position;
					BothAttr[j].calc_vel = BothAttr[j].rb.velocity;
					BothAttr[j].prediction.Clear();
					BothAttr[j].validPrediction = true;
					BothAttr[j].stepsSinceLastPrediction = 0;
				}

				Vector2 force = Attractor.getFutureAttractionVectorSteps(BothAttr[j], BothAttr[j].calc_pos, i);
				BothAttr[j].calc_vel += (force / BothAttr[j].rb.mass) * Time.fixedDeltaTime;
				BothAttr[j].calc_pos += BothAttr[j].calc_vel * Time.fixedDeltaTime;

				BothAttr[j].prediction.Add(new PredictionStep(BothAttr[j].calc_pos, BothAttr[j].calc_vel));

			}
		}
	}
}
