using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OrbitProperties {

	public int stepsToApoapsis;
	public float timeToApoapsis {
		get {
			return stepsToApoapsis * Time.fixedDeltaTime;
		}
	}

	public int stepsToPeriapsis;
	public float timeToPeriapsis {
		get {
			return stepsToPeriapsis * Time.fixedDeltaTime;
		}
	}

	public float ApoapsisHeight;
	public float PeriapsisHeight;

	public Attractor orbitingBody;
}

public class PredictionStep {

	public Vector2 pos;
	public Vector2 vel;
	public Attractor strongestAttractor;

	public PredictionStep(Vector2 pos, Vector2 vel, Attractor strongestAttractor) {
		this.pos = pos;
		this.vel = vel;
		this.strongestAttractor = strongestAttractor;
	}

	public static implicit operator Vector2(PredictionStep s) => s.pos;
	public static implicit operator Vector3(PredictionStep s) => s.pos;
}

[RequireComponent(typeof(Rigidbody2D))]
public class Attracted : MonoBehaviour {

	//number of physics steps in the future to simulate when calculating preditions
	public const int numPre = 20000;

	//number of physics steps to skip when calculating predictions
	//larger numbers are faster but less accurate
	public const int numStepSkip = 0;

	//required distance between the actual position and the predicted position required before recalculating
	public const float errorThreshold = 0.25f;

	public const float maxPredictionsPerUpdate = 1000;


	public const float errorThresholdSquared = errorThreshold * errorThreshold;


	private const int stepInc = 1 + numStepSkip;

	private bool invalidPrediction = false;

	[HideInInspector]
	public bool PredictionCalculationDone {
		get {
			return prediction.Count == (numPre / stepInc) || IntersectionDetected;
		}
	}

	[HideInInspector]
	public bool IntersectionDetected { get; private set; }
	[HideInInspector]
	public Rigidbody2D rb;

	[HideInInspector]
	public Attractor myAttractor;

	public List<PredictionStep> prediction = new List<PredictionStep>();

	public List<Vector2> getPosArray() {
		List<Vector2> newList = new List<Vector2>(prediction.Count);
		foreach (PredictionStep step in prediction) {
			newList.Add(step);
		}
		return newList;
	}

	public List<Vector3> getPosArrayVec3() {
		List<Vector3> newList = new List<Vector3>(prediction.Count);
		int i = 0;
		foreach (PredictionStep step in prediction) {
			++i;
			newList.Add(step);
		}
		return newList;
	}

	public Attractor getOrbitingBody() {
		if (prediction.Count == 0)
			return null;
		return prediction[0].strongestAttractor;
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
		if (myAttractor != null) {
			if (BothAttr == null) {
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

	public void InvalidatePrediction() {
		validPrediction = false;
		invalidPrediction = true;
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

			if (!invalidPrediction && prediction.Count != 0) {
				if (stepsSinceLastPrediction < stepInc) {
					prediction[0].pos = transform.position;
					return;
				}
				//else if (stepInc != 1) {
					//prediction.RemoveAt(0);
				//}
			}

			int preindex = stepsSinceLastPrediction / stepInc;

			if (!invalidPrediction && prediction.Count != 0 && preindex < prediction.Count && ((prediction.Count != (numPre / stepInc) && !IntersectionDetected) || (Vector2.SqrMagnitude(prediction[preindex - 1] - rb.position) <= errorThresholdSquared))) {
				startIndex = prediction.Count - preindex;
				pos = prediction[prediction.Count - 1].pos;
				vel = prediction[prediction.Count - 1].vel;
				prediction.RemoveRange(0, preindex);
			}
			else {
				prediction.Clear();
				pos = rb.position;
				vel = rb.velocity;
			}

			IntersectionDetected = false;
			for (int i = startIndex * stepInc, count = 0; i < numPre && count < maxPredictionsPerUpdate; i += stepInc, ++count) {
				//calculate prediction step

				AttractorReturn force = Attractor.getFutureAttractionVectorSteps(this, pos, i);
				vel += (force.force / rb.mass) * (Time.fixedDeltaTime * stepInc);
				pos += vel * (Time.fixedDeltaTime * stepInc);

				//check for intersection with a planet
				foreach (Attractor att in Attractor.attractors) {
					float distSquared = Vector2.SqrMagnitude(pos - att.getPosInPhysicsSteps(i));
					float attScale = att.transform.localScale.x / 2;
					if (distSquared <= attScale * attScale) {
						IntersectionDetected = true;
						break;
					}
				}
				if(IntersectionDetected) {
					break;
				}
				prediction.Add(new PredictionStep(pos, vel, force.strongest));
			}
		}
		validPrediction = true;
		invalidPrediction = false;
		stepsSinceLastPrediction = 0;
	}

	public static void CalculateBothPrediction() {
		if (BothAttr == null || BothAttr.Count == 0 || BothAttr[0].validPrediction)
			return;

		for (int i = 0; i < numPre; i += stepInc) {
			for (int j = 0; j < BothAttr.Count; ++j) {
				//calculate prediction step including the movement of attractor objects
				if (i == 0) {
					BothAttr[j].calc_pos = BothAttr[j].transform.position;
					BothAttr[j].calc_vel = BothAttr[j].rb.velocity;
					BothAttr[j].prediction.Clear();
					BothAttr[j].validPrediction = true;
					BothAttr[j].stepsSinceLastPrediction = 0;
				}

				AttractorReturn force = Attractor.getFutureAttractionVectorSteps(BothAttr[j], BothAttr[j].calc_pos, i);
				BothAttr[j].calc_vel += (force.force / BothAttr[j].rb.mass) * (Time.fixedDeltaTime * stepInc);
				BothAttr[j].calc_pos += BothAttr[j].calc_vel * (Time.fixedDeltaTime * stepInc);

				BothAttr[j].prediction.Add(new PredictionStep(BothAttr[j].calc_pos, BothAttr[j].calc_vel, force.strongest));

			}
		}
	}


	public OrbitProperties getOrbitProperties(Attractor orbitingBody) {
		return getOrbitProperties(orbitingBody, 0);
	}

	public OrbitProperties getOrbitProperties(Attractor orbitingBody, int startingIndex) {
		if (orbitingBody == null)
			return null;

		int soiEnterIndex = startingIndex;
		bool found = false;

		for(int i = soiEnterIndex; i < prediction.Count; ++i) {
			if(prediction[i].strongestAttractor == orbitingBody) {
				soiEnterIndex = i;
				found = true;
				break;
			}
		}

		if(!found) {
			// we do not pass through this attractors soi in the prediction
			return null;
		}

		int periapsisIndex = soiEnterIndex;
		bool periapsisFound = false;

		int apoapsisIndex = soiEnterIndex;
		bool apoapsisFound = false;

		float distSQ1 = 0;
		float distSQ2 = 0;
		float distSQ3 = 0;

		//find apoapsis and periapsis
		for (int i = soiEnterIndex; i < prediction.Count; ++i) {
			if(distSQ1 != 0) {
				if ((periapsisFound && apoapsisFound) || prediction[i].strongestAttractor != orbitingBody)
					break;

				if (distSQ1 > distSQ2 && distSQ2 < distSQ3) {
					//periapsis found
					periapsisFound = true;
					periapsisIndex = i - 1;
				}

				if (!apoapsisFound) {
					apoapsisIndex = i - 1;
					if (distSQ1 < distSQ2 && distSQ2 > distSQ3) {
						//apoapsis found
						apoapsisFound = true;
					}
				}
			}
			distSQ1 = distSQ2;
			distSQ2 = distSQ3;
			distSQ3 = Vector2.SqrMagnitude(prediction[i] - orbitingBody.getPosInPhysicsSteps(getFutureStepsFromPredictionIndex(i)));
		}

		OrbitProperties properties = new OrbitProperties();

		properties.stepsToApoapsis = getFutureStepsFromPredictionIndex(apoapsisIndex);
		properties.stepsToPeriapsis = getFutureStepsFromPredictionIndex(periapsisIndex);

		properties.ApoapsisHeight = Vector2.Distance(prediction[apoapsisIndex], orbitingBody.getPosInPhysicsSteps(getFutureStepsFromPredictionIndex(apoapsisIndex)));
		properties.PeriapsisHeight = Vector2.Distance(prediction[periapsisIndex], orbitingBody.getPosInPhysicsSteps(getFutureStepsFromPredictionIndex(periapsisIndex)));

		properties.orbitingBody = orbitingBody;

		if (!apoapsisFound)
			properties.stepsToApoapsis = 0;
		if (!periapsisFound)
			properties.stepsToPeriapsis = 0;

		return properties;
	}


	public Vector2 getPosInPhysicsSteps(int stepsFromNow) {

		int index = getPredictionIndexFromFutureSteps(stepsFromNow);

		if (index >= prediction.Count) {
			index = prediction.Count - 1;
		}
		if (prediction.Count == 0)
			return transform.position;
		return prediction[index].pos;
	}

	public Vector2 getPosInSeconds(float secondsFromNow) {
		return getPosInPhysicsSteps((int)(secondsFromNow / Time.fixedDeltaTime));
	}

	public static int getFutureStepsFromPredictionIndex(int predictionIndex) {
		return predictionIndex * stepInc;
	}

	public static int getPredictionIndexFromFutureSteps(int futureSteps) {
		return futureSteps / stepInc;
	}

}
