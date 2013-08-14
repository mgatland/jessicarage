#pragma strict

static var player: Transform;
private var defaultSpeed: float = 28f;
private var speed: float;
private var awake: boolean;
private var isDead: boolean;

private var awakeRange: int = 25;
private var attackRange: int = 5;
private var backoffDist:int = 2;
private var attackDelay:int = 75; //in fixed frames.

private var attackTime:int = 0;

var awakeSound: AudioClip;
var strikeSound: AudioClip;
var dieSound: AudioClip;
var knockSound: AudioClip;

var losMask: int;

function OnCollisionEnter(collision : Collision) { 
	//sounds:
	//no sound for hitting player
	if (collision.gameObject.tag == "Player") return;
	//no sound for hitting other enemies
	if (collision.gameObject.layer == "Enemy") return;

    // Play a sound if the coliding objects had a big impact.        
    if (collision.relativeVelocity.magnitude > 2) {
			AudioSource.PlayClipAtPoint(knockSound, transform.position);
		} else {
			//print ("Minor collision, no sound.");
		}
		
}

function Start() {
	if (player == null) {
		player = gameObject.FindWithTag("Player").transform;
	}
	
	if (losMask == 0) {
		//actually: enemies see through everything except the world.
		losMask = 1;

	}
	
	speed = defaultSpeed; // * (0.8f + Random.value * 0.2f);
}

function FixedUpdate () {
	if (!awake) {
		if (!CanSeeTarget()) return;
		awake= true;
		AudioSource.PlayClipAtPoint(awakeSound, transform.position);
		print("awake");
		//audio.PlayOneShot(awakeSound);
	}
	
	if (CanSeeTarget()) {
		var seekDir:Vector3 = (player.transform.position + Vector3.up * 1) - transform.position;
		if (seekDir.magnitude > backoffDist) rigidbody.AddForce(seekDir.normalized * speed);
		if (CanAttack()) {
			Attack();
		} else {
			StopAttack();
		}
	} else {
		StopAttack();
		//when blocked, try going upwards
		seekDir = (player.transform.position + Vector3.up * 4) - transform.position;
		rigidbody.AddForce(seekDir.normalized * speed);
	}
}

function CanSeeTarget () : boolean
{
	if (!awake && Vector3.Distance(transform.position, player.position) > awakeRange) return false;
	var hit : RaycastHit;
	if (Physics.Linecast (transform.position, player.position, hit, losMask)) {
		return hit.transform == player;
	}
	//second chance - look for tip of player's head
	if (Physics.Linecast (transform.position, player.position + Vector3.up * 1.1, hit, losMask)) {
		return hit.transform == player;
	}
	return false;
}

function CanAttack(): boolean {
	//assumes we can see  and are awake
	if (Vector3.Distance(transform.position, player.position) > attackRange) return false;
	return true;
}

function Attack() {
	attackTime++; //this is called with the fixed physics framerate so doesn't need delta
	audio.Play();
	if (attackTime > attackDelay) {
		AudioSource.PlayClipAtPoint(strikeSound, transform.position);
		StopAttack();
	}
}

function StopAttack() {
	attackTime = 0;
	audio.Stop();
}

function HitMessage() {
	if (!awake) return;
	StopAttack();
	AudioSource.PlayClipAtPoint(dieSound, transform.position);
	
	isDead = true;
	//fade out this dead creature.
	rigidbody.useGravity = true;
	rigidbody.drag = 0;
	rigidbody.angularDrag = 0.05;
	yield WaitForSeconds(15);
	Destroy(gameObject);
}