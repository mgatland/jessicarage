using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
	
	private int refireRate = 15;
	private int refireTimer = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	void FixedUpdate () {
		if (Input.GetButton("Fire1") && refireTimer == 0) {
			Debug.Log ("Fire1");
			refireTimer = refireRate;
		} else if (refireTimer > 0) {
			refireTimer--;
		}
	}
}
