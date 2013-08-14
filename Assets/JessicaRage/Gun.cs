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
			Debug.Log ("Fire!");
			
			Vector3 fwd = transform.TransformDirection (Vector3.forward);
			RaycastHit hitInfo = new RaycastHit();
			if (Physics.Raycast (transform.position, fwd, out hitInfo, 10240f)) {
				hitInfo.transform.BroadcastMessage("HitMessage", SendMessageOptions.DontRequireReceiver);
				print ("hit!");
			}
			
			refireTimer = refireRate;
		} else if (refireTimer > 0) {
			refireTimer--;
		}
	}
}
