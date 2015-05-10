﻿

using UnityEngine;
using System.Collections;

/// <summary>
/// C# translation from http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html
/// Author: UA @aldonaletto 
/// </summary>

// Prequisites: create an empty GameObject, attach to it a Rigidbody w/ UseGravity unchecked
// To empty GO also add BoxCollider and this script. Makes this the parent of the Player
// Size BoxCollider to fit around Player model.
public abstract class baseEarthMove : MonoBehaviour
{
	
	
	protected float moveSpeed = 6; // move speed
	public float turnSpeed = 90; // turning speed (degrees/second)
	protected float lerpSpeed = 10; // smoothing speed
	protected float gravity = 10; // gravity acceleration
	protected bool isGrounded;
	protected float deltaGround = 0.2f; // character is grounded up to this distance
	protected float jumpSpeed = 10; // vertical jump initial speed
	protected float jumpRange = 10; // range to detect target wall
	protected Vector3 surfaceNormal; // current surface normal
	protected Vector3 myNormal; // character normal
	protected float distGround; // distance from character position to ground
	protected bool jumping = false; // flag &quot;I'm jumping to wall&quot;
	protected float vertSpeed = 0; // vertical jump current speed
	protected float external_control_speed = 1f;
	protected Transform myTransform;
	public BoxCollider boxCollider; // drag BoxCollider ref in editor

	private void Start ()
	{
		myNormal = transform.up; // normal starts as character up direction
		myTransform = transform;
		GetComponent<Rigidbody> ().freezeRotation = true; // disable physics rotation
		// distance from transform.position to ground
		distGround = boxCollider.extents.y - boxCollider.center.y;
	}
	
	public void setExternalSpeedCurrent (float e)
	{
		external_control_speed = e;
	}
	
	protected virtual float getControlSpeedNow ()
	{
		return external_control_speed;
	}
	
	private void FixedUpdate ()
	{
		// apply constant weight force according to character normal:
		GetComponent<Rigidbody> ().AddForce (-gravity * GetComponent<Rigidbody> ().mass * myNormal);
	}

	protected abstract void controlDirecitonTurns ();

	private void Update ()
	{
		// jump code - jump to wall or simple jump
		if (jumping)
			return; // abort Update while jumping to a wall
		
		Ray ray;
		RaycastHit hit;
		
		if (Input.GetButtonDown ("Jump")) { // jump pressed:
			ray = new Ray (myTransform.position, myTransform.forward);
			if (Physics.Raycast (ray, out hit, jumpRange)) { // wall ahead?
				JumpToWall (hit.point, hit.normal); // yes: jump to the wall
			} else if (isGrounded) { // no: if grounded, jump up
				GetComponent<Rigidbody> ().velocity += jumpSpeed * myNormal;
			}
		}
		
		controlDirecitonTurns ();
		// update surface normal and isGrounded:
		ray = new Ray (myTransform.position, -myNormal); // cast ray downwards
		if (Physics.Raycast (ray, out hit)) { // use it to update myNormal and isGrounded
			isGrounded = hit.distance <= distGround + deltaGround;
			surfaceNormal = hit.normal;
		} else {
			isGrounded = false;
			// assume usual ground normal to avoid "falling forever"
			surfaceNormal = Vector3.up;
		}
		myNormal = Vector3.Lerp (myNormal, surfaceNormal, lerpSpeed * Time.deltaTime);
		// find forward direction with new myNormal:
		Vector3 myForward = Vector3.Cross (myTransform.right, myNormal);
		// align character to the new myNormal while keeping the forward direction:
		Quaternion targetRot = Quaternion.LookRotation (myForward, myNormal);
		myTransform.rotation = Quaternion.Lerp (myTransform.rotation, targetRot, lerpSpeed * Time.deltaTime);
		// move the character forth/back with Vertical axis:
		myTransform.Translate (0, 0, getControlSpeedNow () * moveSpeed * Time.deltaTime);
		//Input.GetAxis ("Vertical") 
		checkhp();
	}
	protected abstract void checkhp();
	public void exploreFrom (Vector3 point, Vector3 normal, float distance)
	{
		// jump to wall
		jumping = true; // signal it's jumping to wall
		GetComponent<Rigidbody> ().isKinematic = true; // disable physics while jumping
		Vector3 orgPos = myTransform.position;
		Quaternion orgRot = myTransform.rotation;
		Vector3 dstPos = point + myTransform.up * (distGround + distance); // will jump to 0.5 above wall
		Vector3 myForward = Vector3.Cross (myTransform.right, normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, normal);
		
		StartCoroutine (jumpTime (orgPos, orgRot, dstPos, dstRot, myTransform.up));
		//jumptime
	}
	
	protected void JumpToWall (Vector3 point, Vector3 normal)
	{
		// jump to wall
		jumping = true; // signal it's jumping to wall
		GetComponent<Rigidbody> ().isKinematic = true; // disable physics while jumping
		Vector3 orgPos = myTransform.position;
		Quaternion orgRot = myTransform.rotation;
		Vector3 dstPos = point + normal * (distGround + 0.5f); // will jump to 0.5 above wall
		Vector3 myForward = Vector3.Cross (myTransform.right, normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, normal);
		
		StartCoroutine (jumpTime (orgPos, orgRot, dstPos, dstRot, normal));
		//jumptime
	}
	
	protected IEnumerator jumpTime (Vector3 orgPos, Quaternion orgRot, Vector3 dstPos, Quaternion dstRot, Vector3 normal)
	{
		for (float t = 0.0f; t < 1.0f;) {
			t += Time.deltaTime;
			myTransform.position = Vector3.Lerp (orgPos, dstPos, t);
			myTransform.rotation = Quaternion.Slerp (orgRot, dstRot, t);
			yield return null; // return here next frame
		}
		myNormal = normal; // update myNormal
		GetComponent<Rigidbody> ().isKinematic = false; // enable physics
		jumping = false; // jumping to wall finished
	}
	
}

