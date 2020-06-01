using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class WidgetData {
	public Vector3 pos;
	public Vector2 scale;
	public Sprite sprite;
	public Color tint;

	public WidgetData(Vector3 pos, Vector2 scale, Sprite sprite, Color tint) {
		this.pos = pos;
		this.scale = scale;
		this.sprite = sprite;
		this.tint = tint;
	}
}

public class WidgetDisplay : MonoBehaviour {

	//singleton component
	private static WidgetDisplay display = null;

	public static WidgetDisplay getWidgetDisplay() {
		if(display == null) {
			GameObject obj = new GameObject("WidgetDisplay");

			obj.transform.position = Vector3.zero;
			obj.transform.localScale = Vector3.one;

			display = obj.AddComponent<WidgetDisplay>();
		}
		return display;
	}

	private WidgetDisplay() {

	}
	//----------------

	private List<SpriteRenderer> widgets = new List<SpriteRenderer>();
	private List<WidgetData> widgetsToRender = new List<WidgetData>();

	public void DisplayWidget(Sprite sprite, Vector3 pos, Vector2 scale, Color tint) {
		widgetsToRender.Add(new WidgetData(pos, scale, sprite, tint));
	}

	private void Update() {
		renderWidgets();
	}

	private void renderWidgets() {

		validateWidgetIndex(widgetsToRender.Count);

		DisableAllWidgets();

		for(int i = 0; i < widgetsToRender.Count; ++i) {

			widgets[i].transform.position = widgetsToRender[i].pos;
			widgets[i].transform.localScale = widgetsToRender[i].scale * CameraFollow.scaleMultMatchScreen;
			widgets[i].sprite = widgetsToRender[i].sprite;
			widgets[i].color = widgetsToRender[i].tint;

			widgets[i].gameObject.SetActive(true);

		}

		widgetsToRender.Clear();

	}

	private void DisableAllWidgets() {
		for(int i = 0; i < widgets.Count; ++i) {
			widgets[i].gameObject.SetActive(false);
		}
	}

	private void validateWidgetIndex(int i) {
		while(i >= widgets.Count) {
			//create new widget object
			GameObject widgetObj = new GameObject("Widget");

			widgetObj.transform.SetParent(display.transform);

			SpriteRenderer widget = widgetObj.AddComponent<SpriteRenderer>();

			widget.sortingOrder = 10;

			widget.gameObject.SetActive(false);

			widgets.Add(widget);
		}
	}

}
