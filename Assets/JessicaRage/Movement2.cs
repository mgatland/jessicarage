using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
 
public class Movement2 : MonoBehaviour {
 
	private float speed = 10.0f;
	private float gravity = 10.0f;
	private float maxVelocityChange = 1.0f;
	private float airControl = 0.5f;
	private bool grounded = false;
	
	//jetpack
	private bool canJump = true;
	private float thrust = 12f; //10 is gravity.
	private float maxFuel = 100f;
	private float fuel = 100f;
 
	//Fuel bar
	void OnGUI() {
    	GUI.Box(new Rect(10, Screen.height - 30, fuel/maxFuel*Screen.width - 20, 20), "Fuel");
	}	
	
	
	public int getFuel() {
		return (int)(fuel * 100f / maxFuel);	
	}
	
	void Awake () {
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity = false;
	}
 
	void FixedUpdate () {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
		float currentMaxVelocityChange = maxVelocityChange * (grounded ? 1.0f : airControl); 
        velocityChange.x = Mathf.Clamp(velocityChange.x, -currentMaxVelocityChange, currentMaxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -currentMaxVelocityChange, currentMaxVelocityChange);
        velocityChange.y = 0;
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        // Jump
        if (fuel > 0 && canJump && Input.GetButton("Jump")) {
			float force = thrust;
			if (grounded) force *= 4;
            rigidbody.AddForce(new Vector3(0, force, 0));
			fuel--;
			if (fuel < 0) fuel = 0;
        } else {
			fuel+= 2;
			if (fuel > maxFuel) fuel = maxFuel;
		}
 
	    // We apply gravity manually for more tuning control
	    rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
 
	    grounded = false;
	}
 
	void OnCollisionStay () {
	    grounded = true;    
	}

}