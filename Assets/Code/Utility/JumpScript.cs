using UnityEngine;
using System.Collections;

public class JumpScript : MonoBehaviour {

	bool onGround = false;
	float timer = 0;
	float timeTillJump = 5f;
	void OnTriggerEnter(Collider col)
	{
		if(onGround == false && col.tag == "Ground")
		{
			onGround = true;
			rigidbody.useGravity = false;
			timer = Time.time + timeTillJump;
			rigidbody.velocity = Vector3.zero;
		}
	}

	void OnCollisionEnter(Collision col)
	{
		if(onGround == false && col.gameObject.tag == "Ground")
		{
			onGround = true;
			timer = Time.time + timeTillJump;
		}
	}
	
	void Start () {
	
	}

	float timeIncrement = 0.3f;
	void Update () {
		if(onGround && Time.time > timer)
		{
			rigidbody.useGravity = true;
			onGround = false;
			rigidbody.AddForce(0, 40f, 0, ForceMode.Impulse);
		}

		Debug.DrawLine(transform.position, transform.position + (rigidbody.velocity *timeIncrement), Color.black);
	}
}
