using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody2D))]
public class PlanetOrbitController : MonoBehaviour {

	public const float LongitudeOfAcendingNode = 0;
	public const float maxIterations = 30;
	public const float decimalPlaces = 30;

	[Header("Orbit Properties")]
	public float apoapsisHeight;
	public float periapsisHeight;

	[Range(0, Mathf.PI * 2)]
	public float ArgumentOfPeriapsis;
	[Range(0, Mathf.PI * 2)]
	public float StartingMeanAnomaly;

	public Rigidbody2D rb { get; private set; }
	private PlanetOrbitController ParentOrbit;

	private float currTime = 0;

	public float semiMajorAxis {
		get {
			return (periapsisHeight + apoapsisHeight) / 2;
		}
	}

	public float eccentricity {
		get {
			return 1 - (2 / ((apoapsisHeight / periapsisHeight) + 1));
		}
	}

	public float orbitalPeriod {
		get {
			float a = semiMajorAxis;
			return (2 * Mathf.PI) * Mathf.Sqrt((a * a * a) / Gravity.getSGP(ParentBody.mass));
		}
	}

	public Rigidbody2D ParentBody;

	public float getAngleAtTime(float time) {
		float meanA = (StartingMeanAnomaly + ((time / orbitalPeriod) * (Mathf.PI*2))) % (Mathf.PI*2);
		return meanA;

	}

	public Vector2 getPosAtTime(float time) {
		float angle = getAngleAtTime(time);

		Vector2 pos = ParentBody.position;
		if(ParentOrbit != null) {
			pos = ParentOrbit.getPosAtTime(time);
		}
		Vector2 vec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		float dist = periapsisHeight;
		

		return pos + (vec * dist);
	}

	public Vector2 getPosInSteps(int steps) {
		Vector2 pos = getPosAtTime(currTime + ((float)steps / (1 / Time.fixedDeltaTime)));
		Debug.DrawLine(pos, pos + new Vector2(0.1f, 0.0f), Color.green);
		return pos;
	}

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		rb.position = getPosAtTime(0);

		ParentOrbit = ParentBody.GetComponent<PlanetOrbitController>();
	}

	private void Update() {
		if(!Application.isPlaying) {
			transform.position = getPosAtTime(0);
		}
	}

	private void FixedUpdate() {
		currTime += Time.fixedDeltaTime;
		rb.velocity = new Vector2(0, 0);
		rb.position = getPosAtTime(currTime);

		for(int i = 0; i < 1000; ++i) {
			getPosInSteps(i);
		}
	}
}
