using UnityEngine;
using System.Collections;

public class MouseOrbitCamScript : MonoBehaviour
{
	public Transform target;
	public float distance = 10.0f;
	
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;
	
	public float yMinLimit = -20;
	public float yMaxLimit = 80;
	
	public float distanceMin = .5f;
	public float distanceMax = 15f;

	public Vector3 camOffset = new Vector3(0.0f,0.0f,0.0f);
	
	private float x = 0.0f;
	private float y = 0.0f;
	
	void Start ()
	{
		var angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
	
	void Update ()
	{
		if (target && !GameManager.Instance().IsGameOver)
		{
			if(!GameManager.Instance().IsPaused)
			{
				x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
				y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
			}
			
			y = ClampAngle(y, yMinLimit, yMaxLimit);

			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);

			Quaternion rotation = Quaternion.Euler(y, x, 0);
			Vector3 position = rotation * negDistance + target.position + camOffset;

			transform.rotation = rotation;
			transform.position = position;
		}
	}
	
	static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
		{
			angle += 360;
		}
		if (angle > 360)
		{
			angle -= 360;
		}
		return Mathf.Clamp(angle, min, max);
	}
}