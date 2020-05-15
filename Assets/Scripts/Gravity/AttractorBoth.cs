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
		if(stepsFromNow >= att.prediction.Count) {
			stepsFromNow = att.prediction.Count - 1;
		}
		if (att.prediction.Count == 0)
			return transform.position;
		return att.prediction[stepsFromNow];
	}

	public override Vector2 getPosInSeconds(float secondsFromNow) {
		int index = (int)(secondsFromNow / Time.fixedDeltaTime);
		if (index >= att.prediction.Count) {
			index = att.prediction.Count - 1;
		}
		if (att.prediction.Count == 0)
			return transform.position;
		return att.prediction[index];
	}

}
