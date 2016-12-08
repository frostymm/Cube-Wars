using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TankBodyController : MonoBehaviour {

	public BaseBody bb;
	public Collider maincollider;
	public Transform turretConnector;

	public GameObject wheelCollider;
	public GameObject leftTrack;
	public Transform[] leftTrackUpperWheels;
	public Transform[] leftTrackWheels;
	public Transform[] leftTrackBones;
	public GameObject rightTrack;
	public Transform[] rightTrackUpperWheels;
	public Transform[] rightTrackWheels;
	public Transform[] rightTrackBones;

	private bool variablesSet = false;

	// Use this for initialization
	void SetVariables() 
	{
		bb.m_MovingEntity.SetCollider(maincollider);
		bb.turretConnector = turretConnector;

		if(bb.m_MovingEntity.GetGameObject().GetPhotonView().isMine)
		{
			bb.wheelCollider = wheelCollider;
			bb.leftTrack = leftTrack;
			bb.leftTrackUpperWheels = leftTrackUpperWheels;
			bb.leftTrackWheels = leftTrackWheels;
			bb.leftTrackBones = leftTrackBones;
			bb.rightTrack = rightTrack;
			bb.rightTrackUpperWheels = rightTrackUpperWheels;
			bb.rightTrackWheels = rightTrackWheels;
			bb.rightTrackBones = rightTrackBones;
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
		}

	}
}
