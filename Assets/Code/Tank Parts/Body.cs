using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Body{

	public MovingEntity m_MovingEntity;
	
	public abstract void Initialize();
	public abstract void FixedUpdate();
	public abstract void Awake();

	private GameObject m_GameObject;
	public GameObject GetGameObject (){ return m_GameObject; }
	public void SetGameObject (GameObject go){ m_GameObject = go; }
	public bool HasGameObject(){ return (m_GameObject); }

	public bool HasTurretConnector(){ return (turretConnector); }

	private WheelCollider[] m_LeftWheels;
	public WheelCollider[] GetLeftWheels(){ return m_LeftWheels; }
	public void SetLeftWheels(WheelCollider[] leftWheels){ m_LeftWheels = leftWheels; }

	private WheelCollider[] m_RightWheels;
	public WheelCollider[] GetRightWheels(){ return m_RightWheels; }
	public void SetRightWheels(WheelCollider[] rightWheels){ m_RightWheels = rightWheels; }

	private float m_JumpForce;
	public float GetJumpForce(){ return m_JumpForce; }
	public void SetJumpForce(float jumpForce){ m_JumpForce = jumpForce; }

	private Dictionary<string, GameObject> m_ObjectVariables = new Dictionary<string, GameObject>();
	public void AddObjectVariable(string s, GameObject go){ m_ObjectVariables.Add(s, go); }
	public bool HasObjectVariable(string s){ return m_ObjectVariables.ContainsKey(s); }
	public GameObject GetObjectVariable(string s){ return m_ObjectVariables[s]; }


	public Transform turretConnector;
}
