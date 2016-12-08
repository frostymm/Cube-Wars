using UnityEngine;
using System.Collections;

public class StabilizerScript : MonoBehaviour {
	
	public float stability = 0.3f;
	public float speed = 2.0f;
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 predictedUp = Quaternion.AngleAxis(
			rigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed,
			rigidbody.angularVelocity
			) * transform.up;
		
		Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
		rigidbody.AddTorque(torqueVector * speed * speed);
	}
}