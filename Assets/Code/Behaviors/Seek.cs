using System;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehavior
{
	Vector3 m_TargetPosition;

	public Seek() : base("Seek") {}

	public void SetTargetPosition(Vector3 tp)
	{
	    m_TargetPosition = tp;
	}

	public override Vector3 GetForce(MovingEntity me)
	{
		m_TargetPosition = new Vector3(0,0,0);//me.target.transform.position;

	    Vector3 desiredVelocity = Vector3.Normalize(m_TargetPosition - me.GetPosition())
	                    * me.GetMaxSpeed();

	    return (desiredVelocity - me.GetVelocity());
	}
}
