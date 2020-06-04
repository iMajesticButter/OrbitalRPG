using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody2D))]
public class PlanetOrbitController : MonoBehaviour {

	public const float LongitudeOfAcendingNode = 0;
	public const float maxIterations = 10;
	public const float targetDecimals = 5;

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

	public float SOI { get; private set; }

	public Rigidbody2D ParentBody;

	public float getAngleAtTime(float time) {
		float meanA = (StartingMeanAnomaly + ((time / orbitalPeriod) * (Mathf.PI * 2))) % (Mathf.PI * 2);

		//convert mean anomaly to true anomaly
		//unfortunately there is no elegent way to do this...

		//calculate eccentric anomaly
		float e = eccentricity;

		float eccA;
		float F;

		if (e < 0.8f)
			eccA = meanA;
		else
			eccA = Mathf.PI;

		F = eccA - e * Mathf.Sin(meanA) - meanA;

		float delta = Mathf.Pow(10, -targetDecimals);

		for (int i = 0; i < maxIterations && (Mathf.Abs(F) > delta); ++i) {
			eccA = eccA - F / (1.0f - e * Mathf.Cos(eccA));
			F = eccA - e * Mathf.Sin(eccA) - meanA;
		}


		//calculate true anomaly
		float sinE = Mathf.Sin(eccA);
		float cosE = Mathf.Cos(eccA);

		float fak = Mathf.Sqrt(1 - (e * e));
		float phi = Mathf.Atan2(fak * sinE, cosE - e);

		return phi;
	}

	public Vector2 getPosAtAngle(float angle) {
		float aopAngle = angle + ArgumentOfPeriapsis;

		Vector2 vec = new Vector2(Mathf.Cos(aopAngle), Mathf.Sin(aopAngle));


		//calculate distance from parent body based on true anomaly
		float e = eccentricity;

		float dist = (semiMajorAxis * (1 - (e * e))) / (1 + (e * Mathf.Cos(angle)));


		return vec * dist;
	}

	public Vector2 getPosAtTime(float time) {
		float angle = getAngleAtTime(time);

		Vector2 pos = ParentBody.position;
		if (ParentOrbit != null) {
			pos = ParentOrbit.getPosAtTime(time);
		}

		return pos + getPosAtAngle(angle);
	}

	public Vector2 getPosInSteps(int steps) {
		Vector2 pos = getPosAtTime(currTime + ((float)steps / (1 / Time.fixedDeltaTime)));
		//Debug.DrawLine(pos, pos + new Vector2(0.1f, 0.0f), Color.green);
		return pos;
	}

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		rb.position = getPosAtTime(0);

		ParentOrbit = ParentBody.GetComponent<PlanetOrbitController>();

		SOI = Mathf.Pow(semiMajorAxis * (rb.mass / ParentBody.mass), 2.0f / 5.0f);
	}

	private void Update() {
		//editor stuff
		if (!Application.isPlaying) {
			SOI = Mathf.Pow(semiMajorAxis * (rb.mass / ParentBody.mass), 2.0f / 5.0f);
			transform.position = getPosAtTime(0);

			for (int i = 0; i < 360; ++i) {
				Vector2 start = getPosAtAngle(i * Mathf.Deg2Rad) + (Vector2)ParentBody.transform.position;
				Vector2 end = getPosAtAngle((i + 1) * Mathf.Deg2Rad) + (Vector2)ParentBody.transform.position;
				Debug.DrawLine(start, end, Color.green);
			}
		}
	}

	private void FixedUpdate() {
		currTime += Time.fixedDeltaTime;
		rb.velocity = new Vector2(0, 0);
		rb.position = getPosAtTime(currTime);
	}
}
