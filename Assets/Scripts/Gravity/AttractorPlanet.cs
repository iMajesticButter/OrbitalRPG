using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlanetOrbitController))]
public class AttractorPlanet : Attractor {

	private PlanetOrbitController planet;

	protected override void Init() {
		planet = GetComponent<PlanetOrbitController>();
	}

	public override Vector2 getPosInPhysicsSteps(int stepsFromNow) {
		return planet.getPosInSteps(stepsFromNow);
	}

	public override Vector2 getPosInSeconds(float secondsFromNow) {
		return planet.getPosAtTime(Time.time + secondsFromNow);
	}
}
