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

	public Material lineMat;

	public LineColorSettings[] lineColors;



	public Attracted att { get; private set; }


	private List<LineRenderer> lines = new List<LineRenderer>();

	void Start() {
		att = GetComponent<Attracted>();
	}

	
	void Update() {
		//make sure the prediction is up to date
		att.CalculatePrediction();

		//make sure we have the correct number of lines
		//set the lines verticies
		List<Vector3> positions = new List<Vector3>();

		Attractor currentSOI = null;
		int lineIndex = -1;

		for(int i = 0; i < att.prediction.Count; ++i) {
			if(currentSOI != att.prediction[i].strongestAttractor) {
				currentSOI = att.prediction[i].strongestAttractor;
				setLineVerts(lineIndex, positions);
				++lineIndex;
				validateLineIndex(lineIndex);

				positions.Clear();
			}
			positions.Add(currentSOI.transformPositionReletiveSteps(att.prediction[i], Attracted.getFutureStepsFromPredictionIndex(i)));
		}

		setLineVerts(lineIndex, positions);

		clearAllLinesAfterIndex(lineIndex);

	}

	private void clearAllLinesAfterIndex(int lineIndex) {
		for(int i = lineIndex+1; i < lines.Count; ++i) {
			lines[i].positionCount = 0;
		}
	}

	private void setLineVerts(int lineIndex, List<Vector3> verts) {
		if(lineIndex >= 0 && lineIndex < lines.Count) {
			LineRenderer lr = lines[lineIndex];

			if(lr.positionCount != verts.Count) {
				lr.positionCount = verts.Count;
			}

			lr.SetPositions(verts.ToArray());
		}
	}

	private void validateLineIndex(int i) {
		if(i >= lines.Count) {
			//create a new line object
			GameObject lineObj = new GameObject("line");
			LineRenderer newline = lineObj.AddComponent<LineRenderer>();
			lineObj.transform.SetParent(transform);

			LineColorSettings color = lineColors[i % lineColors.Length];

			newline.startColor = color.startColor;
			newline.endColor = new Color(color.startColor.r, color.startColor.g, color.startColor.b, color.endAlpha);

			newline.startWidth = lineWidthStart;
			newline.endWidth = lineWidthEnd;

			newline.material = lineMat;

			newline.numCapVertices = lineCapVerts;

			lines.Add(newline);
		}
	}
}
