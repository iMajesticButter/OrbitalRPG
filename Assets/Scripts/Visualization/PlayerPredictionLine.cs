using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LineColorSettings {
	public Color startColor = Color.green;
	[Range(0, 1)]
	public float endAlpha = 0.25f;
}

[RequireComponent(typeof(Attracted))]
public class PlayerPredictionLine : MonoBehaviour {

	[Header("Line Appearance")]
	public float lineWidthStart = 0.25f;
	public float lineWidthEnd = 0.1f;
	public int lineCapVerts = 3;

	public float maxRotationsPerPlanet = 2.0f;

	public float futureAlphaMultiplier = 0.5f;

	public int linePointSkip = 0;
	public int linePointInc {
		get {
			return linePointSkip + 1;
		}
	}

	public Material lineMat;

	public LineColorSettings[] lineColors;

	[Header("Widget Appearance")]
	public float widgetSize = 1.0f;

	public Sprite TransferNodeWidget;
	public Sprite ApoapsisWidget;
	public Sprite PeriapsisWidget;
	public Sprite CollisionWidget;



	public Attracted att { get; private set; }


	private List<LineRenderer> lines = new List<LineRenderer>();

	void Start() {
		att = GetComponent<Attracted>();
	}

	private float calcAngle(int index, Attractor orbitingBody) {
		if (att.prediction.Count == 0)
			return 0;

		int steps = Attracted.getFutureStepsFromPredictionIndex(index);

		Vector2 vec = orbitingBody.getPosInPhysicsSteps(steps) - att.prediction[index];

		return Mathf.Atan2(vec.y, vec.x);
	}

	void Update() {
		//make sure the prediction is up to date
		att.CalculatePrediction();

		//make sure we have the correct number of lines
		//set the lines verticies
		List<Vector3> positions = new List<Vector3>();

		Attractor currentSOI = null;
		int lineIndex = -1;

		//TODO: count rotation around each body and stop drawing the line if it passes a set threshold

		float lastAngle = calcAngle(0, att.getOrbitingBody());
		float totalAngle = 0;
		bool stop = false;

		for (int i = 0; i < att.prediction.Count; i += linePointInc) {
			if (currentSOI != att.prediction[i].strongestAttractor || i == att.prediction.Count-1 || stop) {
				setLineVerts(lineIndex, positions);
				++lineIndex;
				validateLineIndex(lineIndex);

				if (positions.Count != 0) {
					if(currentSOI != att.prediction[i].strongestAttractor)
						WidgetDisplay.getWidgetDisplay().DisplayWidget(TransferNodeWidget, positions[positions.Count - 1], new Vector2(widgetSize, widgetSize), lineColors[lineIndex % lineColors.Length].startColor);

					//draw apoapsis and periapsis
					OrbitProperties properties = att.getOrbitProperties(currentSOI, i - positions.Count);
					if (properties != null) {

						Vector2 apoapsisPos = att.prediction[Attracted.getPredictionIndexFromFutureSteps(properties.stepsToApoapsis)];
						Vector2 periapsisPos = att.prediction[Attracted.getPredictionIndexFromFutureSteps(properties.stepsToPeriapsis)];

						apoapsisPos = currentSOI.transformPositionReletiveSteps(apoapsisPos, Attracted.getFutureStepsFromPredictionIndex(properties.stepsToApoapsis));
						periapsisPos = currentSOI.transformPositionReletiveSteps(periapsisPos, Attracted.getFutureStepsFromPredictionIndex(properties.stepsToPeriapsis));

						if(properties.stepsToApoapsis != 0)
							WidgetDisplay.getWidgetDisplay().DisplayWidget(ApoapsisWidget, apoapsisPos,
								new Vector2(widgetSize, widgetSize), lineColors[(lineIndex - 1) % lineColors.Length].startColor);

						if (properties.stepsToPeriapsis != 0)
							WidgetDisplay.getWidgetDisplay().DisplayWidget(PeriapsisWidget, periapsisPos,
								new Vector2(widgetSize, widgetSize), lineColors[(lineIndex - 1) % lineColors.Length].startColor);

					}

				}

				currentSOI = att.prediction[i].strongestAttractor;
				positions.Clear();

				lastAngle = calcAngle(i, currentSOI);
				totalAngle = 0;

				if (stop)
					break;
			}

			Vector2 pos = currentSOI.transformPositionReletiveSteps(att.prediction[i], Attracted.getFutureStepsFromPredictionIndex(i));

			if (i == att.prediction.Count - 1 && att.IntersectionDetected) {
				WidgetDisplay.getWidgetDisplay().DisplayWidget(CollisionWidget, pos, new Vector2(widgetSize, widgetSize), Color.red);
			}

			float angle = calcAngle(i, currentSOI);

			if(Mathf.Abs(angle - lastAngle) < Mathf.PI) {
				totalAngle += Mathf.Abs(angle - lastAngle);
			}

			
			lastAngle = angle;

			if (totalAngle >= (Mathf.PI*2) * (maxRotationsPerPlanet)) {
				stop = true;
			}

			positions.Add(pos);
		}

		//setLineVerts(lineIndex, positions);

		clearAllLinesAfterIndex(lineIndex-1);

	}

	private void clearAllLinesAfterIndex(int lineIndex) {
		for (int i = lineIndex + 1; i < lines.Count; ++i) {
			lines[i].positionCount = 0;
		}
	}

	private void setLineVerts(int lineIndex, List<Vector3> verts) {
		if (lineIndex >= 0 && lineIndex < lines.Count) {
			LineRenderer lr = lines[lineIndex];

			if (lr.positionCount != verts.Count) {
				lr.positionCount = verts.Count;
			}

			lr.SetPositions(verts.ToArray());

			//update line thickness
			lr.startWidth = lineWidthStart * CameraFollow.scaleMultMatchScreen;
			lr.endWidth = lineWidthEnd * CameraFollow.scaleMultMatchScreen;
		}
	}

	private void validateLineIndex(int i) {
		while (i >= lines.Count) {
			//create a new line object
			GameObject lineObj = new GameObject("line");
			LineRenderer newline = lineObj.AddComponent<LineRenderer>();
			lineObj.transform.SetParent(transform);

			LineColorSettings color = lineColors[(lines.Count) % lineColors.Length];

			float startAlpha = color.startColor.a;
			float endAlpha = color.endAlpha;

			if (i != 0) {
				startAlpha *= futureAlphaMultiplier;
				endAlpha *= futureAlphaMultiplier;
			}

			newline.startColor = new Color(color.startColor.r, color.startColor.g, color.startColor.b, startAlpha);
			newline.endColor = new Color(color.startColor.r, color.startColor.g, color.startColor.b, endAlpha);

			newline.startWidth = lineWidthStart;
			newline.endWidth = lineWidthEnd;

			newline.material = lineMat;

			newline.numCapVertices = lineCapVerts;

			newline.sortingOrder = -1;

			lines.Add(newline);
		}
	}
}
