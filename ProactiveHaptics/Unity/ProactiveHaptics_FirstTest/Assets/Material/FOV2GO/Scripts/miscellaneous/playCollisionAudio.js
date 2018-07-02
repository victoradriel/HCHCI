#pragma strict
@script RequireComponent(AudioSource);

var impactSound : AudioClip;
var minimumMagnitude : float = 2;
var pitchMin : float = 0.25;
var pitchMax : float = 2.0;

function OnCollisionEnter (collision: Collision) {
	if (collision.relativeVelocity.magnitude > minimumMagnitude) {
		GetComponent.<AudioSource>().volume = collision.relativeVelocity.magnitude/30;
		GetComponent.<AudioSource>().pitch = Random.Range(pitchMin,pitchMax);
    	GetComponent.<AudioSource>().PlayOneShot(impactSound);
	}
}