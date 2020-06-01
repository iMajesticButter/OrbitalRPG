using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Attracted))]
public class AttractorBoth : Attractor {

	private Attracted att;

	protected override void Init() {
		att = GetComponent<Attracted>();
	}

	public override Vector2 getPosInPhysicsSteps(int stepsFromNow) {
		return att.getPosInPhysicsSteps(stepsFromNow);
	}

	public override Vector2 getPosInSeconds(float secondsFromNow) {
		return att.getPosInSeconds(secondsFromNow);
	}

}
