using UnityEngine;
using System.Collections;

public class TankTurretController : MonoBehaviour 
{
	public BaseTurret bt;
	public Transform turret;
	public Transform barrel;
	private bool variablesSet = false;
	public Transform[] firingPoints;

	// Use this for initialization
	void SetVariables () 
	{
		bt.turret = turret;
		bt.barrel = barrel;
		bt.m_FiringPoints = firingPoints;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(bt != null)
		{
			if(!variablesSet)
			{
				SetVariables();
				variablesSet = true;
			}
		}
	}
}
