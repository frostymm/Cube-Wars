using UnityEngine;
using System.Collections;

public class TankBasicBodyController : MonoBehaviour {

	public Body bb;
	public bool centerOfMassDebug = false;
	public Vector3 centerOfMass;
	public Collider maincollider;
	public Transform turretConnector;

	public WheelCollider[] leftTrackWheels;
	public WheelCollider[] rightTrackWheels;
	
	private bool variablesSet = false;

	public GameObject[] OtherObjectVariables;

	// Use this for initialization
	void SetVariables() 
	{
		bb.m_MovingEntity.SetCollider(maincollider);
		bb.turretConnector = turretConnector;

		if(bb.m_MovingEntity.GetGameObject().GetPhotonView().isMine)
		{
			bb.SetLeftWheels(leftTrackWheels);
			bb.SetRightWheels(rightTrackWheels);
		}

		foreach(GameObject go in OtherObjectVariables)
		{
			bb.AddObjectVariable(go.name, go);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(bb != null)
		{
			if(!variablesSet)
			{
				SetVariables();
				variablesSet = true;
			}

			if(centerOfMassDebug && rigidbody.centerOfMass != centerOfMass)
				rigidbody.centerOfMass = centerOfMass;
		}
		
	}
}
