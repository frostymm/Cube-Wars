using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class SteeringBehavior
{
	private string m_Name = "";
	private float m_Weight = 1;
	private bool m_IsActive = false;

	public SteeringBehavior(string name)
	{
	    m_Name = name;
	}

	public string GetName() { return m_Name; }
	public abstract Vector3 GetForce(MovingEntity me);
	public float GetWeight() { return m_Weight; }
	public void SetWeight(float weight) { m_Weight = weight; }
	public bool GetIsActive() { return m_IsActive; }
	public void SetIsActive(bool active) { m_IsActive = active; }
}
