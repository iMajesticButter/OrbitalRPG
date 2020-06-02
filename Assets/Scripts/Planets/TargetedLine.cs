using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlanetOrbitController))]
public class TargetedLine : MonoBehaviour {

	private LineRenderer lr;
	private PlanetOrbitController orbit;

	public float lineWidth = 0.05f;

	public Material lineMat;
	public Color lineColor = Color.grey;
	public Color lineTargetedColor = Color.green;

	void Start() {
		GameObject lrOBJ = new GameObject("line");
		lrOBJ.transform.SetParent(transform);
		lr = lrOBJ.AddComponent<LineRenderer>();

		orbit = GetComponent<PlanetOrbitController>();

		lr.loop = true;

		lr.positionCount = 360;

		lr.material = lineMat;

		lr.startColor = lineColor;
		lr.endColor = lineColor;

		lr.useWorldSpace = false;

		lr.sortingOrder = -2;

		for(int i = 0; i < 360; ++i) {
			lr.SetPosition(i, orbit.getPosAtAngle(i * Mathf.Deg2Rad));
		}

	}

	public void SetAsTarget(GameObject obj) {
		if(obj == PlayerShipController.playerShip.gameObject) {
			lr.startColor = lineTargetedColor;
			lr.endColor = lineTargetedColor;
		}
	}
	public void UnsetAsTarget(GameObject obj) {
		if (obj == PlayerShipController.playerShip.gameObject) {
			lr.startColor = lineColor;
			lr.endColor = lineColor;
		}
	}


	void Update() {
		if(lr.enabled) {
			lr.startWidth = lineWidth * CameraFollow.scaleMultMatchScreen;
			lr.endWidth = lineWidth * CameraFollow.scaleMultMatchScreen;

			if(orbit.ParentBody != null) {
				lr.transform.position = orbit.ParentBody.transform.position;
			}

		}
	}
}
