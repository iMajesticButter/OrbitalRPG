﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Attracted))]
[RequireComponent(typeof(LineRenderer))]
public class DebugPredictionLine : MonoBehaviour {

	Attracted att;
	LineRenderer lr;

	void Start() {
		att = GetComponent<Attracted>();
		lr = GetComponent<LineRenderer>();
	}

	void Update() {

		att.CalculatePrediction();

		if(lr.positionCount != att.prediction.Count)
			lr.positionCount = att.prediction.Count;

		lr.SetPositions(att.prediction.ToArray());

	}
}