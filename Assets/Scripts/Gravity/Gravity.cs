using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Gravity {

	public static float G = 66.74f;

	public static float getSGP(float mass) {
		return G * mass;
	}

}
