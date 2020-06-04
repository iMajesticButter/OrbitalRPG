using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMapWidget : MonoBehaviour {

	public Sprite planetIcon;

	public Color iconColor = Color.gray;
	public Color iconTargetedColor = Color.green;

	private Color col;

	private bool targeted = false;

	public static float iconSize = 0.2f;

	void Start() {
		col = iconColor;
	}

	void Update() {

		float realSize = CameraFollow.scaleMultMatchScreen * iconSize;

		if (realSize > transform.lossyScale.x || targeted) {

			WidgetDisplay.getWidgetDisplay().DisplayWidget(planetIcon, transform.position, new Vector2(iconSize, iconSize), col);

		}
		
	}

	public void SetAsTarget(GameObject obj) {
		if (obj == PlayerShipController.playerShip.gameObject) {
			col = iconTargetedColor;
			targeted = true;
		}
	}
	public void UnsetAsTarget(GameObject obj) {
		if (obj == PlayerShipController.playerShip.gameObject) {
			col = iconColor;
			targeted = false;
		}
	}
}
