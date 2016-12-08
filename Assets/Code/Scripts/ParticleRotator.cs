using UnityEngine;
using System.Collections;

public class ParticleRotator : MonoBehaviour {

	public Vector3 particleRotation = new Vector3();
	public float particleRotationSpeed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(particleRotation * particleRotationSpeed);
	}
}
